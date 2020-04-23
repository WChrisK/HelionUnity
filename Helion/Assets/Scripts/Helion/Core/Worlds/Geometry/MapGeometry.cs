using System;
using Helion.Bsp;
using Helion.Bsp.Node;
using Helion.Core.Resource.MapsNew;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    public class MapGeometry : IDisposable
    {
        public MapGeometry(MapData map)
        {
            try
            {
                BspBuilder builder = new BspBuilder(map);
                BspNode bspRoot = builder.Build();
                if (bspRoot == null)
                    throw new Exception("Failed to generate BSP tree");
            }
            catch (Exception e)
            {
                Debug.Log($":( {e.Message}");
                throw;
            }

            // TODO
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
