using Helion.Core.Resource.Maps;
using Helion.Core.Util;
using Helion.Core.Worlds.Entities;
using Helion.Core.Worlds.Geometry;
using UnityEngine;

namespace Helion.Core.Worlds
{
    /// <summary>
    /// A world that runs a simulation.
    /// </summary>
    public class World
    {
        public readonly MapGeometry Geometry;
        public readonly EntityManager Entities;
        private readonly GameObject gameObject;

        private World(IMap map)
        {
            gameObject = new GameObject($"World ({map.Name})");
            Geometry = new MapGeometry(gameObject, map);
            Entities = new EntityManager(gameObject, Geometry, map);
        }

        /// <summary>
        /// Creates a world from a map.
        /// </summary>
        /// <param name="map">The map to create a world from.</param>
        /// <returns>The world for the map, or an empty value if there was an
        /// error creating the world.</returns>
        public static Optional<World> From(IMap map)
        {
            try
            {
                return new World(map);
            }
            catch
            {
                return Optional<World>.Empty();
            }
        }
    }
}
