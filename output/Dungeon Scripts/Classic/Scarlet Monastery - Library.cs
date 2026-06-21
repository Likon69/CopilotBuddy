using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Profiles;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class ScarletMonasteryLibrary : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 165; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(2867.000, -822.000, 160.3331); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(253.61, -202.38, 18.68); } }

        #endregion

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var t in units)
            {
                var prioObject = t.Object;
                //Target good ones first
                if (prioObject.Entry == 4304 || prioObject.Entry == 4293)
                {
                    t.Score += 400;
                }
            }
        }
        
        //This guy is a pain, theres a stupid ledge that blocks los so bots stand around thinking they can do stuff
        [EncounterHandler(3974, "Houndmaster Loksey")]
        public Composite HoundmasterLokseyFight()
        {
            WoWPoint goodSpot = new WoWPoint(124.1805, -253.1161, 18.54657);

            return new Decorator(r=>StyxWoW.Me.IsTank() && StyxWoW.Me.Location.Distance(goodSpot) > 8, new Action(r=>Navigator.MoveTo(goodSpot)));
        }

        WoWUnit _bestTarget;

        //Run away when hes channeling detonation
        [EncounterHandler(6487,"Arcanist Doan")]
        public Composite ArcanistDoanFight()
        {
            return new PrioritySelector(
                ctx => _bestTarget = ScriptHelpers.FindBestTargetWithIdsRange(40, 6487),
                ScriptHelpers.CreateRunAwayFromBad(ctx => _bestTarget != null && _bestTarget.CastingSpellId == 9435, 20f, 6487)
                );


        }

    }


     

    //<Vendor Name="player" Entry="0" Type="Repair" X="" />
}
