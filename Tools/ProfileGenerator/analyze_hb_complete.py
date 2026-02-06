#!/usr/bin/env python3
"""
Analyse complète des profils HB pour créer un plan d'implémentation
"""
import os
import re
from collections import Counter, defaultdict

def analyze_hb_profiles(base_path):
    """Analyse tous les fichiers XML HB pour extraire les éléments utilisés"""
    
    results = {
        'profile_settings': Counter(),  # MinLevel, MaxLevel, etc.
        'objective_types': Counter(),   # KillMob, CollectItem, etc.
        'custom_behaviors': Counter(),  # Tous les CustomBehavior File=
        'condition_patterns': [],       # If Condition patterns
        'xml_elements': Counter(),      # Tous les éléments XML
    }
    
    for root, dirs, files in os.walk(base_path):
        for f in files:
            if f.endswith('.xml'):
                try:
                    filepath = os.path.join(root, f)
                    with open(filepath, 'r', encoding='utf-8', errors='ignore') as file:
                        content = file.read()
                        
                        # CustomBehaviors
                        for m in re.finditer(r'CustomBehavior File="([^"]+)"', content):
                            results['custom_behaviors'][m.group(1)] += 1
                        
                        # Objective Types
                        for m in re.finditer(r'Objective[^>]*Type="([^"]+)"', content):
                            results['objective_types'][m.group(1)] += 1
                        
                        # XML Elements
                        for m in re.finditer(r'<(\w+)\s', content):
                            results['xml_elements'][m.group(1)] += 1
                        
                        # Profile Settings
                        for setting in ['MinLevel', 'MaxLevel', 'MinDurability', 'MinFreeBagSlots',
                                       'MailGrey', 'SellGrey', 'TargetElites', 'ContinentID']:
                            if f'<{setting}>' in content:
                                results['profile_settings'][setting] += 1
                        
                        # If Conditions (sample)
                        for m in re.finditer(r'<If Condition="([^"]+)"', content):
                            if len(results['condition_patterns']) < 50:
                                results['condition_patterns'].append(m.group(1)[:100])
                                
                except Exception as e:
                    print(f"Error reading {filepath}: {e}")
    
    return results

def main():
    base_path = r"C:\Users\Texy\Desktop\HonorBuddy-2.0.0.5999-WoW-4.3.4\Default Profiles"
    
    print("=" * 80)
    print("ANALYSE COMPLÈTE DES PROFILS HONORBUDDY")
    print("=" * 80)
    
    results = analyze_hb_profiles(base_path)
    
    print("\n" + "=" * 80)
    print("CUSTOM BEHAVIORS (Top 30)")
    print("=" * 80)
    for cb, count in results['custom_behaviors'].most_common(30):
        print(f"  {cb}: {count}")
    
    print("\n" + "=" * 80)
    print("OBJECTIVE TYPES")
    print("=" * 80)
    for ot, count in results['objective_types'].most_common():
        print(f"  {ot}: {count}")
    
    print("\n" + "=" * 80)
    print("XML ELEMENTS (Top 30)")
    print("=" * 80)
    for elem, count in results['xml_elements'].most_common(30):
        print(f"  {elem}: {count}")
    
    print("\n" + "=" * 80)
    print("IF CONDITIONS (Exemples)")
    print("=" * 80)
    unique_conditions = set()
    for cond in results['condition_patterns']:
        # Extraire le pattern de base
        pattern = re.sub(r'\d+', 'N', cond)  # Remplacer nombres
        pattern = re.sub(r'"[^"]*"', '"X"', pattern)  # Remplacer strings
        unique_conditions.add(pattern[:80])
    
    for cond in sorted(unique_conditions)[:20]:
        print(f"  {cond}")
    
    print("\n" + "=" * 80)
    print("PROFILE SETTINGS")
    print("=" * 80)
    for setting, count in results['profile_settings'].most_common():
        print(f"  {setting}: {count}")

if __name__ == '__main__':
    main()
