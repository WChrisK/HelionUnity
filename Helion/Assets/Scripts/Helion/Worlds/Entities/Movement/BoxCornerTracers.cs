using Helion.Util.Geometry.Segments;

namespace Helion.Worlds.Entities.Movement
{
    public struct BoxCornerTracers
    {
        public readonly Seg2F First;
        public readonly Seg2F Second;
        public readonly Seg2F Third;

        public BoxCornerTracers(Seg2F first, Seg2F second, Seg2F third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }
}
