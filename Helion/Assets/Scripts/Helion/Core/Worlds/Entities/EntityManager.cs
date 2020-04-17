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
using Helion.Core.Worlds.Geometry.Bsp;
using UnityEngine;

namespace Helion.Core.Worlds.Entities
{
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

                GameObject entityObject = new GameObject($"{definition.Name}");
                entityCollectorGameObject.SetChild(entityObject);

                // Note: The definition should be set immediately on the entity
                // as other code relies on it. We should consider restructuring
                // this so the dependency is not required.
                Entity entity = entityObject.AddComponent<Entity>();
                entity.SetDefinition(actorDefinition.Value);
                entity.entityNode = Entities.AddLast(entity);

                // TODO: Does spawning this cause collision detection if it ends up at the origin?
                float height = entity.Definition.Properties.Height;
                float diameter = entity.Definition.Properties.Radius * 2;
                float y = geometry.BspTree.Sector(thing.Position).FloorPlane.Height;
                Vector3 position = new Vector3(thing.Position.x, y, thing.Position.y);

                BoxCollider collider = entityObject.AddComponent<BoxCollider>();
                collider.center = new Vector3(position.x, position.y + (height / 2), position.z) * Constants.MapUnit;
                collider.size = new Vector3(diameter, height, diameter) * Constants.MapUnit;
                AddLineRendererIfDebug(collider);
            }
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
