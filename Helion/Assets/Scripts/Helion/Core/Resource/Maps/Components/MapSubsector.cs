using System.Collections.Generic;
using Helion.Core.Util.Geometry.Segments;

namespace Helion.Core.Resource.Maps.Components
{
    public class MapSubsector
    {
        public readonly int Index;
        public int SectorIndex;
        public List<Seg2F> Edges = new List<Seg2F>();

        public MapSubsector(int index)
        {
            Index = index;
        }
    }
}
