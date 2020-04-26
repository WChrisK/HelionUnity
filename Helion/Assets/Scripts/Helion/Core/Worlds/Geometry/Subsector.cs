using System;
using System.Collections.Generic;
using Helion.Core.Util.Geometry.Segments;

namespace Helion.Core.Worlds.Geometry
{
    /// <summary>
    /// A convex polygon that is the leaf of a BSP tree. This also happens to
    /// be the shape to which we can render a floor or ceiling.
    /// </summary>
    public class Subsector : IDisposable
    {
        public readonly int Index;
        public readonly Sector Sector;

        public Subsector(int index, Sector sector, List<Seg2F> edges)
        {
            Index = index;
            Sector = sector;

            // TODO: Make subsector floor/ceiling.

            sector.Subsectors.Add(this);
        }

        public void UpdateMeshes()
        {
            // TODO
        }

        public void Dispose()
        {
            // TODO: Dispose floor/ceiling.
        }
    }
}
