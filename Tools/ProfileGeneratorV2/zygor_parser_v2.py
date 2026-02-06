#!/usr/bin/env python3
"""
Zygor Guide to CopilotBuddy Profile Converter V2
INTELLIGENT PARSER - Wraps CustomBehaviors in proper If/While conditions

Key improvement over V1:
- Every UseItemOn/InteractWith is wrapped in While/If with HasQuest && !IsQuestCompleted
- Queries Questie for prereqs (chained quests get If condition on PickUp)
- Detects auto-complete quests (no TurnIn NPC) → CompleteLogQuest pattern
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
    step_type: str  # PickUp, TurnIn, Objective, UseItem, InteractWith, RunTo, etc.
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
    negate_filter: bool = False
    objective_index: Optional[int] = None
    game_object_id: Optional[int] = None
    game_object_name: Optional[str] = None
    # CustomBehavior fields
    use_item_id: Optional[int] = None
    use_item_name: Optional[str] = None
    target_mob_id: Optional[int] = None
    target_mob_name: Optional[str] = None
    num_of_times: int = 1
    gossip_options: Optional[str] = None
    wait_time: int = 3000  # Default wait after action
    # SetHearthstone fields
    hearthstone_inn_name: Optional[str] = None
    hearthstone_npc_id: Optional[int] = None
    hearthstone_zone_id: Optional[int] = None
    # Level requirement
    required_level: Optional[int] = None
    # Grind mob for GrindTo (tracked from previous kill step)
    grind_mob_id: Optional[int] = None
    grind_mob_name: Optional[str] = None
    grind_faction_id: Optional[int] = None
    grind_min_level: Optional[int] = None
    grind_max_level: Optional[int] = None

@dataclass
class QuestInfo:
    """Questie quest data with objectives and chain info"""
    quest_id: int
    name: str
    giver_npc_id: Optional[int] = None
    turnin_npc_id: Optional[int] = None
    turnin_object_id: Optional[int] = None  # For quests that finish at GameObjects
    turnin_object_name: Optional[str] = None  # GameObject name for TurnIn
    mob_objectives: List[int] = field(default_factory=list)
    item_objectives: List[int] = field(default_factory=list)
    objectives_text: List[str] = field(default_factory=list)
    # Chain info for If conditions
    pre_quest_single: Optional[int] = None  # Must complete this first
    pre_quest_group: List[int] = field(default_factory=list)  # Must complete any/all of these

@dataclass
class Guide:
    """Represents a parsed Zygor guide"""
    name: str
    min_level: int = 1
    max_level: int = 60
    faction: str = "Horde"
    race: Optional[str] = None
    steps: List[QuestStep] = field(default_factory=list)
    next_guide: Optional[str] = None

# ============================================================================
# Zone Mappings (Zygor zone names to MapId)
# ============================================================================

ZONE_TO_MAPID = {
    # Kalimdor
    "Durotar": 14, "Valley of Trials": 363, "Mulgore": 215, "Camp Narache": 221,
    "Tirisfal Glades": 85, "Deathknell": 20, "The Barrens": 17, "Orgrimmar": 1637,
    "Thunder Bluff": 1638, "Undercity": 1497, "Ashenvale": 331, "Stonetalon Mountains": 406,
    "Thousand Needles": 400, "Desolace": 405, "Dustwallow Marsh": 15, "Feralas": 357,
    "Tanaris": 440, "Un'Goro Crater": 490, "Silithus": 1377, "Felwood": 361,
    "Winterspring": 618, "Azshara": 16, "Moonglade": 493,
    
    # Eastern Kingdoms  
    "Elwynn Forest": 12, "Northshire": 9, "Westfall": 40, "Redridge Mountains": 44,
    "Duskwood": 10, "Stranglethorn Vale": 33, "Swamp of Sorrows": 8, "Blasted Lands": 4,
    "Burning Steppes": 46, "Searing Gorge": 51, "Badlands": 3, "Loch Modan": 38,
    "Wetlands": 11, "Arathi Highlands": 45, "Hillsbrad Foothills": 267, "Alterac Mountains": 36,
    "Silverpine Forest": 130, "Western Plaguelands": 28, "Eastern Plaguelands": 139,
    "The Hinterlands": 47, "Dun Morogh": 1, "Ironforge": 1537, "Stormwind City": 1519,
    
    # Blood Elf / Draenei
    "Eversong Woods": 3430, "Sunstrider Isle": 3430, "Ghostlands": 3433, "Silvermoon City": 3487,
    
    # Outland
    "Hellfire Peninsula": 3483, "Zangarmarsh": 3521, "Terokkar Forest": 3519,
    "Nagrand": 3518, "Blade's Edge Mountains": 3522, "Netherstorm": 3523,
    "Shadowmoon Valley": 3520, "Shattrath City": 3703,
    
    # Northrend
    "Borean Tundra": 3537, "Howling Fjord": 495, "Dragonblight": 65, "Grizzly Hills": 394,
    "Zul'Drak": 66, "Sholazar Basin": 3711, "The Storm Peaks": 67, "Icecrown": 210,
    "Crystalsong Forest": 2817, "Dalaran": 4395,
    
    # Death Knight
    "Plaguelands: The Scarlet Enclave": 139,
}

# ============================================================================
# Parser Regex Patterns
# ============================================================================

PATTERN_TALK = re.compile(r'talk\s+([^#]+)##(\d+)')
PATTERN_ACCEPT = re.compile(r'accept\s+([^#]+)##(\d+)')
PATTERN_TURNIN = re.compile(r'turnin\s+([^#]+)##(\d+)')
PATTERN_KILL = re.compile(r'kill\s+(?:(\d+)\s+)?([^#]+)##(\d+)')
PATTERN_COLLECT = re.compile(r'collect\s+(?:(\d+)\s+)?([^#]+)##(\d+)')
PATTERN_GOTO = re.compile(r'\|goto\s+(?:([^/\d][^/]*?)(?:/\d+)?\s+)?([\d.]+)\s*,\s*([\d.]+)')
PATTERN_QUEST_OBJ = re.compile(r'\|q\s+(\d+)(?:/(\d+))?')  # objIndex is optional
PATTERN_ONLY_IF = re.compile(r'\|only\s+(?:if\s+)?(.+)$')
PATTERN_CLICK = re.compile(r'click\s+([^#\n]+?)(?:##(\d+))?(?:\s|$|\|)')
PATTERN_USE = re.compile(r'use\s+([^#]+)##(\d+)')
PATTERN_USE_TARGET = re.compile(r'\|use\s+([^#]+)##(\d+)')
# Pattern for Zygor #number# objective format: "Slay #10# Scarlet Soldiers"
PATTERN_ZYGOR_OBJ_COUNT = re.compile(r'#(\d+)#\s+([^\|]+)')
# Pattern for home (SetHearthstone): home Razor Hill |goto 51.52,41.65
PATTERN_HOME = re.compile(r'^home\s+(.+?)(?:\s*\|goto|$)', re.IGNORECASE)
# Pattern for level requirement: Reach Level 6 |ding 6
PATTERN_DING = re.compile(r'\|ding\s+(\d+)')
# Pattern for using hearthstone: use the Hearthstone##6948, Hearth to Camp Winterhoof
PATTERN_USE_HEARTH = re.compile(r'use\s+(?:the\s+)?Hearthstone##6948', re.IGNORECASE)
PATTERN_HEARTH_TO = re.compile(r'Hearth\s+to\s+(.+?)(?:\s*\||$)', re.IGNORECASE)

RACE_KEYWORDS = ['Orc', 'Troll', 'Tauren', 'Undead', 'Scourge', 'BloodElf', 'Goblin',
                 'Human', 'Dwarf', 'NightElf', 'Gnome', 'Draenei', 'Worgen']
CLASS_KEYWORDS = ['Warrior', 'Paladin', 'Hunter', 'Rogue', 'Priest', 
                  'Shaman', 'Mage', 'Warlock', 'Druid', 'DeathKnight']

# ============================================================================
# Parser Class
# ============================================================================

class ZygorParser:
    def __init__(self, questie_db_path: Optional[str] = None, spawns_json_path: Optional[str] = None):
        self.questie_npcs: Dict[int, dict] = {}
        self.questie_quests: Dict[int, QuestInfo] = {}
        self.questie_items: Dict[int, str] = {}
        
        self.mob_to_quests: Dict[int, Set[int]] = {}
        self.item_to_quests: Dict[int, Set[int]] = {}
        self.item_to_objects: Dict[int, List[int]] = {}
        
        self.gameobject_spawns: Dict[int, List[Dict]] = {}
        self.gameobject_names: Dict[int, str] = {}
        
        # Innkeeper mapping: inn_name -> {npc_id, zone_id}
        self.innkeeper_mapping: Dict[str, Dict] = {}
        self._load_innkeeper_mapping()
        
        if questie_db_path:
            self._load_questie_db(questie_db_path)
        
        if spawns_json_path:
            self._load_spawns_json(spawns_json_path)
        else:
            default_path = Path(__file__).parent / "gameobject_spawns.json"
            if default_path.exists():
                self._load_spawns_json(str(default_path))
    
    def _load_innkeeper_mapping(self):
        """Load innkeeper NPC ID and zone ID mapping from JSON file"""
        # Try V2 format first (with zone IDs), fall back to V1
        mapping_file = Path(__file__).parent / "innkeeper_mapping_v2.json"
        if not mapping_file.exists():
            mapping_file = Path(__file__).parent / "innkeeper_mapping.json"
        
        if mapping_file.exists():
            try:
                with open(mapping_file, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                # Filter out comment keys and convert to standard format
                for k, v in data.items():
                    if k.startswith('_'):
                        continue
                    # Support both formats: int (V1) or {npc_id, zone_id} (V2)
                    if isinstance(v, dict):
                        self.innkeeper_mapping[k] = v
                    else:
                        self.innkeeper_mapping[k] = {'npc_id': v, 'zone_id': 0}
                print(f"  Loaded {len(self.innkeeper_mapping)} innkeeper mappings")
            except Exception as e:
                print(f"  Warning: Could not load innkeeper mapping: {e}")
    
    def get_innkeeper_info(self, inn_name: str) -> Optional[Dict]:
        """Get innkeeper NPC ID and zone ID for an inn name"""
        # Direct match
        if inn_name in self.innkeeper_mapping:
            return self.innkeeper_mapping[inn_name]
        # Try without apostrophes (Grom'arsh -> Gromarsh)
        clean_name = inn_name.replace("'", "")
        for key, info in self.innkeeper_mapping.items():
            if key.replace("'", "") == clean_name:
                return info
        return None
    
    def get_innkeeper_id(self, inn_name: str) -> Optional[int]:
        """Get innkeeper NPC ID for an inn name (backwards compat)"""
        info = self.get_innkeeper_info(inn_name)
        return info['npc_id'] if info else None
    
    def _load_questie_db(self, path: str):
        """Load Questie database for NPC coords, quest objectives, and chain info"""
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
        content = filepath.read_text(encoding='utf-8', errors='ignore')
        # Enhanced pattern to extract more NPC data
        # Format: [id] = {'name',minLvlHP,maxLvlHP,minLvl,maxLvl,rank,spawns,waypoints,zoneID,starts,ends,factionID,...}
        npc_pattern = re.compile(r"^\[(\d+)\]\s*=\s*\{'([^']+)',(\d+),(\d+),(\d+),(\d+),(\d+),")
        # Coordinate pattern: matches {[zoneId]={{x,y}}} format - note square brackets around zoneId
        coord_pattern = re.compile(r'\{\[(\d+)\]\s*=\s*\{\{([\d.]+),([\d.]+)\}')
        # Pattern to find factionID: number followed by friendlyToFaction string (like "A", "H", "AH", or "")
        # Format: ...nil,nil,factionID,"A",... or ...{},factionID,"AH",...
        faction_pattern = re.compile(r',(\d+),"([AH]*)"(?:,|$)')
        
        for line in content.split('\n'):
            npc_match = npc_pattern.match(line)
            if npc_match:
                npc_id = int(npc_match.group(1))
                npc_name = npc_match.group(2)
                min_level = int(npc_match.group(5))
                max_level = int(npc_match.group(6))
                
                # Extract factionID - look for pattern: ,number,"letter(s)",
                faction_id = 0
                faction_match = faction_pattern.search(line)
                if faction_match:
                    faction_id = int(faction_match.group(1))
                
                coord_match = coord_pattern.search(line)
                if coord_match:
                    self.questie_npcs[npc_id] = {
                        'name': npc_name,
                        'zone': int(coord_match.group(1)),
                        'x': float(coord_match.group(2)),
                        'y': float(coord_match.group(3)),
                        'min_level': min_level,
                        'max_level': max_level,
                        'faction_id': faction_id
                    }
    
    def _parse_questie_quests(self, filepath: Path):
        """Parse quests with prereq detection for chain conditions"""
        content = filepath.read_text(encoding='utf-8', errors='ignore')
        quest_pattern = re.compile(r'^\[(\d+)\]\s*=\s*\{"([^"]+)"')
        
        for line in content.split('\n'):
            quest_match = quest_pattern.match(line)
            if not quest_match:
                continue
            
            quest_id = int(quest_match.group(1))
            quest_name = quest_match.group(2)
            quest_info = QuestInfo(quest_id=quest_id, name=quest_name)
            
            # Giver NPC (startedBy field = 2nd field after name)
            giver_match = re.search(r'^\[\d+\]\s*=\s*\{[^,]+,\{\{(\d+)\}', line)
            if giver_match:
                quest_info.giver_npc_id = int(giver_match.group(1))
            
            # TurnIn NPC/Object (finishedBy = 3rd field after name)
            # Format: [ID] = {"name",{startedBy},{finishedBy},...
            # finishedBy = {{npcIds},{objectIds}} 
            # Examples: 
            #   {nil,{190936}} = Object only (Plague Cauldron)
            #   {{28919}} = NPC only (Noth the Plaguebringer)
            #   {{28919},{190936}} = Both NPC and Object
            
            # Look for }},{ which marks end of startedBy and start of finishedBy  
            # Then extract what's between }},{ and the next },number
            finished_by_match = re.search(r'\}\},(\{.*?\}),\d', line)
            if finished_by_match:
                finished_by = finished_by_match.group(1)
                # Check for NPC turn-in: {{npcId}} at start
                npc_match = re.search(r'^\{\{(\d+)\}', finished_by)
                if npc_match:
                    quest_info.turnin_npc_id = int(npc_match.group(1))
                # Check for Object turn-in: {nil,{objId}} or after NPC },{{objId}}
                obj_match = re.search(r'(?:nil|\}),\{(\d+)\}', finished_by)
                if obj_match:
                    quest_info.turnin_object_id = int(obj_match.group(1))
            
            # Mob objectives
            mob_obj_match = re.search(r',nil,\{\{\{(\d+)', line)
            if mob_obj_match:
                mob_ids = re.findall(r'\{\{(\d+)[,\}]', line[mob_obj_match.start():])
                for mob_id_str in mob_ids[:5]:
                    mob_id = int(mob_id_str)
                    quest_info.mob_objectives.append(mob_id)
                    if mob_id not in self.mob_to_quests:
                        self.mob_to_quests[mob_id] = set()
                    self.mob_to_quests[mob_id].add(quest_id)
            
            # Item objectives
            item_obj_match = re.search(r'\{nil,nil,\{\{(\d+)', line)
            if item_obj_match:
                item_ids = re.findall(r'\{\{(\d+)[,\}]', line[item_obj_match.start():])
                for item_id_str in item_ids[:5]:
                    item_id = int(item_id_str)
                    quest_info.item_objectives.append(item_id)
                    if item_id not in self.item_to_quests:
                        self.item_to_quests[item_id] = set()
                    self.item_to_quests[item_id].add(quest_id)
            
            # Objectives text - found in field 8, after flags field
            # Format: ,flags,{"text1","","detailed objective text with numbers like slay 150"}
            # The text table starts with ,number,{"
            obj_table_start = re.search(r',\d+,\{"', line)
            if obj_table_start:
                # Find the matching closing }
                table_content_start = obj_table_start.end() - 1
                # Get all quoted strings from this table (limited scope)
                remaining = line[obj_table_start.end()-1:]
                # Find the closing } - count braces
                brace_count = 1
                i = 1  # skip first {
                while i < len(remaining) and brace_count > 0:
                    if remaining[i] == '{':
                        brace_count += 1
                    elif remaining[i] == '}':
                        brace_count -= 1
                    i += 1
                table_str = remaining[:i]
                obj_texts = re.findall(r'"([^"]{3,})"', table_str)
                quest_info.objectives_text = obj_texts[:5]
            
            # *** NEW: Parse preQuestSingle and preQuestGroup ***
            # Format: ...,preQuestSingle,...,preQuestGroup,...
            # Index 4 = preQuestSingle (can be nil or {id} or id)
            # Index 5 = preQuestGroup (can be nil or {id1,id2,...})
            prereq_single = re.search(r',(\d+),\{?\d*\}?,[^,]*,[^,]*,[^,]*,nil,\{"', line)
            if prereq_single:
                quest_info.pre_quest_single = int(prereq_single.group(1))
            
            # Try to extract preQuestGroup: ,{id1,id2},
            prereq_group_match = re.search(r',\{(\d+(?:,\d+)*)\},', line)
            if prereq_group_match:
                ids = prereq_group_match.group(1).split(',')
                quest_info.pre_quest_group = [int(x) for x in ids if x]
            
            self.questie_quests[quest_id] = quest_info
    
    def _parse_questie_items(self, filepath: Path):
        content = filepath.read_text(encoding='utf-8', errors='ignore')
        item_pattern = re.compile(r'^\[(\d+)\]\s*=\s*\{[\'"]([^\'"]+)[\'"],([^,]*),(\{[^}]*\}|nil)')
        
        for line in content.split('\n'):
            item_match = item_pattern.match(line)
            if item_match:
                item_id = int(item_match.group(1))
                item_name = item_match.group(2)
                object_drops = item_match.group(4)
                
                self.questie_items[item_id] = item_name
                
                if object_drops and object_drops != 'nil':
                    obj_ids = re.findall(r'(\d+)', object_drops)
                    if obj_ids:
                        self.item_to_objects[item_id] = [int(x) for x in obj_ids]
        
        if self.item_to_objects:
            print(f"  Built {len(self.item_to_objects)} item->object mappings")
    
    def _load_spawns_json(self, filepath: str):
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
    
    def has_prereq(self, quest_id: int) -> bool:
        """Check if quest has prerequisites (chain)"""
        if quest_id not in self.questie_quests:
            return False
        quest = self.questie_quests[quest_id]
        return bool(quest.pre_quest_single or quest.pre_quest_group)
    
    def get_turnin_type(self, quest_id: int) -> Tuple[str, Optional[int]]:
        """Get turn-in type: ('npc', npc_id), ('object', object_id), or ('auto', None)"""
        if quest_id not in self.questie_quests:
            return ('npc', None)  # Assume NPC if unknown
        quest = self.questie_quests[quest_id]
        if quest.turnin_npc_id:
            return ('npc', quest.turnin_npc_id)
        elif quest.turnin_object_id:
            return ('object', quest.turnin_object_id)
        else:
            return ('auto', None)
    
    def find_quest_for_mob(self, mob_id: int, active_quests: Set[int]) -> Optional[int]:
        if mob_id not in self.mob_to_quests:
            return None
        possible_quests = self.mob_to_quests[mob_id] & active_quests
        if possible_quests:
            return next(iter(possible_quests))
        return None
    
    def find_quest_for_item(self, item_id: int, active_quests: Set[int]) -> Optional[int]:
        if item_id not in self.item_to_quests:
            return None
        possible_quests = self.item_to_quests[item_id] & active_quests
        if possible_quests:
            return next(iter(possible_quests))
        return None
    
    def get_kill_count_from_questie(self, quest_id: int, mob_name: str) -> Optional[int]:
        if quest_id not in self.questie_quests:
            return None
        quest = self.questie_quests[quest_id]
        if not quest.objectives_text:
            return None
        
        mob_name_clean = mob_name.lower().strip()
        # Handle plurals - try both singular and simple -s plural
        mob_name_singular = mob_name_clean.rstrip('s')
        mob_name_plural = mob_name_clean if mob_name_clean.endswith('s') else mob_name_clean + 's'
        mob_variants = [mob_name_clean, mob_name_singular, mob_name_plural]
        
        for text in quest.objectives_text:
            text_lower = text.lower()
            for variant in mob_variants:
                if variant in text_lower and len(variant) > 3:  # Avoid false matches
                    patterns = [
                        rf'(?:kill|slay|defeat|destroy|burn|eliminate|free|dismember|infect)\s+(\d+)\s+{re.escape(variant)}',
                        rf'(\d+)\s+{re.escape(variant)}',
                    ]
                    for pattern in patterns:
                        match = re.search(pattern, text_lower)
                        if match:
                            return int(match.group(1))
        return None

    def parse_guide(self, content: str, guide_name: str = "Unknown") -> List[Guide]:
        guides = []
        guide_blocks = re.split(r'ZygorGuidesViewer:RegisterGuide\(', content)
        
        for block in guide_blocks[1:]:
            guide = self._parse_guide_block(block)
            if guide and guide.steps:
                guides.append(guide)
        
        return guides
    
    def _parse_guide_block(self, block: str) -> Optional[Guide]:
        name_match = re.match(r'"([^"]+)"', block)
        if not name_match:
            return None
        
        guide_name = name_match.group(1)
        # Clean up name - remove "Leveling Guides\\" prefix to shorten filenames
        if guide_name.startswith("Leveling Guides\\"):
            guide_name = guide_name[16:]  # len("Leveling Guides\\") = 16
        guide = Guide(name=guide_name)
        
        if 'Horde' in guide_name:
            guide.faction = 'Horde'
        elif 'Alliance' in guide_name:
            guide.faction = 'Alliance'
        
        level_match = re.search(r'level\s*<=\s*(\d+)', block)
        if level_match:
            guide.max_level = int(level_match.group(1))
        
        race_match = re.search(r"raceclass\('(\w+)'\)", block)
        if race_match:
            guide.race = race_match.group(1)
        
        next_match = re.search(r'next="([^"]+)"', block)
        if next_match:
            guide.next_guide = next_match.group(1)
        
        active_quests: Dict[int, str] = {}
        last_pickup_quest_id: Optional[int] = None  # Track most recent PickUp for fallback inference
        last_kill_mob_id: Optional[int] = None  # Track most recent kill mob for GrindTo
        last_kill_mob_name: Optional[str] = None  # Track most recent kill mob name
        
        step_blocks = re.split(r'\nstep\n', block)
        current_npc_id = None
        current_npc_name = None
        current_zone = None
        current_coords = None
        current_map_id = None
        current_game_object_id = None
        current_game_object_name = None
        
        for step_block in step_blocks:
            lines = step_block.strip().split('\n')
            
            def extract_filter(line_text):
                race, cls, negate = None, None, False
                only_match = PATTERN_ONLY_IF.search(line_text)
                if only_match:
                    condition = only_match.group(1).strip()
                    if condition.lower().startswith('not '):
                        negate = True
                        condition = condition[4:]
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
                
                line_race_filter = None
                line_class_filter = None
                line_negate_filter = False
                
                goto_match = PATTERN_GOTO.search(line)
                if goto_match:
                    zone_from_goto = goto_match.group(1)
                    if zone_from_goto:
                        current_zone = zone_from_goto.strip()
                        current_map_id = ZONE_TO_MAPID.get(current_zone)
                    current_coords = (float(goto_match.group(2)), float(goto_match.group(3)))
                
                line_race_filter, line_class_filter, line_negate_filter = extract_filter(line)
                
                if not line_race_filter and not line_class_filter and i + 1 < len(lines):
                    next_line = lines[i + 1].strip()
                    if next_line.startswith('|only'):
                        line_race_filter, line_class_filter, line_negate_filter = extract_filter(next_line)
                
                # Parse click → InteractWith
                click_match = PATTERN_CLICK.search(line)
                if click_match:
                    current_game_object_name = click_match.group(1).strip()
                    current_game_object_id = int(click_match.group(2)) if click_match.group(2) else None
                    
                    quest_obj_match = PATTERN_QUEST_OBJ.search(line)
                    quest_id = int(quest_obj_match.group(1)) if quest_obj_match else None
                    obj_index = int(quest_obj_match.group(2)) if quest_obj_match and quest_obj_match.group(2) else None
                    
                    # Look-ahead: if no |q on this line, search subsequent lines in same step block
                    if not quest_id:
                        for future_line in lines[i+1:]:
                            future_quest_match = PATTERN_QUEST_OBJ.search(future_line)
                            if future_quest_match:
                                quest_id = int(future_quest_match.group(1))
                                obj_index = int(future_quest_match.group(2)) if future_quest_match.group(2) else None
                                break
                    
                    # Infer quest_id if not explicit (and look-ahead failed)
                    if not quest_id and last_pickup_quest_id and last_pickup_quest_id in active_quests:
                        quest_id = last_pickup_quest_id
                    elif not quest_id and len(active_quests) == 1:
                        quest_id = next(iter(active_quests.keys()))
                    
                    quest_name = None
                    if quest_id:
                        quest_name = active_quests.get(quest_id)
                        if not quest_name and quest_id in self.questie_quests:
                            quest_name = self.questie_quests[quest_id].name
                    
                    num_times = 1
                    if quest_id and quest_id in self.questie_quests:
                        quest_info = self.questie_quests[quest_id]
                        for text in quest_info.objectives_text:
                            count_match = re.search(r'(\d+)\s+' + re.escape(current_game_object_name.lower()), text.lower())
                            if count_match:
                                num_times = int(count_match.group(1))
                                break
                    
                    if current_game_object_id:
                        step = QuestStep(
                            step_type="InteractWith",
                            quest_id=quest_id,
                            quest_name=quest_name,
                            game_object_id=current_game_object_id,
                            game_object_name=current_game_object_name,
                            num_of_times=num_times,
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
                
                # Parse use → UseItem
                use_match = PATTERN_USE.search(line)
                if use_match:
                    item_name = use_match.group(1).strip()
                    item_id = int(use_match.group(2))
                    
                    quest_obj_match = PATTERN_QUEST_OBJ.search(line)
                    quest_id = int(quest_obj_match.group(1)) if quest_obj_match else None
                    obj_index = int(quest_obj_match.group(2)) if quest_obj_match and quest_obj_match.group(2) else None
                    
                    # Look-ahead: if no |q on this line, search subsequent lines in same step block
                    if not quest_id:
                        for future_line in lines[i+1:]:
                            future_quest_match = PATTERN_QUEST_OBJ.search(future_line)
                            if future_quest_match:
                                quest_id = int(future_quest_match.group(1))
                                obj_index = int(future_quest_match.group(2)) if future_quest_match.group(2) else None
                                break
                    
                    # Infer quest_id if not explicit on line (and look-ahead failed)
                    if not quest_id:
                        # Try Questie item->quest mapping
                        if item_id in self.item_to_quests:
                            possible = self.item_to_quests[item_id] & set(active_quests.keys())
                            if len(possible) == 1:
                                quest_id = next(iter(possible))
                            elif len(possible) > 1 and last_pickup_quest_id in possible:
                                quest_id = last_pickup_quest_id
                        # Fallback: use last pickup quest if still active
                        if not quest_id and last_pickup_quest_id and last_pickup_quest_id in active_quests:
                            quest_id = last_pickup_quest_id
                        # Final fallback: use most recent active quest (if only one)
                        elif not quest_id and len(active_quests) == 1:
                            quest_id = next(iter(active_quests.keys()))
                    
                    quest_name = None
                    if quest_id:
                        quest_name = active_quests.get(quest_id)
                        if not quest_name and quest_id in self.questie_quests:
                            quest_name = self.questie_quests[quest_id].name
                    
                    target_mob_id = None
                    target_mob_name = None
                    use_target_match = PATTERN_USE_TARGET.search(line)
                    if use_target_match:
                        target_mob_name = use_target_match.group(1).strip()
                        target_mob_id = int(use_target_match.group(2))
                    
                    step = QuestStep(
                        step_type="UseItem",
                        quest_id=quest_id,
                        quest_name=quest_name,
                        use_item_id=item_id,
                        use_item_name=item_name,
                        target_mob_id=target_mob_id,
                        target_mob_name=target_mob_name,
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
                
                # Parse talk → InteractWith (if quest objective)
                talk_match = PATTERN_TALK.search(line)
                if talk_match:
                    current_npc_name = talk_match.group(1).strip()
                    current_npc_id = int(talk_match.group(2))
                    
                    quest_obj_match = PATTERN_QUEST_OBJ.search(line)
                    has_accept = PATTERN_ACCEPT.search(line)
                    has_turnin = PATTERN_TURNIN.search(line)
                    
                    if quest_obj_match and not has_accept and not has_turnin:
                        quest_id = int(quest_obj_match.group(1))
                        obj_index = int(quest_obj_match.group(2)) if quest_obj_match.group(2) else None
                        
                        quest_name = active_quests.get(quest_id)
                        if not quest_name and quest_id in self.questie_quests:
                            quest_name = self.questie_quests[quest_id].name
                        
                        step = QuestStep(
                            step_type="InteractWith",
                            quest_id=quest_id,
                            quest_name=quest_name,
                            npc_id=current_npc_id,
                            npc_name=current_npc_name,
                            gossip_options="1",
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
                
                # Parse accept quest
                accept_match = PATTERN_ACCEPT.search(line)
                if accept_match:
                    quest_name = accept_match.group(1).strip()
                    quest_id = int(accept_match.group(2))
                    active_quests[quest_id] = quest_name
                    last_pickup_quest_id = quest_id  # Track for fallback inference
                    
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
                    active_quests.pop(quest_id, None)
                    # Clear last_pickup if this was it
                    if last_pickup_quest_id == quest_id:
                        last_pickup_quest_id = None
                    
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
                    
                    quest_obj_match = PATTERN_QUEST_OBJ.search(line)
                    quest_id = int(quest_obj_match.group(1)) if quest_obj_match else None
                    obj_index = int(quest_obj_match.group(2)) if quest_obj_match and quest_obj_match.group(2) else None
                    
                    # Look-ahead: if no |q on this line, search subsequent lines in same step block
                    if not quest_id:
                        for future_line in lines[i+1:]:
                            future_quest_match = PATTERN_QUEST_OBJ.search(future_line)
                            if future_quest_match:
                                quest_id = int(future_quest_match.group(1))
                                obj_index = int(future_quest_match.group(2)) if future_quest_match.group(2) else None
                                break
                    
                    if not quest_id:
                        quest_id = self.find_quest_for_mob(mob_id, set(active_quests.keys()))
                    
                    # FALLBACK: Use last_pickup_quest_id if still no match and it's active
                    if not quest_id and last_pickup_quest_id and last_pickup_quest_id in active_quests:
                        quest_id = last_pickup_quest_id
                    # FALLBACK 2: If only one active quest, use it
                    elif not quest_id and len(active_quests) == 1:
                        quest_id = next(iter(active_quests.keys()))
                    
                    count = 1
                    if quest_id:
                        questie_count = self.get_kill_count_from_questie(quest_id, mob_name)
                        if questie_count:
                            count = questie_count
                        elif zygor_count:
                            count = zygor_count
                    elif zygor_count:
                        count = zygor_count
                    
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
                    # Track last kill mob for potential GrindTo
                    last_kill_mob_id = mob_id
                    last_kill_mob_name = mob_name
                
                # Parse collect objective
                collect_match = PATTERN_COLLECT.search(line)
                if collect_match:
                    count = int(collect_match.group(1)) if collect_match.group(1) else 1
                    item_name = collect_match.group(2).strip()
                    item_id = int(collect_match.group(3))
                    
                    quest_obj_match = PATTERN_QUEST_OBJ.search(line)
                    quest_id = int(quest_obj_match.group(1)) if quest_obj_match else None
                    obj_index = int(quest_obj_match.group(2)) if quest_obj_match and quest_obj_match.group(2) else None
                    
                    # Look-ahead: if no |q on this line, search subsequent lines in same step block
                    if not quest_id:
                        for future_line in lines[i+1:]:
                            future_quest_match = PATTERN_QUEST_OBJ.search(future_line)
                            if future_quest_match:
                                quest_id = int(future_quest_match.group(1))
                                obj_index = int(future_quest_match.group(2)) if future_quest_match.group(2) else None
                                break
                    
                    if not quest_id:
                        quest_id = self.find_quest_for_item(item_id, set(active_quests.keys()))
                    
                    # FALLBACK: Use last_pickup_quest_id if still no match and it's active
                    if not quest_id and last_pickup_quest_id and last_pickup_quest_id in active_quests:
                        quest_id = last_pickup_quest_id
                    # FALLBACK 2: If only one active quest, use it
                    elif not quest_id and len(active_quests) == 1:
                        quest_id = next(iter(active_quests.keys()))
                    
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
                    current_game_object_id = None
                    current_game_object_name = None
                
                # Parse home → SetHearthstone
                home_match = PATTERN_HOME.match(line)
                if home_match:
                    inn_name = home_match.group(1).strip()
                    # Try to get innkeeper NPC ID and zone from mapping
                    innkeeper_info = self.get_innkeeper_info(inn_name)
                    innkeeper_id = innkeeper_info['npc_id'] if innkeeper_info else None
                    zone_id = innkeeper_info.get('zone_id', 0) if innkeeper_info else 0
                    innkeeper_name = inn_name
                    
                    step = QuestStep(
                        step_type="SetHearthstone",
                        hearthstone_inn_name=inn_name,
                        hearthstone_npc_id=innkeeper_id,
                        hearthstone_zone_id=zone_id,
                        npc_name=innkeeper_name,
                        zone=current_zone,
                        map_id=current_map_id
                    )
                    if current_coords:
                        step.hotspots.append(Hotspot(x=current_coords[0], y=current_coords[1]))
                    guide.steps.append(step)
                
                # Parse "use the Hearthstone##6948" → UseHearthstone
                use_hearth_match = PATTERN_USE_HEARTH.search(line)
                if use_hearth_match:
                    # Also check for destination: "Hearth to Camp Winterhoof"
                    hearth_dest = None
                    for check_line in lines[i:min(i+3, len(lines))]:  # Check next few lines
                        hearth_to_match = PATTERN_HEARTH_TO.search(check_line)
                        if hearth_to_match:
                            hearth_dest = hearth_to_match.group(1).strip()
                            break
                    
                    step = QuestStep(
                        step_type="UseHearthstone",
                        hearthstone_inn_name=hearth_dest,  # Where we're hearthing to
                        zone=current_zone,
                        map_id=current_map_id
                    )
                    guide.steps.append(step)
                
                # Parse |ding → LevelRequirement (grind until level)
                ding_match = PATTERN_DING.search(line)
                if ding_match:
                    required_level = int(ding_match.group(1))
                    step = QuestStep(
                        step_type="LevelRequirement",
                        required_level=required_level,
                        zone=current_zone,
                        map_id=current_map_id,
                        grind_mob_id=last_kill_mob_id,  # Use last killed mob for grind area
                        grind_mob_name=last_kill_mob_name
                    )
                    if current_coords:
                        step.hotspots.append(Hotspot(x=current_coords[0], y=current_coords[1]))
                    guide.steps.append(step)
        
        return guide
    
    def enrich_with_questie(self, guide: Guide):
        """Enrich guide data with Questie database information"""
        for step in guide.steps:
            if step.npc_id and step.npc_id in self.questie_npcs:
                npc_data = self.questie_npcs[step.npc_id]
                if not step.hotspots:
                    step.hotspots.append(Hotspot(x=npc_data['x'], y=npc_data['y']))
                if not step.npc_name:
                    step.npc_name = npc_data['name']
            
            if step.mob_id and step.mob_id in self.questie_npcs:
                mob_data = self.questie_npcs[step.mob_id]
                if not step.hotspots:
                    step.hotspots.append(Hotspot(x=mob_data['x'], y=mob_data['y']))
                if not step.mob_name:
                    step.mob_name = mob_data['name']
            
            if step.quest_id and step.quest_id in self.questie_quests:
                quest_info = self.questie_quests[step.quest_id]
                if not step.quest_name:
                    step.quest_name = quest_info.name
                
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
            
            if step.item_id and step.item_id in self.questie_items:
                if not step.item_name:
                    step.item_name = self.questie_items[step.item_id]
            
            # Enrich LevelRequirement with grind mob data
            if step.step_type == "LevelRequirement" and step.grind_mob_id:
                if step.grind_mob_id in self.questie_npcs:
                    mob_data = self.questie_npcs[step.grind_mob_id]
                    # Add mob spawn location as hotspot for grinding
                    if not step.hotspots:
                        step.hotspots.append(Hotspot(x=mob_data['x'], y=mob_data['y']))
                    # Get mob name if not set
                    if not step.grind_mob_name:
                        step.grind_mob_name = mob_data.get('name', '')
                    # Get faction ID for HB to know what to attack
                    if not step.grind_faction_id:
                        step.grind_faction_id = mob_data.get('faction_id', 0)
                    # Get mob level range for targeting
                    if not step.grind_min_level:
                        step.grind_min_level = mob_data.get('min_level', 1)
                    if not step.grind_max_level:
                        step.grind_max_level = mob_data.get('max_level', 60)
    
    def resolve_unknown_quests(self, guide: Guide):
        """Post-processing: Resolve steps with no quest by looking at adjacent context"""
        steps = guide.steps
        
        for i, step in enumerate(steps):
            # Skip if already has quest info
            if step.quest_id and step.quest_name:
                continue
            
            # Only process objectives (kill/collect)
            if step.step_type not in ("Objective", "CollectItem"):
                continue
            
            # Look forward for a step with the same mob/item that has quest info
            for j in range(i + 1, min(i + 10, len(steps))):
                future_step = steps[j]
                if future_step.quest_id and future_step.quest_name:
                    # Check if related (same mob or same item)
                    if (step.mob_id and future_step.mob_id == step.mob_id) or \
                       (step.item_id and future_step.item_id == step.item_id):
                        step.quest_id = future_step.quest_id
                        step.quest_name = future_step.quest_name
                        break
                    # Check if future step is a collect that drops from our kill mob
                    if step.mob_id and future_step.item_id:
                        # This kill might be to get the drop - inherit quest
                        step.quest_id = future_step.quest_id
                        step.quest_name = future_step.quest_name
                        break
            
            # If still unknown, look backward for most recent PickUp
            if not step.quest_id:
                for j in range(i - 1, max(i - 20, -1), -1):
                    past_step = steps[j]
                    if past_step.step_type == "PickUp" and past_step.quest_id:
                        step.quest_id = past_step.quest_id
                        step.quest_name = past_step.quest_name
                        break


# ============================================================================
# XML Generator V2 - INTELLIGENT WITH CONDITIONS
# ============================================================================

RACE_TO_WOWRACE = {
    'Orc': 'Orc', 'Troll': 'Troll', 'Tauren': 'Tauren', 'Undead': 'Undead',
    'Scourge': 'Undead', 'BloodElf': 'BloodElf', 'Goblin': 'Goblin',
    'Human': 'Human', 'Dwarf': 'Dwarf', 'NightElf': 'NightElf', 'Gnome': 'Gnome',
    'Draenei': 'Draenei', 'Worgen': 'Worgen',
}

CLASS_TO_WOWCLASS = {
    'Warrior': 'Warrior', 'Paladin': 'Paladin', 'Hunter': 'Hunter',
    'Rogue': 'Rogue', 'Priest': 'Priest', 'Shaman': 'Shaman',
    'Mage': 'Mage', 'Warlock': 'Warlock', 'Druid': 'Druid',
    'DeathKnight': 'DeathKnight',
}


class ProfileGeneratorV2:
    """V2 Generator: Wraps CustomBehaviors in If/While conditions"""
    
    def __init__(self, parser: 'ZygorParser' = None):
        self.parser = parser
    
    def _escape_xml(self, text: str) -> str:
        if not text:
            return ""
        return (text
            .replace('&', '&amp;')
            .replace('<', '&lt;')
            .replace('>', '&gt;')
            .replace('"', '&quot;')
            .replace("'", '&apos;'))
    
    def _get_object_name(self, object_id: int) -> Optional[str]:
        """Get GameObject name from gameobject_spawns.json"""
        if not self.parser or not self.parser.gameobject_names:
            return None
        return self.parser.gameobject_names.get(object_id)
    
    def _build_race_class_condition(self, race: Optional[str], cls: Optional[str], negate: bool = False) -> Optional[str]:
        """Build XML condition for race/class filtering"""
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
    
    def _quest_condition(self, quest_id: int, completed: bool = False) -> str:
        """Generate quest condition: HasQuest(X) && !IsQuestCompleted(X) or HasQuest(X) && IsQuestCompleted(X)"""
        if completed:
            return f"((HasQuest({quest_id})) &amp;&amp; (IsQuestCompleted({quest_id})))"
        else:
            return f"((HasQuest({quest_id})) &amp;&amp; (!IsQuestCompleted({quest_id})))"
    
    def _pickup_condition(self, quest_id: int) -> str:
        """Generate PickUp condition for chained quests: !HasQuest(X) && !IsQuestCompleted(X)"""
        return f"((!HasQuest({quest_id})) &amp;&amp; (!IsQuestCompleted({quest_id})))"
    
    def generate_xml(self, guide: Guide, race_filter: Optional[str] = None) -> str:
        """Generate CopilotBuddy XML profile with intelligent If/While conditions"""
        
        steps = guide.steps
        if race_filter:
            steps = [s for s in steps if not s.race_filter or s.race_filter == race_filter]
        
        # Display name includes faction for user-facing messages
        display_name = f"{guide.faction}\\{guide.name}"
        
        xml_lines = [
            '<?xml version="1.0" encoding="utf-8"?>',
            '<HBProfile>',
            f'  <Name>{self._escape_xml(display_name)}</Name>',
            '',
            f'  <MinLevel>{guide.min_level}</MinLevel>',
            f'  <MaxLevel>{guide.max_level}</MaxLevel>',
            '',
            '  <MinDurability>0.2</MinDurability>',
            '  <MinFreeBagSlots>2</MinFreeBagSlots>',
            '',
        ]
        
        # Quest overrides for CollectItem
        quest_overrides = self._generate_quest_overrides(steps)
        if quest_overrides:
            xml_lines.append('  <!-- Quest Overrides -->')
            xml_lines.extend(quest_overrides)
            xml_lines.append('')
        
        xml_lines.extend([
            '  <QuestOrder>',
            f'    <CustomBehavior File="Message" Text="Leveling Guides\\{self._escape_xml(display_name)}" LogColor="Green" />',
        ])
        
        seen_pickups = set()
        seen_turnins = set()
        
        # Track open blocks for proper nesting
        current_race_class_condition = None
        race_class_block_open = False
        
        for step in steps:
            # Handle race/class If blocks (outer wrapper)
            step_rc_condition = self._build_race_class_condition(step.race_filter, step.class_filter, step.negate_filter)
            
            if step_rc_condition != current_race_class_condition:
                if race_class_block_open:
                    xml_lines.append('    </If>')
                    race_class_block_open = False
                
                if step_rc_condition:
                    xml_lines.append(f'    <If Condition="{step_rc_condition}">')
                    race_class_block_open = True
                
                current_race_class_condition = step_rc_condition
            
            # Base indentation
            indent = '      ' if race_class_block_open else '    '
            
            # Generate step XML with proper conditions
            step_xml = self._step_to_xml_v2(step, seen_pickups, seen_turnins, indent)
            if step_xml:
                xml_lines.extend(step_xml)
        
        # Close remaining race/class If block
        if race_class_block_open:
            xml_lines.append('    </If>')
        
        # End message and chain
        xml_lines.append(f'    <CustomBehavior File="Message" Text="Leveling Guides\\{self._escape_xml(display_name)} Complete" LogColor="Orange" />')
        
        if guide.next_guide:
            next_filename = re.sub(r'[\\/*?:"<>|]', '_', guide.next_guide)
            next_filename = next_filename.replace(' ', '_')
            xml_lines.append(f'    <CustomBehavior File="LoadProfile" ProfileName="{self._escape_xml(next_filename)}" />')
        
        xml_lines.extend([
            '  </QuestOrder>',
            '</HBProfile>',
        ])
        
        return '\n'.join(xml_lines)
    
    def _step_to_xml_v2(self, step: QuestStep, seen_pickups: set, seen_turnins: set, indent: str) -> List[str]:
        """Convert QuestStep to XML with proper If/While conditions
        
        CRITICAL RULES:
        - PickUp simple: direct <PickUp />
        - PickUp chained (has prereq): wrap in <If !HasQuest && !IsQuestCompleted>
        - UseItem, InteractWith: ALWAYS wrap in <While HasQuest && !IsQuestCompleted>
        - TurnIn simple: direct <TurnIn />
        - TurnIn auto-complete (no NPC): wrap in <While HasQuest && IsQuestCompleted> + CompleteLogQuest
        - Objective (Kill/Collect): direct with QuestId (bot skips automatically)
        """
        lines = []
        
        # ===== PICKUP =====
        if step.step_type == "PickUp":
            if step.quest_id in seen_pickups:
                return []
            seen_pickups.add(step.quest_id)
            
            attrs = [
                f'QuestName="{self._escape_xml(step.quest_name or "Unknown")}"',
                f'QuestId="{step.quest_id}"',
            ]
            if step.npc_name:
                attrs.append(f'GiverName="{self._escape_xml(step.npc_name)}"')
            if step.npc_id:
                attrs.append(f'GiverId="{step.npc_id}"')
            
            pickup_xml = f'<PickUp {" ".join(attrs)} />'
            
            # Check if chained quest (has prereqs)
            is_chained = self.parser and self.parser.has_prereq(step.quest_id)
            
            if is_chained:
                # Wrap in If condition for chained quests
                condition = self._pickup_condition(step.quest_id)
                lines.append(f'{indent}<If Condition="{condition}">')
                lines.append(f'{indent}  {pickup_xml}')
                lines.append(f'{indent}</If>')
            else:
                lines.append(f'{indent}{pickup_xml}')
        
        # ===== TURNIN =====
        elif step.step_type == "TurnIn":
            if step.quest_id in seen_turnins:
                return []
            seen_turnins.add(step.quest_id)
            
            # Determine turn-in type: NPC, Object, or auto-complete
            turnin_type, turnin_id = ('npc', step.npc_id)
            if self.parser:
                turnin_type, questie_id = self.parser.get_turnin_type(step.quest_id)
                if questie_id:
                    turnin_id = questie_id
            
            if turnin_type == 'npc' and turnin_id:
                # Normal TurnIn with NPC
                attrs = [
                    f'QuestName="{self._escape_xml(step.quest_name or "Unknown")}"',
                    f'QuestId="{step.quest_id}"',
                ]
                if step.npc_name:
                    attrs.append(f'TurnInName="{self._escape_xml(step.npc_name)}"')
                if turnin_id:
                    attrs.append(f'TurnInId="{turnin_id}"')
                lines.append(f'{indent}<TurnIn {" ".join(attrs)} />')
                
            elif turnin_type == 'object' and turnin_id:
                # TurnIn at GameObject (e.g., quest 12717 "Noth's Special Brew" → Plague Cauldron)
                object_name = self._get_object_name(turnin_id) or f"Object_{turnin_id}"
                attrs = [
                    f'QuestName="{self._escape_xml(step.quest_name or "Unknown")}"',
                    f'QuestId="{step.quest_id}"',
                    f'TurnInName="{self._escape_xml(object_name)}"',
                    'TurnInType="Object"',
                    f'TurnInId="{turnin_id}"',
                ]
                lines.append(f'{indent}<TurnIn {" ".join(attrs)} />')
                
            else:
                # True auto-complete quest (no NPC or Object, rare in WotLK)
                # Skip CompleteLogQuest - most quests have real turn-ins
                # Just comment it for review
                lines.append(f'{indent}<!-- Quest {step.quest_id} may auto-complete (no turn-in found) -->')
        
        # ===== OBJECTIVE (Kill/Collect) =====
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
                lines.append(f'{indent}<Objective {" ".join(attrs)} />')
        
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
                lines.append(f'{indent}<Objective {" ".join(attrs)} />')
        
        # ===== USEITEM - ALWAYS WRAP IN WHILE =====
        elif step.step_type == "UseItem":
            if step.use_item_id and step.quest_id:
                condition = self._quest_condition(step.quest_id, completed=False)
                
                lines.append(f'{indent}<While Condition="{condition}">')
                
                # Add RunTo if we have coordinates
                if step.hotspots:
                    hs = step.hotspots[0]
                    lines.append(f'{indent}  <RunTo X="{hs.x}" Y="{hs.y}" Z="0" />')
                
                # UseItemOn behavior
                attrs = ['File="UseItemOn"']
                attrs.append(f'QuestId="{step.quest_id}"')
                attrs.append(f'ItemId="{step.use_item_id}"')
                mob_id = step.target_mob_id or 0
                attrs.append(f'MobId="{mob_id}"')
                attrs.append('NumOfTimes="1"')
                attrs.append('CollectionDistance="30"')
                attrs.append('WaitTime="1500"')
                
                lines.append(f'{indent}  <CustomBehavior {" ".join(attrs)} />')
                lines.append(f'{indent}  <CustomBehavior File="WaitTimer" WaitTime="3000" />')
                lines.append(f'{indent}</While>')
            
            elif step.use_item_id:
                # UseItem without quest (rare case) - still wrap but simpler
                attrs = ['File="UseItemOn"']
                attrs.append(f'ItemId="{step.use_item_id}"')
                mob_id = step.target_mob_id or 0
                attrs.append(f'MobId="{mob_id}"')
                attrs.append('NumOfTimes="1"')
                lines.append(f'{indent}<CustomBehavior {" ".join(attrs)} />')
        
        # ===== INTERACTWITH - ALWAYS WRAP IN IF OR WHILE =====
        elif step.step_type == "InteractWith":
            target_id = step.npc_id or step.game_object_id
            if target_id and step.quest_id:
                condition = self._quest_condition(step.quest_id, completed=False)
                
                # Use While if multiple interactions needed, If otherwise
                needs_repeat = step.num_of_times > 1
                wrapper = "While" if needs_repeat else "If"
                
                lines.append(f'{indent}<{wrapper} Condition="{condition}">')
                
                # InteractWith behavior
                attrs = ['File="InteractWith"']
                attrs.append(f'QuestId="{step.quest_id}"')
                attrs.append(f'MobId="{target_id}"')
                
                if step.num_of_times > 1:
                    attrs.append(f'NumOfTimes="{step.num_of_times}"')
                else:
                    attrs.append('NumOfTimes="1"')
                
                if step.gossip_options:
                    attrs.append(f'GossipOptions="{step.gossip_options}"')
                
                attrs.append('CollectionDistance="100"')
                attrs.append('WaitTime="3000"')
                
                lines.append(f'{indent}  <CustomBehavior {" ".join(attrs)} />')
                lines.append(f'{indent}  <CustomBehavior File="WaitTimer" WaitTime="2000" />')
                lines.append(f'{indent}</{wrapper}>')
            
            elif target_id:
                # InteractWith without quest - just generate without condition (rare)
                attrs = ['File="InteractWith"']
                attrs.append(f'MobId="{target_id}"')
                attrs.append('NumOfTimes="1"')
                if step.gossip_options:
                    attrs.append(f'GossipOptions="{step.gossip_options}"')
                lines.append(f'{indent}<CustomBehavior {" ".join(attrs)} />')
        
        # ===== SETHEARTHSTONE - Set home inn =====
        elif step.step_type == "SetHearthstone":
            inn_name = step.hearthstone_inn_name or "Inn"
            if step.hearthstone_npc_id:
                # Use SetHearthstone CustomBehavior with NPC ID and AreaId
                # AreaId makes the QB auto-skip if hearth already set in that zone
                attrs = ['File="SetHearthstone"']
                attrs.append(f'MobId="{step.hearthstone_npc_id}"')
                if step.hearthstone_zone_id:
                    attrs.append(f'AreaId="{step.hearthstone_zone_id}"')
                if step.hotspots:
                    hs = step.hotspots[0]
                    attrs.append(f'X="{hs.x}"')
                    attrs.append(f'Y="{hs.y}"')
                    attrs.append(f'Z="0"')
                lines.append(f'{indent}<CustomBehavior {" ".join(attrs)} /> <!-- Set hearth: {self._escape_xml(inn_name)} -->')
            else:
                # Comment only - manual review needed
                lines.append(f'{indent}<!-- TODO: Set hearthstone at {self._escape_xml(inn_name)} (innkeeper not found) -->')
        
        # ===== USEHEARTHSTONE - Use hearthstone item =====
        elif step.step_type == "UseHearthstone":
            dest_comment = f" to {self._escape_xml(step.hearthstone_inn_name)}" if step.hearthstone_inn_name else ""
            # Hearthstone item ID is always 6948
            lines.append(f'{indent}<!-- Use Hearthstone{dest_comment} -->')
            lines.append(f'{indent}<CustomBehavior File="UseItem" ItemId="6948" />')
            # Wait for cast and loading screen
            lines.append(f'{indent}<CustomBehavior File="WaitTimer" WaitTime="12000" />')
        
        # ===== LEVELREQUIREMENT - Grind to level =====
        elif step.step_type == "LevelRequirement":
            if step.required_level:
                level = step.required_level
                mob_comment = f" (grind {self._escape_xml(step.grind_mob_name)})" if step.grind_mob_name else ""
                lines.append(f'{indent}<If Condition="Me.Level &lt; {level}">')
                
                # Generate SetGrindArea if we have hotspots from the grind mob
                if step.hotspots:
                    lines.append(f'{indent}  <SetGrindArea>')
                    lines.append(f'{indent}    <GrindArea>')
                    
                    # Add name for the grind area
                    grind_name = f"Grind to Level {level}"
                    if step.grind_mob_name:
                        grind_name += f" - {self._escape_xml(step.grind_mob_name)}"
                    lines.append(f'{indent}      <Name>{grind_name}</Name>')
                    
                    # Add faction for HB to know what to attack
                    if step.grind_faction_id:
                        lines.append(f'{indent}      <Factions>{step.grind_faction_id}</Factions>')
                    
                    # Add target level range
                    if step.grind_min_level:
                        lines.append(f'{indent}      <TargetMinLevel>{step.grind_min_level}</TargetMinLevel>')
                    if step.grind_max_level:
                        lines.append(f'{indent}      <TargetMaxLevel>{step.grind_max_level}</TargetMaxLevel>')
                    
                    lines.append(f'{indent}      <Hotspots>')
                    for hotspot in step.hotspots:
                        z = hotspot.z if hotspot.z else 0
                        lines.append(f'{indent}        <Hotspot X="{hotspot.x:.2f}" Y="{hotspot.y:.2f}" Z="{z:.2f}" />')
                    lines.append(f'{indent}      </Hotspots>')
                    lines.append(f'{indent}    </GrindArea>')
                    lines.append(f'{indent}  </SetGrindArea>')
                
                # GrindTo is HB's built-in tag that grinds nearby mobs until level reached
                lines.append(f'{indent}  <GrindTo Level="{level}" />{" <!-- " + mob_comment + " -->" if mob_comment else ""}')
                lines.append(f'{indent}</If>')
        
        return lines
    
    def _generate_quest_overrides(self, steps: List[QuestStep]) -> List[str]:
        """Generate Quest override sections for CollectItem objectives with hotspots"""
        quest_overrides: Dict[int, Dict] = {}
        
        for step in steps:
            if step.step_type == "CollectItem" and step.quest_id and step.item_id:
                if step.quest_id not in quest_overrides:
                    quest_overrides[step.quest_id] = {
                        'name': step.quest_name or "Unknown",
                        'objectives': []
                    }
                
                existing = None
                for obj in quest_overrides[step.quest_id]['objectives']:
                    if obj['item_id'] == step.item_id:
                        existing = obj
                        break
                
                if existing:
                    continue
                
                game_object_id = step.game_object_id
                game_object_name = step.game_object_name
                
                if not game_object_id and self.parser and step.item_id in self.parser.item_to_objects:
                    obj_ids = self.parser.item_to_objects[step.item_id]
                    if obj_ids:
                        game_object_id = obj_ids[0]
                        game_object_name = self.parser.gameobject_names.get(game_object_id, game_object_name)
                
                hotspots = []
                if game_object_id and self.parser and game_object_id in self.parser.gameobject_spawns:
                    spawns = self.parser.gameobject_spawns[game_object_id]
                    sorted_spawns = sorted(spawns, key=lambda s: (s['x'], s['y']))
                    
                    for spawn in sorted_spawns[:25]:
                        hotspots.append({
                            'x': round(spawn['x'], 2),
                            'y': round(spawn['y'], 2),
                            'z': round(spawn['z'], 2)
                        })
                
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
        
        xml_lines = []
        for quest_id, quest_data in quest_overrides.items():
            if not quest_data['objectives']:
                continue
            
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


# ============================================================================
# Main
# ============================================================================

def main():
    import argparse
    
    parser = argparse.ArgumentParser(description='Convert Zygor guides to CopilotBuddy profiles V2 (with conditions)')
    parser.add_argument('zygor_file', help='Path to Zygor guide .lua file')
    parser.add_argument('-o', '--output', help='Output directory for XML profiles', default='.')
    parser.add_argument('-q', '--questie', help='Path to Questie-335 addon folder')
    parser.add_argument('-r', '--race', help='Filter by race (e.g., Orc, Troll)')
    parser.add_argument('--list', action='store_true', help='List guides in file without converting')
    
    args = parser.parse_args()
    
    print("=== Zygor Parser V2 - Intelligent XML Generator ===")
    print("  CustomBehaviors are wrapped in If/While conditions")
    print("")
    
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
    
    # Enrich with Questie
    if args.questie:
        for guide in guides:
            zygor_parser.enrich_with_questie(guide)
            zygor_parser.resolve_unknown_quests(guide)  # Post-process to fix Unknown quests
    
    # Generate profiles with V2 generator
    generator = ProfileGeneratorV2(zygor_parser)
    output_dir = Path(args.output)
    output_dir.mkdir(parents=True, exist_ok=True)
    
    for guide in guides:
        safe_name = re.sub(r'[\\/*?:"<>|]', '_', guide.name)
        safe_name = safe_name.replace(' ', '_')
        output_file = output_dir / f"{safe_name}.xml"
        
        xml_content = generator.generate_xml(guide, race_filter=args.race)
        
        with open(output_file, 'w', encoding='utf-8') as f:
            f.write(xml_content)
        
        # Count behaviors
        use_count = xml_content.count('File="UseItemOn"')
        interact_count = xml_content.count('File="InteractWith"')
        while_count = xml_content.count('<While')
        if_count = xml_content.count('<If')
        
        print(f"Generated: {output_file}")
        print(f"  - UseItemOn: {use_count}, InteractWith: {interact_count}")
        print(f"  - Conditions: {while_count} While, {if_count} If")
    
    print(f"\nGenerated {len(guides)} profiles with intelligent conditions!")


if __name__ == '__main__':
    main()
