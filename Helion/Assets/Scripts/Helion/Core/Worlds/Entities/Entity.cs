using System;
using System.Collections.Generic;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Util.Extensions;
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
        public ActorDefinition Definition;

        /// <summary>
        /// This is the position in world coordinates (not Unity coordinates).
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The previous position in world coordinates (not Unity coordinates).
        /// Used for interpolation purposes.
        /// </summary>
        public Vector3 PrevPosition;

        /// <summary>
        /// The velocity in world coordinates (not Unity coordinates).
        /// </summary>
        public Vector3 Velocity;

        internal LinkedListNode<Entity> entityNode;
        internal World world;

        void Update()
        {
            // TODO
        }

        void FixedUpdate()
        {
            Tick();
        }

        public Vector3 InterpolatedPosition(float fraction)
        {
            Vector3 pos = Vector3.Lerp(PrevPosition, Position, fraction);
            pos.y += 42; // TODO: View height here!
            return pos;
        }

        public void SetPosition(Vector3 position)
        {
            PrevPosition = Position;
            Position = position;
        }

        public void Tick()
        {
            PrevPosition = Position;
        }

        public void Dispose()
        {
            Destroy(gameObject);
            entityNode.List.Remove(entityNode);
        }
    }
}
