using System.Collections.Generic;
using Helion.Core.Resource;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Unity;
using Helion.Core.Worlds.Geometry;
using Helion.Core.Worlds.Geometry.Bsp;
using UnityEngine;

namespace Helion.Core.Worlds.Entities
{
    public class EntityManager
    {
        public readonly LinkedList<Entity> Entities = new LinkedList<Entity>();
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
            BspTree bspTree = geometry.BspTree;

            foreach (DoomThing thing in things)
            {
                GameObject entityObject = new GameObject($"Entity UNKNOWN ({thing.EditorNumber})");
                entityCollectorGameObject.SetChild(entityObject);

                float y = bspTree.Sector(thing.Position).FloorPlane.Height;
                float height = 32;

                Entity entity = entityObject.AddComponent<Entity>();
                entity.entityNode = Entities.AddLast(entity);

                // TODO: Need to use the BSP for the Y coordinate base, and decorate for the bbox.
                // TODO: Note this is the center, not the Y-bottom center foot location.
                // TODO: Does spawning this cause collision detection if it ends up at the origin?
                BoxCollider collider = entityObject.AddComponent<BoxCollider>();
                collider.center = new Vector3(thing.Position.x, y + (height / 2), thing.Position.y) * Constants.MapUnit;
                collider.size = new Vector3(32, height, 32) * Constants.MapUnit;

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
}
