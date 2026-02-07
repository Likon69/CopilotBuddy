#!/usr/bin/env python3
"""
Batch generator for V2 profiles
Generates profiles for all Zygor leveling guides (Classic + WotLK)
"""

import subprocess
import sys
from pathlib import Path

# Paths
ZYGOR_BASE = Path(r"c:\Users\Texy\Desktop\ZygorGuidesViewerClassicTBC\Guides-WOTLK\Leveling")
QUESTIE_BASE = Path(r"c:\Users\Texy\Desktop\Questie-335")  # Root folder, parser adds /Database/Wotlk/
OUTPUT_BASE = Path(__file__).parent / "output_v2"
PARSER_SCRIPT = Path(__file__).parent / "zygor_parser_v2.py"
SPAWNS_JSON = Path(__file__).parent / "gameobject_spawns.json"

# Zygor guide files - (filename, faction)
# Toutes les races partagent les mêmes quêtes après les zones de départ
GUIDES = [
    # Horde
    ("ZygorLevelingHordeCLASSIC.lua", "Horde"),
    ("ZygorLevelingHordeWOTLK.lua", "Horde"),
    # Alliance
    ("ZygorLevelingAllianceCLASSIC.lua", "Alliance"),
    ("ZygorLevelingAllianceWOTLK.lua", "Alliance"),
]

def main():
    OUTPUT_BASE.mkdir(parents=True, exist_ok=True)
    
    total = 0
    success = 0
    
    for guide_file, faction in GUIDES:
        zygor_path = ZYGOR_BASE / guide_file
        
        if not zygor_path.exists():
            print(f"[SKIP] {guide_file} not found")
            continue
        
        # Create faction-specific output directory
        faction_dir = OUTPUT_BASE / faction
        faction_dir.mkdir(parents=True, exist_ok=True)
        
        cmd = [
            sys.executable,
            str(PARSER_SCRIPT),
            str(zygor_path),
            "-o", str(faction_dir),
            "--questie", str(QUESTIE_BASE),
            "--faction", faction,
        ]
        
        print(f"\n{'='*60}")
        print(f"Processing: {guide_file} ({faction})")
        print(f"{'='*60}")
        
        result = subprocess.run(cmd, capture_output=False)
        total += 1
        if result.returncode == 0:
            success += 1
    
    print(f"\n{'='*60}")
    print(f"SUMMARY: {success}/{total} successful")
    print(f"{'='*60}")

if __name__ == "__main__":
    main()
