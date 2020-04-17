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
        public ActorDefinition Definition { get; private set; }
        public Vector3 Velocity;
        internal LinkedListNode<Entity> entityNode;

        public GameObject EntityGameObject => transform.parent.gameObject;

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
            Destroy(EntityGameObject);
            entityNode.List.Remove(entityNode);
        }

        public void SetDefinition(ActorDefinition definition)
        {
            Definition = definition;
        }
    }
}
