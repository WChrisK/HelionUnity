namespace Helion.Core.Resource.MapsNew.Components
{
    public class Vertex
    {
        public readonly int Index;
        public float X;
        public float Y;

        // ZDoom specific.
        public float? FloorZ;
        public float? CeilingZ;

        public Vertex(int index)
        {
            Index = index;
        }
    }
}
