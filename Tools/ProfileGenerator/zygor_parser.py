#!/usr/bin/env python3
"""
Zygor Guide to CopilotBuddy Profile Converter
Parses Zygor leveling guides and generates CopilotBuddy XML profiles.
Uses Questie database for smart quest<->objective mapping.
"""

import re
import os
import sys
import json
from dataclasses import dataclass, field
from typing import List, Optional, Dict, Tuple, Set
from pathlib import Path

# ============================================================================
# Data Classes
# ============================================================================

@dataclass
class Hotspot:
    x: float
    y: float
    z: float = 0.0

@dataclass 
class QuestStep:
    """Represents a single step in a quest profile"""
    step_type: str  # PickUp, TurnIn, Objective, etc.
    quest_id: Optional[int] = None
    quest_name: Optional[str] = None
    npc_id: Optional[int] = None
    npc_name: Optional[str] = None
    mob_id: Optional[int] = None
    mob_name: Optional[str] = None
    kill_count: Optional[int] = None
    item_id: Optional[int] = None
    item_name: Optional[str] = None
    collect_count: Optional[int] = None
    hotspots: List[Hotspot] = field(default_factory=list)
    zone: Optional[str] = None
    map_id: Optional[int] = None
    race_filter: Optional[str] = None
    class_filter: Optional[str] = None
    negate_filter: bool = False  # True for |only if not X (everyone EXCEPT X)
    objective_index: Optional[int] = None
    game_object_id: Optional[int] = None  # For clickable world objects
    game_object_name: Optional[str] = None

@dataclass
class QuestInfo:
    """Questie quest data with objectives"""
    quest_id: int
    name: str
    giver_npc_id: Optional[int] = None
    turnin_npc_id: Optional[int] = None
    mob_objectives: List[int] = field(default_factory=list)  # Mob IDs to kill
    item_objectives: List[int] = field(default_factory=list)  # Item IDs to collect
    objectives_text: List[str] = field(default_factory=list)  # Full objectives text for extracting counts

@dataclass
class Guide:
    """Represents a parsed Zygor guide"""
    name: str
    min_level: int = 1
    max_level: int = 60
    faction: str = "Horde"
    race: Optional[str] = None
    steps: List[QuestStep] = field(default_factory=list)
    next_guide: Optional[str] = None  # Name of next guide to chain to

# ============================================================================
# Zone Mappings (Zygor zone names to MapId)
# ============================================================================

ZONE_TO_MAPID = {
    # Kalimdor
    "Durotar": 14,
    "Valley of Trials": 363,
    "Mulgore": 215,
    "Camp Narache": 221,
    "Tirisfal Glades": 85,
    "Deathknell": 20,
    "The Barrens": 17,
    "Orgrimmar": 1637,
    "Thunder Bluff": 1638,
    "Undercity": 1497,
    "Ashenvale": 331,
    "Stonetalon Mountains": 406,
    "Thousand Needles": 400,
    "Desolace": 405,
    "Dustwallow Marsh": 15,
    "Feralas": 357,
    "Tanaris": 440,
    "Un'Goro Crater": 490,
    "Silithus": 1377,
    "Felwood": 361,
    "Winterspring": 618,
    "Azshara": 16,
    "Moonglade": 493,
    
    # Eastern Kingdoms  
    "Elwynn Forest": 12,
    "Northshire": 9,
    "Westfall": 40,
    "Redridge Mountains": 44,
    "Duskwood": 10,
    "Stranglethorn Vale": 33,
    "Swamp of Sorrows": 8,
    "Blasted Lands": 4,
    "Burning Steppes": 46,
    "Searing Gorge": 51,
    "Badlands": 3,
    "Loch Modan": 38,
    "Wetlands": 11,
    "Arathi Highlands": 45,
    "Hillsbrad Foothills": 267,
    "Alterac Mountains": 36,
    "Silverpine Forest": 130,
    "Western Plaguelands": 28,
    "Eastern Plaguelands": 139,
    "The Hinterlands": 47,
    "Dun Morogh": 1,
    "Ironforge": 1537,
    "Stormwind City": 1519,
    
    # Blood Elf / Draenei
    "Eversong Woods": 3430,
    "Sunstrider Isle": 3430,
    "Ghostlands": 3433,
    "Silvermoon City": 3487,
    
    # Outland
    "Hellfire Peninsula": 3483,
    "Zangarmarsh": 3521,
    "Terokkar Forest": 3519,
    "Nagrand": 3518,
    "Blade's Edge Mountains": 3522,
    "Netherstorm": 3523,
    "Shadowmoon Valley": 3520,
    "Shattrath City": 3703,
    
    # Northrend
    "Borean Tundra": 3537,
    "Howling Fjord": 495,
    "Dragonblight": 65,
    "Grizzly Hills": 394,
    "Zul'Drak": 66,
    "Sholazar Basin": 3711,
    "The Storm Peaks": 67,
    "Icecrown": 210,
    "Crystalsong Forest": 2817,
    "Dalaran": 4395,
    
    # Death Knight
    "Plaguelands: The Scarlet Enclave": 139,
}

# ============================================================================
# Parser Regex Patterns
# ============================================================================

# Guide header pattern
GUIDE_HEADER = re.compile(
    r'RegisterGuide\("([^"]+)".*?'
    r'condition_suggested="([^"]*)".*?'
    r'next="([^"]*)"',
    re.DOTALL
)

# Step patterns
PATTERN_TALK = re.compile(r'talk\s+([^#]+)##(\d+)')
PATTERN_ACCEPT = re.compile(r'accept\s+([^#]+)##(\d+)')
PATTERN_TURNIN = re.compile(r'turnin\s+([^#]+)##(\d+)')
PATTERN_KILL = re.compile(r'kill\s+(?:(\d+)\s+)?([^#]+)##(\d+)')
PATTERN_COLLECT = re.compile(r'collect\s+(?:(\d+)\s+)?([^#]+)##(\d+)')
# Match |goto with optional zone: |goto Zone X,Y or |goto X,Y
PATTERN_GOTO = re.compile(r'\|goto\s+(?:([^/\d][^/]*?)(?:/\d+)?\s+)?([\d.]+)\s*,\s*([\d.]+)')
PATTERN_QUEST_OBJ = re.compile(r'\|q\s+(\d+)/(\d+)')
PATTERN_ONLY_IF = re.compile(r'\|only\s+(?:if\s+)?(.+)$')  # Handles both |only if X and |only X
# Match click with optional ID: click Name##ID or click Name
PATTERN_CLICK = re.compile(r'click\s+([^#\n]+?)(?:##(\d+))?(?:\s|$|\|)')

# Race patterns for filtering (Scourge = Undead in Zygor guides)
RACE_KEYWORDS = ['Orc', 'Troll', 'Tauren', 'Undead', 'Scourge', 'BloodElf', 'Goblin',
                 'Human', 'Dwarf', 'NightElf', 'Gnome', 'Draenei', 'Worgen']
CLASS_KEYWORDS = ['Warrior', 'Paladin', 'Hunter', 'Rogue', 'Priest', 
                  'Shaman', 'Mage', 'Warlock', 'Druid', 'DeathKnight']  # No Monk in WotLK

# ============================================================================
# Parser Class
# ============================================================================

class ZygorParser:
    def __init__(self, questie_db_path: Optional[str] = None, spawns_json_path: Optional[str] = None):
        self.questie_npcs: Dict[int, dict] = {}  # npcId -> {name, zone, x, y}
        self.questie_quests: Dict[int, QuestInfo] = {}  # questId -> QuestInfo
        self.questie_items: Dict[int, str] = {}  # itemId -> name
        
        # Inverse indexes for smart objective resolution
        self.mob_to_quests: Dict[int, Set[int]] = {}  # mobId -> set of questIds
        self.item_to_quests: Dict[int, Set[int]] = {}  # itemId -> set of questIds
        
        # Item to GameObject mapping (which objects drop which items)
        self.item_to_objects: Dict[int, List[int]] = {}  # itemId -> [gameObjectIds]
        
        # GameObject world spawns (from server DB export)
        self.gameobject_spawns: Dict[int, List[Dict]] = {}  # gameObjectId -> [{x, y, z}, ...]
        self.gameobject_names: Dict[int, str] = {}  # gameObjectId -> name
        
        if questie_db_path:
            self._load_questie_db(questie_db_path)
        
        # Load spawns JSON if provided, or look for default location
        if spawns_json_path:
            self._load_spawns_json(spawns_json_path)
        else:
            # Try default location next to this script
            default_path = Path(__file__).parent / "gameobject_spawns.json"
            if default_path.exists():
                self._load_spawns_json(str(default_path))
    
    def _load_questie_db(self, path: str):
        """Load Questie database for NPC coordinates and quest objectives"""
        npc_file = Path(path) / "Database" / "Wotlk" / "wotlkNpcDB.lua"
        quest_file = Path(path) / "Database" / "Wotlk" / "wotlkQuestDB.lua"
        item_file = Path(path) / "Database" / "Wotlk" / "wotlkItemDB.lua"
        
        if npc_file.exists():
            self._parse_questie_npcs(npc_file)
            print(f"  Loaded {len(self.questie_npcs)} NPCs from Questie")
        if quest_file.exists():
            self._parse_questie_quests(quest_file)
            print(f"  Loaded {len(self.questie_quests)} quests from Questie")
            print(f"  Built {len(self.mob_to_quests)} mob->quest mappings")
            print(f"  Built {len(self.item_to_quests)} item->quest mappings")
        if item_file.exists():
            self._parse_questie_items(item_file)
            print(f"  Loaded {len(self.questie_items)} items from Questie")
    
    def _parse_questie_npcs(self, filepath: Path):
        """Parse Questie NPC database for coordinates (fast line-by-line)"""
        content = filepath.read_text(encoding='utf-8', errors='ignore')
        
        # Fast line-by-line parsing - each NPC is on one line
        # Format: [ID] = {'Name', ..., {[zoneID]={{x,y}}}, ...}
        npc_pattern = re.compile(r'^\[(\d+)\]\s*=\s*\{\'([^\']+)\'')
        coord_pattern = re.compile(r'\{(\d+)\}\s*=\s*\{\{([\d.]+),([\d.]+)\}')
        
        for line in content.split('\n'):
            npc_match = npc_pattern.match(line)
            if npc_match:
                npc_id = int(npc_match.group(1))
                npc_name = npc_match.group(2)
                
                # Find first coordinate in this line
                coord_match = coord_pattern.search(line)
                if coord_match:
                    zone_id = int(coord_match.group(1))
                    x = float(coord_match.group(2))
                    y = float(coord_match.group(3))
                    
                    self.questie_npcs[npc_id] = {
                        'name': npc_name,
                        'zone': zone_id,
                        'x': x,
                        'y': y
                    }
    
    def _parse_questie_quests(self, filepath: Path):
        """Parse Questie quest database with full objectives"""
        content = filepath.read_text(encoding='utf-8', errors='ignore')
        
        # Parse quest entries line by line for speed
        # Format: [questId] = {"name", {startedBy}, {finishedBy}, ..., {objectives}, ...}
        # startedBy: {{npcIds}, {objectIds}, {itemIds}}
        # finishedBy: {{npcIds}, {objectIds}}
        # objectives (index 9, 0-based): {{mobs}, {objects}, {items}, ...}
        
        quest_pattern = re.compile(r'^\[(\d+)\]\s*=\s*\{"([^"]+)"')
        
        for line in content.split('\n'):
            quest_match = quest_pattern.match(line)
            if not quest_match:
                continue
            
            quest_id = int(quest_match.group(1))
            quest_name = quest_match.group(2)
            
            quest_info = QuestInfo(quest_id=quest_id, name=quest_name)
            
            # Extract giver NPC ID from startedBy {{npcId}}
            giver_match = re.search(r'^\[\d+\]\s*=\s*\{[^,]+,\{\{(\d+)\}', line)
            if giver_match:
                quest_info.giver_npc_id = int(giver_match.group(1))
            
            # Extract turnin NPC ID from finishedBy {{npcId}}  
            turnin_match = re.search(r'^\[\d+\]\s*=\s*\{[^,]+,[^,]+,\{\{(\d+)\}', line)
            if turnin_match:
                quest_info.turnin_npc_id = int(turnin_match.group(1))
            
            # Extract creature objectives (mobs to kill)
            # Look for pattern like: nil,{{{mobId}}}, or nil,{{{mobId},{mobId2}}}
            # The objectives field is index 9 (counting from 0)
            # Format: {{{mobId}}, {{mobId2}}} for multiple mobs
            mob_obj_match = re.search(r',nil,\{\{\{(\d+)', line)
            if mob_obj_match:
                # Found first mob, look for more
                mob_ids = re.findall(r'\{\{(\d+)[,\}]', line[mob_obj_match.start():])
                for mob_id_str in mob_ids[:5]:  # Limit to 5 mobs
                    mob_id = int(mob_id_str)
                    quest_info.mob_objectives.append(mob_id)
                    # Build inverse index
                    if mob_id not in self.mob_to_quests:
                        self.mob_to_quests[mob_id] = set()
                    self.mob_to_quests[mob_id].add(quest_id)
            
            # Extract item objectives
            # Format: {nil,nil,{{itemId}}} within objectives
            item_obj_match = re.search(r'\{nil,nil,\{\{(\d+)', line)
            if item_obj_match:
                item_ids = re.findall(r'\{\{(\d+)[,\}]', line[item_obj_match.start():])
                for item_id_str in item_ids[:5]:  # Limit to 5 items
                    item_id = int(item_id_str)
                    quest_info.item_objectives.append(item_id)
                    # Build inverse index
                    if item_id not in self.item_to_quests:
                        self.item_to_quests[item_id] = set()
                    self.item_to_quests[item_id].add(quest_id)
            
            # Extract objectivesText (position 8) - contains description like "Kill 8 Mottled Boars..."
            # Format: nil,{"objective text 1","objective text 2",...}
            # The nil is from requiredClasses (position 7)
            obj_text_match = re.search(r'nil,\{"([^"]*)"', line)
            if obj_text_match:
                # Extract all objective texts from the array
                obj_texts = re.findall(r'"([^"]+)"', line[obj_text_match.start():obj_text_match.start()+500])
                quest_info.objectives_text = obj_texts[:5]  # Limit to 5 objectives
            
            self.questie_quests[quest_id] = quest_info
    
    def _parse_questie_items(self, filepath: Path):
        """Parse Questie item database for names and object drops"""
        content = filepath.read_text(encoding='utf-8', errors='ignore')
        
        # Format: [itemId] = {'itemName', npcDrops, objectDrops, ...}
        # objectDrops (index 3) is {obj_id1, obj_id2} or nil
        # Example: [11583] = {'Cactus Apple',nil,{171938},...}
        item_pattern = re.compile(r'^\[(\d+)\]\s*=\s*\{[\'"]([^\'"]+)[\'"],([^,]*),(\{[^}]*\}|nil)')
        
        for line in content.split('\n'):
            item_match = item_pattern.match(line)
            if item_match:
                item_id = int(item_match.group(1))
                item_name = item_match.group(2)
                # npc_drops = item_match.group(3)  # nil or {id1,id2}
                object_drops = item_match.group(4)  # nil or {id1,id2}
                
                self.questie_items[item_id] = item_name
                
                # Extract objectDrops (GameObjects that drop this item)
                if object_drops and object_drops != 'nil':
                    obj_ids = re.findall(r'(\d+)', object_drops)
                    if obj_ids:
                        self.item_to_objects[item_id] = [int(x) for x in obj_ids]
        
        if self.item_to_objects:
            print(f"  Built {len(self.item_to_objects)} item->object mappings")
    
    def _load_spawns_json(self, filepath: str):
        """Load GameObject world spawns from JSON (exported from server DB)"""
        try:
            with open(filepath, 'r', encoding='utf-8') as f:
                data = json.load(f)
            
            objects_data = data.get('objects', {})
            for obj_id_str, obj_info in objects_data.items():
                obj_id = int(obj_id_str)
                self.gameobject_names[obj_id] = obj_info.get('name', f'Unknown {obj_id}')
                self.gameobject_spawns[obj_id] = obj_info.get('spawns', [])
            
            total_spawns = sum(len(v) for v in self.gameobject_spawns.values())
            print(f"  Loaded {len(self.gameobject_spawns)} GameObjects with {total_spawns} spawn points")
        except Exception as e:
            print(f"Warning: Failed to load spawns JSON: {e}")
    
    def find_quest_for_mob(self, mob_id: int, active_quests: Set[int]) -> Optional[int]:
        """Find which active quest requires killing this mob"""
        if mob_id not in self.mob_to_quests:
            return None
        
        possible_quests = self.mob_to_quests[mob_id] & active_quests
        if possible_quests:
            return next(iter(possible_quests))
        return None
    
    def find_quest_for_item(self, item_id: int, active_quests: Set[int]) -> Optional[int]:
        """Find which active quest requires this item"""
        if item_id not in self.item_to_quests:
            return None
        
        possible_quests = self.item_to_quests[item_id] & active_quests
        if possible_quests:
            return next(iter(possible_quests))
        return None
    
    def get_kill_count_from_questie(self, quest_id: int, mob_name: str) -> Optional[int]:
        """Extract kill count from Questie objectives_text by matching mob name.
        
        Examples of text patterns:
        - "Kill 8 Mottled Boars then return to..."
        - "Slay 10 Bristleback Quilboars."
        - "Defeat 5 Scorpid Workers."
        """
        if quest_id not in self.questie_quests:
            return None
        
        quest = self.questie_quests[quest_id]
        if not quest.objectives_text:
            return None
        
        # Normalize mob name for matching (remove special chars, lowercase)
        mob_name_clean = mob_name.lower().strip()
        
        for text in quest.objectives_text:
            text_lower = text.lower()
            
            # Check if this objective mentions the mob
            if mob_name_clean in text_lower:
                # Try patterns: "Kill N", "Slay N", "Defeat N", just "N MobName"
                patterns = [
                    rf'(?:kill|slay|defeat)\s+(\d+)\s+{re.escape(mob_name_clean)}',
                    rf'(\d+)\s+{re.escape(mob_name_clean)}',
                ]
                for pattern in patterns:
                    match = re.search(pattern, text_lower)
                    if match:
                        return int(match.group(1))
        
        return None

    def parse_guide(self, content: str, guide_name: str = "Unknown") -> List[Guide]:
        """Parse a Zygor guide file and return list of Guide objects"""
        guides = []
        
        # Split by guide registration
        guide_blocks = re.split(r'ZygorGuidesViewer:RegisterGuide\(', content)
        
        for block in guide_blocks[1:]:  # Skip first empty block
            guide = self._parse_guide_block(block)
            if guide and guide.steps:
                guides.append(guide)
        
        return guides
    
    def _parse_guide_block(self, block: str) -> Optional[Guide]:
        """Parse a single guide block with active quest tracking"""
        # Extract guide name
        name_match = re.match(r'"([^"]+)"', block)
        if not name_match:
            return None
        
        guide_name = name_match.group(1)
        guide = Guide(name=guide_name)
        
        # Extract faction/race from name
        if 'Horde' in guide_name:
            guide.faction = 'Horde'
        elif 'Alliance' in guide_name:
            guide.faction = 'Alliance'
        
        # Extract level range from condition_suggested
        level_match = re.search(r'level\s*<=\s*(\d+)', block)
        if level_match:
            guide.max_level = int(level_match.group(1))
        
        # Extract race from condition
        race_match = re.search(r"raceclass\('(\w+)'\)", block)
        if race_match:
            guide.race = race_match.group(1)
        
        # Extract next guide from metadata
        next_match = re.search(r'next="([^"]+)"', block)
        if next_match:
            guide.next_guide = next_match.group(1)
        
        # Track active quests (between PickUp and TurnIn)
        # dict: quest_id -> quest_name
        active_quests: Dict[int, str] = {}
        
        # Parse steps
        step_blocks = re.split(r'\nstep\n', block)
        current_npc_id = None
        current_npc_name = None
        current_zone = None
        current_coords = None
        current_map_id = None
        current_game_object_id = None  # Track last clicked GameObject
        current_game_object_name = None
        for step_block in step_blocks:
            lines = step_block.strip().split('\n')
            
            # Helper function to extract filter from a line
            def extract_filter(line_text):
                """Extract race, class, and negate flag from |only [if] [not] condition"""
                race, cls, negate = None, None, False
                only_match = PATTERN_ONLY_IF.search(line_text)
                if only_match:
                    condition = only_match.group(1).strip()
                    # Check for negation: "not Orc Warlock" or "not Hunter"
                    if condition.lower().startswith('not '):
                        negate = True
                        condition = condition[4:]  # Remove "not "
                    for r in RACE_KEYWORDS:
                        if r in condition:
                            race = r
                    for c in CLASS_KEYWORDS:
                        if c in condition:
                            cls = c
                return race, cls, negate
            
            for i, line in enumerate(lines):
                line = line.strip()
                if not line:
                    continue
                
                # Reset per-line filters (|only if applies to current line only)
                line_race_filter = None
                line_class_filter = None
                line_negate_filter = False
                
                # Extract goto coordinates: |goto Zone X,Y or |goto X,Y
                goto_match = PATTERN_GOTO.search(line)
                if goto_match:
                    zone_from_goto = goto_match.group(1)
                    if zone_from_goto:
                        current_zone = zone_from_goto.strip()
                        current_map_id = ZONE_TO_MAPID.get(current_zone)
                    current_coords = (float(goto_match.group(2)), float(goto_match.group(3)))
                
                # Extract only if condition from CURRENT line first
                line_race_filter, line_class_filter, line_negate_filter = extract_filter(line)
                
                # If no filter on current line, check NEXT line (Zygor multi-line format)
                # Example: accept Quest##ID\n|only if Orc Warlock
                if not line_race_filter and not line_class_filter and i + 1 < len(lines):
                    next_line = lines[i + 1].strip()
                    # Only use next line filter if it STARTS with |only (continuation line)
                    if next_line.startswith('|only'):
                        line_race_filter, line_class_filter, line_negate_filter = extract_filter(next_line)
                
                # Parse click (sets current GameObject for collect)
                click_match = PATTERN_CLICK.search(line)
                if click_match:
                    current_game_object_name = click_match.group(1).strip()
                    current_game_object_id = int(click_match.group(2)) if click_match.group(2) else None
                
                # Parse talk (sets current NPC for accept/turnin)
                talk_match = PATTERN_TALK.search(line)
                if talk_match:
                    current_npc_name = talk_match.group(1).strip()
                    current_npc_id = int(talk_match.group(2))
                
                # Parse accept quest
                accept_match = PATTERN_ACCEPT.search(line)
                if accept_match:
                    quest_name = accept_match.group(1).strip()
                    quest_id = int(accept_match.group(2))
                    
                    # Add to active quests
                    active_quests[quest_id] = quest_name
                    
                    step = QuestStep(
                        step_type="PickUp",
                        quest_id=quest_id,
                        quest_name=quest_name,
                        npc_id=current_npc_id,
                        npc_name=current_npc_name,
                        zone=current_zone,
                        map_id=current_map_id,
                        race_filter=line_race_filter,
                        class_filter=line_class_filter,
                        negate_filter=line_negate_filter
                    )
                    if current_coords:
                        step.hotspots.append(Hotspot(x=current_coords[0], y=current_coords[1]))
                    guide.steps.append(step)
                
                # Parse turnin quest
                turnin_match = PATTERN_TURNIN.search(line)
                if turnin_match:
                    quest_name = turnin_match.group(1).strip()
                    quest_id = int(turnin_match.group(2))
                    
                    # Remove from active quests
                    active_quests.pop(quest_id, None)
                    
                    step = QuestStep(
                        step_type="TurnIn",
                        quest_id=quest_id,
                        quest_name=quest_name,
                        npc_id=current_npc_id,
                        npc_name=current_npc_name,
                        zone=current_zone,
                        map_id=current_map_id,
                        race_filter=line_race_filter,
                        class_filter=line_class_filter,
                        negate_filter=line_negate_filter
                    )
                    if current_coords:
                        step.hotspots.append(Hotspot(x=current_coords[0], y=current_coords[1]))
                    guide.steps.append(step)
                
                # Parse kill objective
                kill_match = PATTERN_KILL.search(line)
                if kill_match:
                    zygor_count = int(kill_match.group(1)) if kill_match.group(1) else None
                    mob_name = kill_match.group(2).strip()
                    mob_id = int(kill_match.group(3))
                    
                    # Get quest objective info from |q QuestID/ObjectiveIndex
                    quest_obj_match = PATTERN_QUEST_OBJ.search(line)
                    quest_id = int(quest_obj_match.group(1)) if quest_obj_match else None
                    obj_index = int(quest_obj_match.group(2)) if quest_obj_match else None
                    
                    # If no quest_id from Zygor, try to find it from Questie mappings
                    if not quest_id:
                        quest_id = self.find_quest_for_mob(mob_id, set(active_quests.keys()))
                    
                    # Determine kill count: Questie first (accurate for 3.3.5a), then Zygor, then default 1
                    # Zygor guides may have data from different WoW versions, Questie-335 is accurate for WotLK
                    count = 1
                    if quest_id:
                        questie_count = self.get_kill_count_from_questie(quest_id, mob_name)
                        if questie_count:
                            count = questie_count
                        elif zygor_count:
                            count = zygor_count
                    elif zygor_count:
                        count = zygor_count
                    
                    # Get quest name from active quests or Questie
                    quest_name = None
                    if quest_id:
                        quest_name = active_quests.get(quest_id)
                        if not quest_name and quest_id in self.questie_quests:
                            quest_name = self.questie_quests[quest_id].name
                    
                    step = QuestStep(
                        step_type="Objective",
                        quest_id=quest_id,
                        quest_name=quest_name,
                        mob_id=mob_id,
                        mob_name=mob_name,
                        kill_count=count,
                        zone=current_zone,
                        map_id=current_map_id,
                        objective_index=obj_index,
                        race_filter=line_race_filter,
                        class_filter=line_class_filter,
                        negate_filter=line_negate_filter
                    )
                    if current_coords:
                        step.hotspots.append(Hotspot(x=current_coords[0], y=current_coords[1]))
                    guide.steps.append(step)
                
                # Parse collect objective
                collect_match = PATTERN_COLLECT.search(line)
                if collect_match:
                    count = int(collect_match.group(1)) if collect_match.group(1) else 1
                    item_name = collect_match.group(2).strip()
                    item_id = int(collect_match.group(3))
                    
                    # Get quest objective info
                    quest_obj_match = PATTERN_QUEST_OBJ.search(line)
                    quest_id = int(quest_obj_match.group(1)) if quest_obj_match else None
                    obj_index = int(quest_obj_match.group(2)) if quest_obj_match else None
                    
                    # If no quest_id from Zygor, try to find it from Questie mappings
                    if not quest_id:
                        quest_id = self.find_quest_for_item(item_id, set(active_quests.keys()))
                    
                    # Get quest name from active quests or Questie
                    quest_name = None
                    if quest_id:
                        quest_name = active_quests.get(quest_id)
                        if not quest_name and quest_id in self.questie_quests:
                            quest_name = self.questie_quests[quest_id].name
                    
                    step = QuestStep(
                        step_type="CollectItem",
                        quest_id=quest_id,
                        quest_name=quest_name,
                        item_id=item_id,
                        item_name=item_name,
                        collect_count=count,
                        zone=current_zone,
                        map_id=current_map_id,
                        objective_index=obj_index,
                        race_filter=line_race_filter,
                        class_filter=line_class_filter,
                        negate_filter=line_negate_filter,
                        game_object_id=current_game_object_id,
                        game_object_name=current_game_object_name
                    )
                    if current_coords:
                        step.hotspots.append(Hotspot(x=current_coords[0], y=current_coords[1]))
                    guide.steps.append(step)
                    # Reset GameObject after use (only applies to immediate next collect)
                    current_game_object_id = None
                    current_game_object_name = None
        
        return guide
    
    def enrich_with_questie(self, guide: Guide):
        """Enrich guide data with Questie database information"""
        for step in guide.steps:
            # Enrich NPC coordinates from Questie
            if step.npc_id and step.npc_id in self.questie_npcs:
                npc_data = self.questie_npcs[step.npc_id]
                if not step.hotspots:
                    step.hotspots.append(Hotspot(x=npc_data['x'], y=npc_data['y']))
                if not step.npc_name:
                    step.npc_name = npc_data['name']
            
            # Enrich mob coordinates from Questie
            if step.mob_id and step.mob_id in self.questie_npcs:
                mob_data = self.questie_npcs[step.mob_id]
                if not step.hotspots:
                    step.hotspots.append(Hotspot(x=mob_data['x'], y=mob_data['y']))
                if not step.mob_name:
                    step.mob_name = mob_data['name']
            
            # Enrich quest names from Questie
            if step.quest_id and step.quest_id in self.questie_quests:
                quest_info = self.questie_quests[step.quest_id]
                if not step.quest_name:
                    step.quest_name = quest_info.name
                
                # Enrich NPC for PickUp/TurnIn from Questie
                if step.step_type == "PickUp" and not step.npc_id:
                    if quest_info.giver_npc_id:
                        step.npc_id = quest_info.giver_npc_id
                        if step.npc_id in self.questie_npcs:
                            step.npc_name = self.questie_npcs[step.npc_id]['name']
                
                if step.step_type == "TurnIn" and not step.npc_id:
                    if quest_info.turnin_npc_id:
                        step.npc_id = quest_info.turnin_npc_id
                        if step.npc_id in self.questie_npcs:
                            step.npc_name = self.questie_npcs[step.npc_id]['name']
            
            # Enrich item names from Questie
            if step.item_id and step.item_id in self.questie_items:
                if not step.item_name:
                    step.item_name = self.questie_items[step.item_id]

# ============================================================================
# XML Generator
# ============================================================================

# Mapping Zygor race/class names to WoW API names
RACE_TO_WOWRACE = {
    'Orc': 'Orc', 'Troll': 'Troll', 'Tauren': 'Tauren', 'Undead': 'Undead',
    'Scourge': 'Undead',  # Zygor uses "Scourge" for Undead race
    'BloodElf': 'BloodElf', 'Goblin': 'Goblin',
    'Human': 'Human', 'Dwarf': 'Dwarf', 'NightElf': 'NightElf', 'Gnome': 'Gnome',
    'Draenei': 'Draenei', 'Worgen': 'Worgen',
}

CLASS_TO_WOWCLASS = {
    'Warrior': 'Warrior', 'Paladin': 'Paladin', 'Hunter': 'Hunter',
    'Rogue': 'Rogue', 'Priest': 'Priest', 'Shaman': 'Shaman',
    'Mage': 'Mage', 'Warlock': 'Warlock', 'Druid': 'Druid',
    'DeathKnight': 'DeathKnight',  # No Monk in WotLK 3.3.5a
}

class ProfileGenerator:
    def __init__(self, parser: 'ZygorParser' = None):
        self.parser = parser
    
    def _build_condition(self, race: Optional[str], cls: Optional[str], negate: bool = False) -> Optional[str]:
        """Build XML condition string for race/class filtering
        
        If negate=True, builds condition for everyone EXCEPT the specified race/class
        Example: negate=True, race=Orc, cls=Warlock -> !(Me.Race == Orc && Me.Class == Warlock)
        """
        conditions = []
        if cls and cls in CLASS_TO_WOWCLASS:
            conditions.append(f"Me.Class == WoWClass.{CLASS_TO_WOWCLASS[cls]}")
        if race and race in RACE_TO_WOWRACE:
            conditions.append(f"Me.Race == WoWRace.{RACE_TO_WOWRACE[race]}")
        
        if not conditions:
            return None
        
        inner = " &amp;&amp; ".join(f"({c})" for c in conditions)
        if negate:
            return f"!({inner})"
        return inner
    
    def _generate_quest_overrides(self, steps: List[QuestStep]) -> List[str]:
        """Generate <Quest> override sections for CollectItem objectives with world coordinates.
        
        Uses gameobject_spawns.json (exported from server DB) for accurate world coordinates.
        Falls back to Zygor zone coordinates if server spawns not available.
        """
        quest_overrides: Dict[int, Dict] = {}  # quest_id -> {name, objectives: [...]}
        
        for step in steps:
            if step.step_type == "CollectItem" and step.quest_id and step.item_id:
                if step.quest_id not in quest_overrides:
                    quest_overrides[step.quest_id] = {
                        'name': step.quest_name or "Unknown",
                        'objectives': []
                    }
                
                # Check if we already have this item objective
                existing = None
                for obj in quest_overrides[step.quest_id]['objectives']:
                    if obj['item_id'] == step.item_id:
                        existing = obj
                        break
                
                if existing:
                    continue  # Already processed this item
                
                # Find game_object_id: from step (click##ID) or from Questie item->object mapping
                game_object_id = step.game_object_id
                game_object_name = step.game_object_name
                
                if not game_object_id and self.parser and step.item_id in self.parser.item_to_objects:
                    obj_ids = self.parser.item_to_objects[step.item_id]
                    if obj_ids:
                        game_object_id = obj_ids[0]  # Use first object
                        game_object_name = self.parser.gameobject_names.get(game_object_id, game_object_name)
                
                # Get world coordinates from server DB spawns
                hotspots = []
                if game_object_id and self.parser and game_object_id in self.parser.gameobject_spawns:
                    spawns = self.parser.gameobject_spawns[game_object_id]
                    
                    # Sort spawns by X then Y for better geographic distribution
                    # This ensures hotspots are spread across the zone
                    sorted_spawns = sorted(spawns, key=lambda s: (s['x'], s['y']))
                    
                    # Take up to 25 spawns for good coverage
                    for spawn in sorted_spawns[:25]:
                        hotspots.append({
                            'x': round(spawn['x'], 2),
                            'y': round(spawn['y'], 2),
                            'z': round(spawn['z'], 2)
                        })
                
                # If no server spawns, skip this objective (Zygor coords are zone % and useless)
                if not hotspots:
                    continue
                
                quest_overrides[step.quest_id]['objectives'].append({
                    'item_id': step.item_id,
                    'item_name': step.item_name,
                    'count': step.collect_count or 1,
                    'game_object_id': game_object_id,
                    'game_object_name': game_object_name or (self.parser.gameobject_names.get(game_object_id, "Unknown") if self.parser else "Unknown"),
                    'hotspots': hotspots
                })
        
        # Generate XML
        xml_lines = []
        for quest_id, quest_data in quest_overrides.items():
            if not quest_data['objectives']:
                continue  # No valid objectives with hotspots
            
            xml_lines.append(f'  <Quest Id="{quest_id}" Name="{self._escape_xml(quest_data["name"])}">')
            for obj in quest_data['objectives']:
                xml_lines.append(f'    <Objective Type="CollectItem" ItemId="{obj["item_id"]}" CollectCount="{obj["count"]}">')
                if obj['game_object_id']:
                    xml_lines.append(f'      <CollectFrom>')
                    xml_lines.append(f'        <GameObject Name="{self._escape_xml(obj["game_object_name"])}" Id="{obj["game_object_id"]}" />')
                    xml_lines.append(f'      </CollectFrom>')
                xml_lines.append(f'      <Hotspots>')
                for hs in obj['hotspots']:
                    xml_lines.append(f'        <Hotspot X="{hs["x"]}" Y="{hs["y"]}" Z="{hs["z"]}" />')
                xml_lines.append(f'      </Hotspots>')
                xml_lines.append(f'    </Objective>')
            xml_lines.append(f'  </Quest>')
        
        return xml_lines
    
    def generate_xml(self, guide: Guide, race_filter: Optional[str] = None) -> str:
        """Generate CopilotBuddy XML profile from Guide"""
        
        # Filter steps by race if specified
        steps = guide.steps
        if race_filter:
            steps = [s for s in steps if not s.race_filter or s.race_filter == race_filter]
        
        xml_lines = [
            '<?xml version="1.0" encoding="utf-8"?>',
            '<HBProfile>',
            f'  <Name>{self._escape_xml(guide.name)}</Name>',
            '',
            f'  <MinLevel>{guide.min_level}</MinLevel>',
            f'  <MaxLevel>{guide.max_level}</MaxLevel>',
            '',
            '  <MinDurability>0.2</MinDurability>',
            '  <MinFreeBagSlots>2</MinFreeBagSlots>',
            '',
        ]
        
        # Generate Quest overrides for CollectItem with hotspots
        quest_overrides = self._generate_quest_overrides(steps)
        if quest_overrides:
            xml_lines.append('  <!-- Quest Overrides -->')
            xml_lines.extend(quest_overrides)
            xml_lines.append('')
        
        xml_lines.extend([
            '  <QuestOrder>',
            f'    <CustomBehavior File="Message" Text="{self._escape_xml(guide.name)}" LogColor="Green" />',
        ])
        
        # Track which quests we've seen to avoid duplicates
        seen_pickups = set()
        seen_turnins = set()
        
        # Group consecutive steps with same class/race filter
        current_if_condition = None
        if_block_open = False
        
        for step in steps:
            step_condition = self._build_condition(step.race_filter, step.class_filter, step.negate_filter)
            
            # Handle If block transitions
            if step_condition != current_if_condition:
                # Close previous If block if open
                if if_block_open:
                    xml_lines.append('    </If>')
                    if_block_open = False
                
                # Open new If block if needed
                if step_condition:
                    xml_lines.append(f'    <If Condition="{step_condition}">')
                    if_block_open = True
                
                current_if_condition = step_condition
            
            # Generate step XML
            xml_step = self._step_to_xml(step, seen_pickups, seen_turnins, if_block_open)
            if xml_step:
                xml_lines.append(xml_step)
        
        # Close any remaining If block
        if if_block_open:
            xml_lines.append('    </If>')
        
        # Add message and chain to next profile if defined
        xml_lines.append(f'    <CustomBehavior File="Message" Text="{self._escape_xml(guide.name)} Complete" LogColor="Orange" />')
        
        if guide.next_guide:
            # Convert guide name to filename format
            next_filename = re.sub(r'[\\/*?:"<>|]', '_', guide.next_guide)
            next_filename = next_filename.replace(' ', '_')
            xml_lines.append(f'    <CustomBehavior File="LoadProfile" ProfileName="{self._escape_xml(next_filename)}" />')
        
        xml_lines.extend([
            '  </QuestOrder>',
            '</HBProfile>',
        ])
        
        return '\n'.join(xml_lines)
    
    def _step_to_xml(self, step: QuestStep, seen_pickups: set, seen_turnins: set, in_if_block: bool = False) -> Optional[str]:
        """Convert a QuestStep to XML element"""
        indent = '      ' if in_if_block else '    '
        
        if step.step_type == "PickUp":
            if step.quest_id in seen_pickups:
                return None
            seen_pickups.add(step.quest_id)
            
            attrs = [
                f'QuestName="{self._escape_xml(step.quest_name or "Unknown")}"',
                f'QuestId="{step.quest_id}"',
            ]
            if step.npc_name:
                attrs.append(f'GiverName="{self._escape_xml(step.npc_name)}"')
            if step.npc_id:
                attrs.append(f'GiverId="{step.npc_id}"')
            
            return f'{indent}<PickUp {" ".join(attrs)} />'
        
        elif step.step_type == "TurnIn":
            if step.quest_id in seen_turnins:
                return None
            seen_turnins.add(step.quest_id)
            
            attrs = [
                f'QuestName="{self._escape_xml(step.quest_name or "Unknown")}"',
                f'QuestId="{step.quest_id}"',
            ]
            if step.npc_name:
                attrs.append(f'TurnInName="{self._escape_xml(step.npc_name)}"')
            if step.npc_id:
                attrs.append(f'TurnInId="{step.npc_id}"')
            
            return f'{indent}<TurnIn {" ".join(attrs)} />'
        
        elif step.step_type == "Objective":
            if step.kill_count and step.mob_id:
                attrs = [
                    f'QuestName="{self._escape_xml(step.quest_name or "Unknown")}"',
                ]
                if step.quest_id:
                    attrs.append(f'QuestId="{step.quest_id}"')
                attrs.extend([
                    'Type="KillMob"',
                    f'MobId="{step.mob_id}"',
                ])
                if step.mob_name:
                    attrs.append(f'MobName="{self._escape_xml(step.mob_name)}"')
                attrs.append(f'KillCount="{step.kill_count}"')
                return f'{indent}<Objective {" ".join(attrs)} />'
        
        elif step.step_type == "CollectItem":
            if step.collect_count and step.item_id:
                attrs = [
                    f'QuestName="{self._escape_xml(step.quest_name or "Unknown")}"',
                ]
                if step.quest_id:
                    attrs.append(f'QuestId="{step.quest_id}"')
                attrs.extend([
                    'Type="CollectItem"',
                    f'ItemId="{step.item_id}"',
                ])
                if step.item_name:
                    attrs.append(f'ItemName="{self._escape_xml(step.item_name)}"')
                attrs.append(f'CollectCount="{step.collect_count}"')
                return f'{indent}<Objective {" ".join(attrs)} />'
        
        return None
    
    def _escape_xml(self, text: str) -> str:
        """Escape special XML characters"""
        if not text:
            return ""
        return (text
            .replace('&', '&amp;')
            .replace('<', '&lt;')
            .replace('>', '&gt;')
            .replace('"', '&quot;')
            .replace("'", '&apos;'))

# ============================================================================
# Main
# ============================================================================

def main():
    import argparse
    
    parser = argparse.ArgumentParser(description='Convert Zygor guides to CopilotBuddy profiles')
    parser.add_argument('zygor_file', help='Path to Zygor guide .lua file')
    parser.add_argument('-o', '--output', help='Output directory for XML profiles', default='.')
    parser.add_argument('-q', '--questie', help='Path to Questie-335 addon folder')
    parser.add_argument('-r', '--race', help='Filter by race (e.g., Orc, Troll)')
    parser.add_argument('--list', action='store_true', help='List guides in file without converting')
    
    args = parser.parse_args()
    
    # Initialize parser
    zygor_parser = ZygorParser(questie_db_path=args.questie)
    
    # Read Zygor file
    with open(args.zygor_file, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()
    
    # Parse guides
    guides = zygor_parser.parse_guide(content)
    
    if args.list:
        print(f"Found {len(guides)} guides:")
        for i, guide in enumerate(guides):
            print(f"  {i+1}. {guide.name} ({len(guide.steps)} steps)")
        return
    
    # Enrich with Questie if available
    if args.questie:
        for guide in guides:
            zygor_parser.enrich_with_questie(guide)
    
    # Generate profiles
    generator = ProfileGenerator(zygor_parser)
    output_dir = Path(args.output)
    output_dir.mkdir(parents=True, exist_ok=True)
    
    for guide in guides:
        # Create safe filename
        safe_name = re.sub(r'[\\/*?:"<>|]', '_', guide.name)
        safe_name = safe_name.replace(' ', '_')
        output_file = output_dir / f"{safe_name}.xml"
        
        xml_content = generator.generate_xml(guide, race_filter=args.race)
        
        with open(output_file, 'w', encoding='utf-8') as f:
            f.write(xml_content)
        
        print(f"Generated: {output_file}")
    
    print(f"\nGenerated {len(guides)} profiles!")

if __name__ == '__main__':
    main()
