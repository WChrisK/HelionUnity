using System;
using Helion.Resource.Maps;
using Helion.Util;
using Helion.Util.Timing;
using Helion.Util.Unity;
using Helion.Worlds.Entities;
using Helion.Worlds.Geometry;
using Helion.Worlds.Info;
using UnityEngine;

namespace Helion.Worlds
{
    /// <summary>
    /// A world that runs a simulation.
    /// </summary>
    public class World : ITickable, IDisposable
    {
        public readonly WorldInfo Info;
        public readonly MapGeometry Geometry;
        public readonly EntityManager Entities;
        public int GameTick { get; private set; }
        private readonly Ticker timer = new Ticker(Constants.TickRateMillis);
        private readonly GameObject gameObject;

        private World(WorldInfo info, MapData map, GameObject gameObj)
        {
            Info = info;
            gameObject = gameObj;
            Geometry = new MapGeometry(map);
            Entities = new EntityManager(this, map);

            timer.Start();
        }

        /// <summary>
        /// Tries to create a world from the map provided.
        /// </summary>
        /// <remarks>
        /// This should never fail, as that indicates something has gone
        /// terribly wrong since the map data should be vetted to make sure
        /// it's all valid. Any BSP failure should be reported to developers
        /// since they should not be happening either.
        /// </remarks>
        /// <param name="info">The world information.</param>
        /// <param name="map">The map to create the world from.</param>
        /// <param name="world">The created world (or null on failure).</param>
        /// <param name="worldGameObject">The game object that is made for
        /// applying ticking monobehaviours to (or null on failure).</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool TryCreateWorld(WorldInfo info, MapData map, out World world, out GameObject worldGameObject)
        {
            worldGameObject = new GameObject($"World ({map.Name})");

            try
            {
                world = new World(info, map, worldGameObject);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Warning: There may be lingering game objects that were not cleaned up!");
                Debug.Log("This should not happen. Contact a developer!");
                Debug.Log($"Reason: {e.Message}");

                GameObjectHelper.Destroy(worldGameObject);
                world = null;
                worldGameObject = null;
                return false;
            }
        }

        public void Tick()
        {
            Entities.Tick();

            GameTick++;
            timer.Restart();
        }

        /// <summary>
        /// To be called by Unity's Update() method, so we know to only update
        /// rendering component interpolations.
        /// </summary>
        public void Update()
        {
            Entities.Update(timer.TickFraction);
        }

        public void Dispose()
        {
            Entities.Dispose();
            Geometry.Dispose();

            GameObjectHelper.Destroy(gameObject);
        }
    }
}
