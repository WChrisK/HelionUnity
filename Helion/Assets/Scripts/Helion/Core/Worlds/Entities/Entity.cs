using System;
using System.Collections.Generic;
using Helion.Core.Resource.Decorate.Definitions;
using UnityEngine;

namespace Helion.Core.Worlds.Entities
{
    /// <summary>
    /// Holds data for the entity in a world.
    /// </summary>
    public class Entity : MonoBehaviour, ITickable, IDisposable
    {
        /// <summary>
        /// The definition that makes up this entity.
        /// </summary>
        public ActorDefinition Definition { get; private set; }

        /// <summary>
        /// This is the position in world coordinates (not Unity coordinates).
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The velocity in world coordinates (not Unity coordinates).
        /// </summary>
        public Vector3 Velocity;

        internal LinkedListNode<Entity> entityNode;

        public GameObject GameObject => transform.parent.gameObject;

        void Update()
        {
            // TODO
        }

        void FixedUpdate()
        {
            Tick();
        }

        public void Tick()
        {
            // TODO
        }

        public void Dispose()
        {
            Destroy(GameObject);
            entityNode.List.Remove(entityNode);
        }

        public void SetDefinition(ActorDefinition definition)
        {
            Definition = definition;
        }
    }
}
