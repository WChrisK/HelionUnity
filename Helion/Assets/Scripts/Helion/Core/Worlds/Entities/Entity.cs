using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helion.Core.Worlds.Entities
{
    /// <summary>
    /// Holds data for the entity in a world.
    /// </summary>
    public class Entity : MonoBehaviour, IDisposable
    {
        public Vector3 Velocity;
        internal LinkedListNode<Entity> entityNode;

        public GameObject EntityGameObject => transform.parent.gameObject;

        void Update()
        {
            // TODO
        }

        void FixedUpdate()
        {
            // TODO
        }

        public void Dispose()
        {
            Destroy(EntityGameObject);
            entityNode.List.Remove(entityNode);
        }
    }
}
