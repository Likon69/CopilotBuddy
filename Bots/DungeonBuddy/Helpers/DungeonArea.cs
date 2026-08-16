using Styx.Logic.AreaManagement;
using Tripper.XNAMath;

namespace Bots.DungeonBuddy.Helpers
{
    public class DungeonArea : PolygonArea
    {
        public DungeonArea(params Vector2[] areaDefinition)
            : base(areaDefinition)
        {
        }

        public override AreaType Type => AreaType.Polygon;
    }
}
