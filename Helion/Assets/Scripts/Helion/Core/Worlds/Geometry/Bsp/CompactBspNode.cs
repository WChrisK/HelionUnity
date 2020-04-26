using Helion.Core.Util.Geometry.Segments;

namespace Helion.Core.Worlds.Geometry.Bsp
{
    public struct CompactBspNode
    {
        public const uint IsSubsectorBit = 0x80000000;

        public readonly uint LeftChildBits;
        public readonly uint RightChildBits;
        public readonly Line2F Splitter;

        public uint LeftChildWithoutBits => LeftChildBits & ~IsSubsectorBit;
        public uint RightChildWithoutBits => RightChildBits & ~IsSubsectorBit;

        public CompactBspNode(uint leftChildBits, uint rightChildBits, Line2F splitter)
        {
            LeftChildBits = leftChildBits;
            RightChildBits = rightChildBits;
            Splitter = splitter;
        }
    }
}
