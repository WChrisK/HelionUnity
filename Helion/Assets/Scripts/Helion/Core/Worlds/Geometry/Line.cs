using System.Collections.Generic;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using Helion.Core.Util.Geometry;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    public class Line
    {
        public readonly int Index;
        public readonly Line2 Segment;
        public readonly Side Front;
        public readonly Optional<Side> Back = Optional<Side>.Empty();

        public Line(DoomLinedef linedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            Index = linedef.Index;
            Segment = new Line2(linedef.Start.Vector, linedef.End.Vector);
            Front = new Side(this, true, linedef, linedef.Front, sectors, parentGameObject);

            if (linedef.TwoSided)
            {
                Back = new Side(this, false, linedef, linedef.Back.Value, sectors, parentGameObject);
                Front.PartnerSide = Back.Value;
                Back.Value.PartnerSide = Front;
            }
        }
    }
}
