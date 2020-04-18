using System.Collections.Generic;
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
    public class EntityManager
    {
        private static readonly Log Log = LogManager.Instance();

        public readonly LinkedList<Entity> Entities = new LinkedList<Entity>();
        public readonly SpawnPoints SpawnPoints = new SpawnPoints();
        private readonly GameObject entityCollectorGameObject;
        private readonly MapGeometry geometry;

        public EntityManager(GameObject parentObject, MapGeometry mapGeometry, IMap map)
        {
            geometry = mapGeometry;

            entityCollectorGameObject = new GameObject("Entities");
            parentObject.SetChild(entityCollectorGameObject);

            CreateEntities(((DoomMap)map).Things);
        }

        public Optional<Entity> SpawnPlayer(int playerNumber)
        {
            Optional<ActorDefinition> actorDefinition = Data.Decorate.Find("DOOMPLAYER");
            if (!actorDefinition)
                return Optional<Entity>.Empty();

            Vector2? position = SpawnPoints.Coop(playerNumber);
            if (position == null)
                return Optional<Entity>.Empty();

            Entity entity = CreateEntity(actorDefinition.Value, position.Value);

            CharacterController charController = entity.GameObject.AddComponent<CharacterController>();
            charController.height = entity.Definition.Properties.Height;
            charController.radius = entity.Definition.Properties.Radius;
            charController.center = entity.Position + new Vector3(0, entity.Definition.Properties.Height - 16, 0);
            charController.stepOffset = 24.MapUnit();

            return entity;
        }

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
            entity.SetDefinition(definition);
            entity.entityNode = Entities.AddLast(entity);

            // TODO: Does spawning this cause collision detection if it ends up at the origin?
            float height = entity.Definition.Properties.Height;
            float diameter = entity.Definition.Properties.Radius * 2;
            float y = geometry.FloorHeight(position);
            Vector3 worldPos = new Vector3(position.x, y, position.y);
            entity.Position = worldPos;

            BoxCollider collider = entityObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(worldPos.x, worldPos.y + (height / 2), worldPos.z).MapUnit();
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
