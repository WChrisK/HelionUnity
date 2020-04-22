using Helion.Core.Util.Geometry.Vectors;

namespace Helion.Core.Resource.MapsNew.Components
{
    public class MapVertex : Vector2D
    {
        public readonly int Index;

        // ZDoom specific.
        public float? FloorZ;
        public float? CeilingZ;

        public MapVertex(int index, double x, double y) : base(x, y)
        {
            Index = index;
        }
    }
}
