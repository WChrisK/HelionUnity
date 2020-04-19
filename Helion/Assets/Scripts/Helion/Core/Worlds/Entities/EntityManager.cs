using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Resource;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Logging;
using Helion.Core.Util.Unity;
using Helion.Core.Worlds.Geometry;
using UnityEngine;

namespace Helion.Core.Worlds.Entities
{
    /// <summary>
    /// Manages all of the entities in a world.
    /// </summary>
    public class EntityManager : IEnumerable<Entity>, IDisposable
    {
        private static readonly Log Log = LogManager.Instance();

        public readonly SpawnPoints SpawnPoints = new SpawnPoints();
        private readonly GameObject entityCollectorGameObject;
        private readonly World world;
        private readonly MapGeometry geometry;
        private readonly LinkedList<Entity> entities = new LinkedList<Entity>();
        private readonly Dictionary<int, Player> players = new Dictionary<int, Player>();

        public EntityManager(World owningWorld, GameObject parentObject, MapGeometry mapGeometry, IMap map)
        {
            world = owningWorld;
            geometry = mapGeometry;

            entityCollectorGameObject = new GameObject("Entities");
            parentObject.SetChild(entityCollectorGameObject);

            CreateEntities(((DoomMap)map).Things);
        }

        public Optional<Entity> SpawnPlayer(int playerNumber)
        {
            if (players.ContainsKey(playerNumber))
            {
                Log.Warn("Trying to create multiple players for player ", playerNumber);
                return Optional<Entity>.Empty();
            }

            Optional<ActorDefinition> actorDefinition = Data.Decorate.Find("DOOMPLAYER");
            if (!actorDefinition)
                return Optional<Entity>.Empty();

            Vector2? position = SpawnPoints.Coop(playerNumber);
            if (position == null)
                return Optional<Entity>.Empty();

            Entity entity = CreateEntity(actorDefinition.Value, position.Value);

            Player player = entity.gameObject.AddComponent<Player>();
            player.PlayerNumber = playerNumber;
            player.Attach(entity);
            players[playerNumber] = player;

            return entity;
        }

        public void Dispose()
        {
            // Have to clone the list because the entity will unlink itself,
            // and doing that during iteration is probably really bad.
            entities.ToList().ForEach(entity => entity.Dispose());
        }

        public IEnumerator<Entity> GetEnumerator() => entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void CreateEntities(IEnumerable<DoomThing> things)
        {
            foreach (DoomThing thing in things)
            {
                Optional<ActorDefinition> actorDefinition = Data.Decorate.Find(thing.EditorNumber);
                if (!actorDefinition)
                {
                    Log.Warn("Unknown entity type: ", thing.EditorNumber);
                    continue;
                }

                ActorDefinition definition = actorDefinition.Value;
                if (definition.ActorType.SpawnPoint)
                {
                    SpawnPoints.Add(thing.Position, definition);
                    continue;
                }

                CreateEntity(definition, thing.Position);
            }
        }

        private Entity CreateEntity(ActorDefinition definition, Vector2 position)
        {
            GameObject entityObject = new GameObject($"{definition.Name}");
            entityCollectorGameObject.SetChild(entityObject);

            // Note: The definition should be set immediately on the entity
            // as other code relies on it. We should consider restructuring
            // this so the dependency is not required.
            Entity entity = entityObject.AddComponent<Entity>();
            entity.Definition = definition;
            entity.world = world;
            entity.decorateStateTracker = new DecorateStateTracker(entity);
            entity.entityNode = entities.AddLast(entity);

            float height = entity.Definition.Properties.Height;
            float y = geometry.FloorHeight(position);
            Vector3 worldPos = new Vector3(position.x, y, position.y);
            entity.transform.position = worldPos.MapUnit();
            entity.Position = worldPos;
            entity.PrevPosition = worldPos;

            float diameter = entity.Definition.Properties.Radius * 2;
            BoxCollider collider = entityObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(0, height / 2, 0).MapUnit();
            collider.size = new Vector3(diameter, height, diameter).MapUnit();
            AddLineRendererIfDebug(collider);

            return entity;
        }

        private void AddLineRendererIfDebug(BoxCollider collider)
        {
            if (Data.Config.Debug.DrawEntityWireframes && UnityHelper.InEditor)
            {
                // TODO: Broken due to Z-depth issues... shader issue?
                Vector3[] points = collider.GetEdgeLinePoints();
                LineRenderer lineRenderer = collider.gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = Constants.MapUnit;
                lineRenderer.endWidth = Constants.MapUnit;
                lineRenderer.positionCount = points.Length;
                lineRenderer.SetPositions(points);
            }
        }
    }
}
