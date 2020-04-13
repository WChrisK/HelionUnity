using Helion.Core.Resource.Maps;
using Helion.Core.Util.Extensions;
using UnityEngine;

namespace Helion.Core.Worlds.Entities
{
    public class EntityManager
    {
        private GameObject gameObject;

        public EntityManager(GameObject parentObject, IMap map)
        {
            gameObject = new GameObject("Entities");
            parentObject.SetChild(gameObject);
        }
    }
}
