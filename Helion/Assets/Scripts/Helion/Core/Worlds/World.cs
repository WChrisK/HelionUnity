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
        private readonly GameObject gameObject;
        private readonly MapGeometry geometry;
        private readonly EntityManager entityManager;

        private World(IMap map)
        {
            gameObject = new GameObject($"World ({map.Name})");
            geometry = new MapGeometry(gameObject, map);
            entityManager = new EntityManager(gameObject, geometry, map);
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
