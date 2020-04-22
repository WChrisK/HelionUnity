using System;
using Helion.Core.Resource.MapsNew;
using Helion.Core.Util;
using Helion.Core.Util.Timing;
using Helion.Core.Util.Unity;
using Helion.Core.WorldNew.Entities;
using Helion.Core.WorldNew.Geometry;
using UnityEngine;

namespace Helion.Core.WorldNew
{
    /// <summary>
    /// A world that runs a simulation.
    /// </summary>
    public class World : ITickable, IDisposable
    {
        public readonly CameraManager CameraManager;
        public readonly MapGeometry Geometry;
        public readonly EntityManager Entities;
        public int GameTick { get; private set; }
        private readonly Ticker timer = new Ticker(Constants.TickRateMillis);
        private readonly GameObject gameObject;

        private World(MapData map, GameObject gameObj)
        {
            gameObject = gameObj;
            CameraManager = new CameraManager(this);
            Geometry = new MapGeometry(map);
            Entities = new EntityManager(map);

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
        /// <param name="map">The map to create the world from.</param>
        /// <param name="world">The created world (or null on failure).</param>
        /// <param name="worldGameObject">The game object that is made for
        /// applying ticking monobehaviours to (or null on failure).</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool TryCreateWorld(MapData map, out World world, out GameObject worldGameObject)
        {
            worldGameObject = new GameObject($"World ({map.Name})");

            try
            {
                // TODO: BSP build here.
                world = new World(map, worldGameObject);

                WorldUpdater worldUpdater = worldGameObject.AddComponent<WorldUpdater>();
                worldUpdater.World = world;

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

        public void Dispose()
        {
            CameraManager.Dispose();
            Entities.Dispose();
            Geometry.Dispose();

            GameObjectHelper.Destroy(gameObject);
        }
    }
}
