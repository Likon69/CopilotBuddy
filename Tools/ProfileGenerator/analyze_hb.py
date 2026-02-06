#!/usr/bin/env python3
"""Quick analysis of HB profile CustomBehaviors"""
import os

hb_file = r"C:\Users\Texy\Desktop\HonorBuddy-2.0.0.5999-WoW-4.3.4\Default Profiles\1-85 Questing Profile Pack\LK\[Fly][H - Quest] LK 68-80 [Kick].xml"

with open(hb_file, 'r', encoding='utf-8') as f:
    c = f.read()

print(f"File: {os.path.basename(hb_file)}")
print(f"Lines: {len(c.splitlines())}")
print(f"UseItemOn: {c.count('UseItemOn')}")
print(f"InteractWith: {c.count('InteractWith')}")
print(f"CastSpellOn: {c.count('CastSpellOn')}")
print(f"While loops: {c.count('<While')}")
print(f"RunTo: {c.count('<RunTo')}")
print(f"UserSettings: {c.count('UserSettings')}")
print(f"FlyTo: {c.count('FlyTo')}")
print(f"UserDialog: {c.count('UserDialog')}")
