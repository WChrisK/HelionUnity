using System;
using Helion.Core.Resource.Maps;
using Helion.Core.Util;
using Helion.Core.Util.Timing;
using Helion.Core.Util.Unity;
using Helion.Core.Worlds.Entities;
using Helion.Core.Worlds.Geometry;
using UnityEngine;
using static Helion.Core.Util.OptionalHelper;

namespace Helion.Core.Worlds
{
    /// <summary>
    /// A world that runs a simulation.
    /// </summary>
    public class World : ITickable, IDisposable
    {
        public int GameTick { get; private set; }
        public readonly MapGeometry Geometry;
        public readonly EntityManager Entities;
        private readonly GameObject gameObject;
        private readonly Ticker timer = new Ticker(Constants.TickRateMillis);

        /// <summary>
        /// A normalized value of the gametick. For example, if each tick is
        /// 28ms and the world has run for 14ms, then this returns 0.5f. This
        /// can go over 1.0f.
        /// </summary>
        public float GameTickFraction => timer.TickFraction;

        private World(IMap map)
        {
            gameObject = new GameObject($"World ({map.Name})");
            Geometry = new MapGeometry(gameObject, map);
            Entities = new EntityManager(this, gameObject, Geometry, map);

            timer.Start();
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
                return Empty;
            }
        }

        public void Tick()
        {
            GameTick++;
            timer.Restart();
        }

        public void Dispose()
        {
            Entities.Dispose();
            GameObjectHelper.Destroy(gameObject);
        }
    }
}
