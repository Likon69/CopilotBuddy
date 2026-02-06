# Profile Generator Tool

Converts Zygor leveling guides to CopilotBuddy XML profiles.

## Requirements

- Python 3.8+
- Zygor Guides addon (source of leveling guides)
- Questie-335 addon (optional, for coordinate enrichment)

## Usage

### List guides in a Zygor file:
```bash
python zygor_parser.py "C:\Path\To\ZygorLevelingHordeWOTLK.lua" --list
```

### Convert all guides:
```bash
python zygor_parser.py "C:\Path\To\ZygorLevelingHordeWOTLK.lua" -o ./output
```

### Convert with Questie enrichment:
```bash
python zygor_parser.py "C:\Path\To\ZygorLevelingHordeWOTLK.lua" -o ./output -q "C:\Path\To\Questie-335"
```

### Filter by race:
```bash
python zygor_parser.py "C:\Path\To\ZygorLevelingHordeWOTLK.lua" -o ./output -r Orc
```

## Example (your setup):

```bash
python zygor_parser.py "C:\Users\Texy\Desktop\ZygorGuidesViewerClassicTBC\Guides-MOP\Leveling\ZygorLevelingHordeWOTLK.lua" -o ./output -q "C:\Users\Texy\Desktop\Questie-335"
```

## Output Format

The tool generates CopilotBuddy-compatible XML profiles:

```xml
<?xml version="1.0" encoding="utf-8"?>
<HBProfile>
  <Name>Leveling Guides\Starter Guides\Orc Starter (1-5)</Name>
  <MinLevel>1</MinLevel>
  <MaxLevel>5</MaxLevel>
  
  <QuestOrder>
    <PickUp QuestName="Your Place In The World" QuestId="4641" GiverName="Kaltunk" GiverId="10176" />
    <TurnIn QuestName="Your Place In The World" QuestId="4641" TurnInName="Gornek" TurnInId="3143" />
    <PickUp QuestName="Cutting Teeth" QuestId="788" GiverName="Gornek" GiverId="3143" />
    <Objective QuestName="Cutting Teeth" QuestId="788" Type="KillMob" MobId="3098" KillCount="8" />
    <TurnIn QuestName="Cutting Teeth" QuestId="788" TurnInName="Gornek" TurnInId="3143" />
  </QuestOrder>
</HBProfile>
```

## Supported Zygor Commands

| Zygor DSL | CopilotBuddy XML |
|-----------|------------------|
| `accept Quest##ID` | `<PickUp QuestId="ID" />` |
| `turnin Quest##ID` | `<TurnIn QuestId="ID" />` |
| `kill N Mob##ID` | `<Objective Type="KillMob" MobId="ID" KillCount="N" />` |
| `collect N Item##ID` | `<Objective Type="CollectItem" ItemId="ID" CollectCount="N" />` |
| `talk NPC##ID` | Sets GiverName/TurnInName |
| `\|goto Zone X,Y` | Coordinates (future: Hotspots) |

## Notes

- This tool is in `Tools/` which is gitignored
- Zygor guides are copyrighted - generated profiles for personal use only
- Questie enrichment adds missing NPC coordinates and names
