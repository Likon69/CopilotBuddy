"""
Script to update profile names to include faction path
Updates <Name> and Message elements to include Horde\\ or Alliance\\
"""
import os
import re

output_dir = 'output_v2'

def update_profile(filepath, faction):
    """Update a single profile to include faction in Name and Messages"""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    original = content
    
    # Get the current name from <Name> tag
    name_match = re.search(r'<Name>([^<]+)</Name>', content)
    if not name_match:
        return False
    
    current_name = name_match.group(1)
    
    # Extract base name (without faction prefix)
    if current_name.startswith(f'{faction}\\\\'):
        base_name = current_name[len(faction) + 2:]  # Remove "Horde\\" or "Alliance\\"
    else:
        base_name = current_name
    
    # Check if messages still need updating (they have old format without faction)
    needs_update = f'"Leveling Guides\\\\{base_name}"' in content or f'"Leveling Guides\\\\{base_name} Complete"' in content
    
    # Also update Name if needed
    if not current_name.startswith(f'{faction}\\\\'):
        new_name = f'{faction}\\\\{base_name}'
        content = content.replace(
            f'<Name>{current_name}</Name>',
            f'<Name>{new_name}</Name>',
            1
        )
        needs_update = True
    
    if not needs_update:
        return False
    
    # Update start Message - has "Leveling Guides\\{name}" format
    content = content.replace(
        f'<CustomBehavior File="Message" Text="Leveling Guides\\\\{base_name}" LogColor="Green"',
        f'<CustomBehavior File="Message" Text="Leveling Guides\\\\{faction}\\\\{base_name}" LogColor="Green"',
        1
    )
    
    # Update end Message - has "Leveling Guides\\{name} Complete" format
    content = content.replace(
        f'<CustomBehavior File="Message" Text="Leveling Guides\\\\{base_name} Complete" LogColor="Orange"',
        f'<CustomBehavior File="Message" Text="Leveling Guides\\\\{faction}\\\\{base_name} Complete" LogColor="Orange"',
        1
    )
    
    if content != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        return True
    return False

def main():
    updated = 0
    
    for faction in ['Horde', 'Alliance']:
        faction_dir = os.path.join(output_dir, faction)
        if not os.path.exists(faction_dir):
            continue
        
        for filename in os.listdir(faction_dir):
            if filename.endswith('.xml'):
                filepath = os.path.join(faction_dir, filename)
                if update_profile(filepath, faction):
                    print(f"Updated: {faction}/{filename}")
                    updated += 1
    
    print(f"\nTotal updated: {updated}")

if __name__ == '__main__':
    main()
