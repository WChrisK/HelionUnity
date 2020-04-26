using Helion.Core.Resource.Maps.Components;
using Helion.Core.Util;
using Helion.Core.Util.Geometry.Segments;
using Helion.Core.Worlds.Geometry.Enums;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    public class Line
    {
        public readonly int Index;
        public readonly Side Front;
        public readonly Optional<Side> Back;
        public readonly Line2F Segment;
        public Unpegged Unpegged;

        public Vector2 Start => Segment.Start;
        public Vector2 End => Segment.End;
        public bool OneSided => !TwoSided;
        public bool TwoSided => Back.HasValue;

        public Line(int index, MapLinedef linedef, Vector2 start, Vector2 end, Side front,
            Side back = null)
        {
            Index = index;
            Front = front;
            Back = new Optional<Side>(back);
            Segment = new Line2F(start, end);
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
