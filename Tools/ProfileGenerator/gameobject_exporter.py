#!/usr/bin/env python3
"""
GameObject Spawns Exporter - Direct MariaDB Connection
Extracts GameObject spawn world coordinates from WoW server database.

Usage:
    python gameobject_exporter.py -q "path/to/Questie-335" [--host localhost] [--port 3306] [--user root] [--password ""] [--database world]
"""

import re
import sys
import json
import argparse
from pathlib import Path
from typing import Dict, List, Set

# MariaDB connector - install with: pip install mariadb
try:
    import mariadb
except ImportError:
    print("MariaDB connector not installed. Installing...")
    import subprocess
    subprocess.check_call([sys.executable, "-m", "pip", "install", "mariadb"])
    import mariadb


def parse_questie_item_db(questie_path: str) -> Dict[int, List[int]]:
    """
    Parse Questie itemDB.lua to extract item_id -> [game_object_ids] mapping
    """
    item_to_objects: Dict[int, List[int]] = {}
    
    # Try WotLK first
    for db_name in ['wotlkItemDB.lua', 'tbcItemDB.lua', 'itemDB.lua']:
        item_db_path = Path(questie_path) / 'Database' / 'Wotlk' / db_name
        if not item_db_path.exists():
            item_db_path = Path(questie_path) / 'Database' / 'TBC' / db_name
        if not item_db_path.exists():
            item_db_path = Path(questie_path) / 'Database' / db_name
        if item_db_path.exists():
            break
    
    if not item_db_path.exists():
        print(f"Warning: Could not find itemDB.lua in {questie_path}")
        return item_to_objects
    
    print(f"Parsing {item_db_path}...")
    
    with open(item_db_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Pattern: [item_id] = {'name', npcDrops, objectDrops, ...}
    pattern = r"\[(\d+)\]\s*=\s*\{['\"]([^'\"]+)['\"],([^,]*),(\{[^}]*\}|nil)"
    
    for match in re.finditer(pattern, content):
        item_id = int(match.group(1))
        object_drops = match.group(4)
        
        if object_drops and object_drops != 'nil':
            obj_ids = re.findall(r'(\d+)', object_drops)
            if obj_ids:
                item_to_objects[item_id] = [int(x) for x in obj_ids]
    
    print(f"  Found {len(item_to_objects)} items with objectDrops")
    return item_to_objects


def parse_questie_object_db(questie_path: str) -> Dict[int, str]:
    """Parse Questie objectDB.lua to get GameObject names"""
    object_names: Dict[int, str] = {}
    
    for db_name in ['wotlkObjectDB.lua', 'tbcObjectDB.lua', 'objectDB.lua']:
        obj_db_path = Path(questie_path) / 'Database' / 'Wotlk' / db_name
        if not obj_db_path.exists():
            obj_db_path = Path(questie_path) / 'Database' / 'TBC' / db_name
        if not obj_db_path.exists():
            obj_db_path = Path(questie_path) / 'Database' / db_name
        if obj_db_path.exists():
            break
    
    if not obj_db_path.exists():
        return object_names
    
    print(f"Parsing {obj_db_path}...")
    
    with open(obj_db_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    pattern = r'\[(\d+)\]\s*=\s*\{["\']([^"\']+)["\']'
    
    for match in re.finditer(pattern, content):
        obj_id = int(match.group(1))
        obj_name = match.group(2)
        object_names[obj_id] = obj_name
    
    print(f"  Found {len(object_names)} game objects")
    return object_names


def fetch_spawns_from_db(object_ids: Set[int], host: str, port: int, user: str, password: str, database: str) -> Dict[int, List[Dict]]:
    """
    Connect to MariaDB and fetch GameObject spawn coordinates
    """
    spawns: Dict[int, List[Dict]] = {}
    
    if not object_ids:
        return spawns
    
    print(f"\nConnecting to MariaDB {host}:{port}/{database}...")
    
    try:
        conn = mariadb.connect(
            host=host,
            port=port,
            user=user,
            password=password,
            database=database
        )
        cursor = conn.cursor()
        
        # Build query with all object IDs
        ids_str = ','.join(str(x) for x in sorted(object_ids))
        
        # Query gameobject table (works for TrinityCore/AzerothCore/MaNGOS)
        query = f"""
            SELECT id, position_x, position_y, position_z 
            FROM gameobject 
            WHERE id IN ({ids_str})
            ORDER BY id
        """
        
        print(f"Querying {len(object_ids)} GameObjects...")
        cursor.execute(query)
        
        row_count = 0
        for (obj_id, x, y, z) in cursor:
            if obj_id not in spawns:
                spawns[obj_id] = []
            spawns[obj_id].append({
                'x': float(x),
                'y': float(y),
                'z': float(z)
            })
            row_count += 1
        
        print(f"  Retrieved {row_count} spawn points for {len(spawns)} objects")
        
        cursor.close()
        conn.close()
        
    except mariadb.Error as e:
        print(f"Database error: {e}")
        raise
    
    return spawns


def generate_spawns_json(spawns: Dict[int, List[Dict]], 
                         object_names: Dict[int, str],
                         output_file: str):
    """Generate gameobject_spawns.json"""
    output = {
        "_info": "GameObject spawn world coordinates for CopilotBuddy profiles",
        "_usage": "Used by zygor_parser.py to generate <Hotspots> for CollectItem objectives",
        "objects": {}
    }
    
    for obj_id, coords_list in spawns.items():
        name = object_names.get(obj_id, f"Unknown Object {obj_id}")
        output["objects"][str(obj_id)] = {
            "name": name,
            "spawns": coords_list
        }
    
    with open(output_file, 'w', encoding='utf-8') as f:
        json.dump(output, f, indent=2)
    
    print(f"\nSpawns JSON saved to: {output_file}")
    print(f"  Total objects: {len(spawns)}")
    print(f"  Total spawn points: {sum(len(v) for v in spawns.values())}")


def main():
    parser = argparse.ArgumentParser(description='GameObject Spawns Exporter - MariaDB Direct')
    parser.add_argument('-q', '--questie', required=True, help='Path to Questie addon folder')
    parser.add_argument('--host', default='127.0.0.1', help='MariaDB host (default: 127.0.0.1)')
    parser.add_argument('--port', type=int, default=3306, help='MariaDB port (default: 3306)')
    parser.add_argument('--user', default='root', help='MariaDB user (default: root)')
    parser.add_argument('--password', default='', help='MariaDB password (default: empty)')
    parser.add_argument('--database', default='world', help='Database name (default: world)')
    parser.add_argument('-o', '--output', default=None, help='Output JSON file (default: gameobject_spawns.json)')
    
    args = parser.parse_args()
    
    script_dir = Path(__file__).parent
    output_file = args.output or str(script_dir / "gameobject_spawns.json")
    
    # Parse Questie databases
    item_to_objects = parse_questie_item_db(args.questie)
    object_names = parse_questie_object_db(args.questie)
    
    # Collect all unique object IDs needed
    all_object_ids: Set[int] = set()
    for obj_ids in item_to_objects.values():
        all_object_ids.update(obj_ids)
    
    print(f"\nTotal unique GameObjects to fetch: {len(all_object_ids)}")
    
    # Fetch spawns from database
    spawns = fetch_spawns_from_db(
        all_object_ids,
        args.host,
        args.port,
        args.user,
        args.password,
        args.database
    )
    
    # Generate JSON
    generate_spawns_json(spawns, object_names, output_file)
    
    # Show some stats
    missing = all_object_ids - set(spawns.keys())
    if missing:
        print(f"\nNote: {len(missing)} objects have no spawns in database (may be event/unused objects)")


if __name__ == '__main__':
    main()
