using Helion.Core.Resource.MapsNew.Components;
using Helion.Core.Util;
using Helion.Core.Worlds.Geometry.Enums;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    public class Line
    {
        public readonly int Index;
        public readonly Vector2 Start;
        public readonly Vector2 End;
        public readonly Side Front;
        public readonly Optional<Side> Back;
        public Unpegged Unpegged;

        public bool OneSided => !TwoSided;
        public bool TwoSided => Back.HasValue;

        public Line(int index, MapLinedef linedef, Vector2 start, Vector2 end, Side front,
            Side back = null)
        {
            Index = index;
            Start = start;
            End = end;
            Front = front;
            Back = new Optional<Side>(back);
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
