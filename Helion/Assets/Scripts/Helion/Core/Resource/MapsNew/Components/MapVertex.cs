namespace Helion.Core.Resource.MapsNew.Components
{
    public class MapVertex
    {
        public readonly int Index;
        public float X;
        public float Y;

        // ZDoom specific.
        public float? FloorZ;
        public float? CeilingZ;

        public MapVertex(int index)
        {
            Index = index;
        }
    }
}
