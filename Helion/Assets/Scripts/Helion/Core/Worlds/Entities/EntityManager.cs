using System.Collections.Generic;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using UnityEngine;

namespace Helion.Core.Worlds.Entities
{
    public class EntityManager
    {
        public readonly LinkedList<Entity> Entities = new LinkedList<Entity>();
        private readonly GameObject entityCollectorGameObject;

        public EntityManager(GameObject parentObject, IMap map)
        {
            entityCollectorGameObject = new GameObject("Entities");
            parentObject.SetChild(entityCollectorGameObject);

            CreateEntities(((DoomMap)map).Things);
        }

        private void CreateEntities(IEnumerable<DoomThing> things)
        {
            foreach (DoomThing thing in things)
            {
                GameObject entityObject = new GameObject($"Entity UNKNOWN ({thing.EditorNumber})");
                entityCollectorGameObject.SetChild(entityObject);

                // TODO: Need to use the BSP for the Y coordinate base, and decorate for the bbox.
                // TODO: Note this is the center, not the Y-bottom center foot location.
                // TODO: Does spawning this cause collision detection if it ends up at the origin?
                BoxCollider collider = entityObject.AddComponent<BoxCollider>();
                collider.center = new Vector3(thing.Position.x, 32, thing.Position.y) * Constants.MapUnit;
                collider.size = new Vector3(32, 32, 32) * Constants.MapUnit;

                Entity entity = entityObject.AddComponent<Entity>();
                entity.entityNode = Entities.AddLast(entity);
            }
        }
    }
}
