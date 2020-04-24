using System.Collections.Generic;
using Helion.Core.Util.Geometry.Segments;

namespace Helion.Core.Resource.MapsNew.Components
{
    public class MapSubsector
    {
        public readonly int Index;
        public int SectorIndex;
        public List<Line2F> Edges = new List<Line2F>();

        public MapSubsector(int index)
        {
            Index = index;
        }
    }
}
