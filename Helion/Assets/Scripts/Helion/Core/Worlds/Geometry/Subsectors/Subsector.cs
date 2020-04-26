using System;
using System.Collections.Generic;

namespace Helion.Core.Worlds.Geometry.Subsectors
{
    /// <summary>
    /// A convex polygon that is the leaf of a BSP tree. This also happens to
    /// be the shape to which we can render a floor or ceiling.
    /// </summary>
    public class Subsector
    {
        public readonly int Index;
        public readonly Sector Sector;
        public readonly List<SubsectorPlane> SubsectorPlanes;

        public SubsectorPlane Floor => SubsectorPlanes[0];
        public SubsectorPlane Ceiling => SubsectorPlanes[1];

        public Subsector(int index, Sector sector, SubsectorPlane floor, SubsectorPlane ceiling)
        {
            Index = index;
            Sector = sector;
            SubsectorPlanes = new List<SubsectorPlane> { floor, ceiling };

            floor.Subsector = this;
            ceiling.Subsector = this;
            sector.Subsectors.Add(this);
        }
    }
}
