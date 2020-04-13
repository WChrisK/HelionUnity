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
            Front = new Side(this, linedef, linedef.Front, sectors, parentGameObject);
            if (linedef.TwoSided)
                Back = new Side(this, linedef, linedef.Back.Value, sectors, parentGameObject);
        }

        /// <summary>
        /// Gets the partner side. This should not be called on a one sided
        /// line. For example, if we pass this the front, we get the back.
        /// Likewise, if we pass in the back, we get the front side.
        /// </summary>
        /// <param name="side">The side that we want to get the partner of.
        /// </param>
        /// <returns>The partner side.</returns>
        public Side PartnerSideOf(Side side) => ReferenceEquals(side, Front) ? Back.Value : Front;
    }
}
