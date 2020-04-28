using Helion.Resource.Maps.Components;
using Helion.Util;
using Helion.Util.Geometry.Segments;
using Helion.Util.Geometry.Vectors;
using Helion.Worlds.Geometry.Enums;

namespace Helion.Worlds.Geometry
{
    /// <summary>
    /// A line in a map that contains one or two sides.
    /// </summary>
    public class Line
    {
        public readonly int Index;
        public readonly Side Front;
        public readonly Optional<Side> Back;
        public readonly Seg2F Segment;
        public Unpegged Unpegged;

        public Vec2F Start => Segment.Start;
        public Vec2F End => Segment.End;
        public bool OneSided => !TwoSided;
        public bool TwoSided => Back.HasValue;

        public Line(int index, MapLinedef linedef, Vec2F start, Vec2F end, Side front,
            Side back = null)
        {
            Index = index;
            Front = front;
            Back = new Optional<Side>(back);
            Segment = new Seg2F(start, end);
            Unpegged = ToUnpegged(linedef);

            front.Line = this;
            if (back != null)
                back.Line = this;
        }

        private static Unpegged ToUnpegged(MapLinedef linedef)
        {
            bool lower = linedef.LowerUnpegged;
            bool upper = linedef.UpperUnpegged;

            if (!lower && !upper)
                return Unpegged.None;
            if (lower && !upper)
                return Unpegged.Lower;
            return lower ? Unpegged.UpperAndLower : Unpegged.Upper;
        }
    }
}
