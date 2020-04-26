using System;
using System.Collections.Generic;
using Helion.Core.Util.Geometry.Segments;

namespace Helion.Core.Worlds.Geometry
{
    public class Subsector : IDisposable
    {
        public readonly int Index;
        public readonly Sector Sector;

        public Subsector(int index, Sector sector, List<Line2F> edges)
        {
            Index = index;
            Sector = sector;

            // TODO: Make subsector floor/ceiling.
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
