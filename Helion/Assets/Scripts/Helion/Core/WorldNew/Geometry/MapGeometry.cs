using System;
using Helion.Bsp;
using Helion.Bsp.Node;
using Helion.Core.Resource.MapsNew;

namespace Helion.Core.WorldNew.Geometry
{
    public class MapGeometry : IDisposable
    {
        public MapGeometry(MapData map)
        {
            BspNode bspRoot = new BspBuilder(map).Build();
            if (bspRoot == null)
                throw new Exception("Failed to generate BSP tree");

            // TODO
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
