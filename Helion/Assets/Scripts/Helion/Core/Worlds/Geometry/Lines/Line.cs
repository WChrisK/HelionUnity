using System.Collections.Generic;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using Helion.Core.Util.Geometry;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry.Lines
{
    public class Line
    {
        public readonly int Index;
        public readonly Line2 Segment;
        public readonly Unpeg Unpeg;
        public readonly Side Front;
        public readonly Optional<Side> Back = Optional<Side>.Empty();

        public Line(DoomLinedef linedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            Index = linedef.Index;
            Segment = new Line2(linedef.Start.Vector, linedef.End.Vector);
            Unpeg = CalculateUnpegged(linedef.Flags);
            Front = new Side(this, true, linedef, linedef.Front, sectors, parentGameObject);

            if (linedef.TwoSided)
            {
                Back = new Side(this, false, linedef, linedef.Back.Value, sectors, parentGameObject);
                Front.PartnerSide = Back.Value;
                Back.Value.PartnerSide = Front;
            }
        }

        private static Unpeg CalculateUnpegged(ushort flags)
        {
            if ((flags & 0x0018) == 0x0018)
                return Unpeg.LowerAndUpper;
            if ((flags & 0x0008) == 0x0008)
                return Unpeg.Upper;
            if ((flags & 0x0010) == 0x0010)
                return Unpeg.Lower;
            return Unpeg.None;
        }
    }
}
