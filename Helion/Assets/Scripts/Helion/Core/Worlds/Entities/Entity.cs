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
        /// A unique ID for the entity.
        /// </summary>
        public int ID;

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

        /// <summary>
        /// The bit angle, where 0 is east, 65536/4 is north, 65536/2 is west,
        /// and 65536*3/4 is south.
        /// </summary>
        public ushort AngleBits;

        // TODO: The following sucks, we may need to extract this out so we can get a constructor...
        internal LinkedListNode<Entity> entityNode;
        internal World world;
        internal FrameTracker frameTracker;

        /// <summary>
        /// Gets the angle but in radians. East in the world (or the positive X
        /// axis) is considered zero and this rotates counter-clockwise if we
        /// were looking down along the X/Z plane in a birds eye view.
        /// </summary>
        public float AngleRadians => AngleBits * Mathf.PI / ushort.MaxValue;

        void Update()
        {
            UpdateSpriteMesh();
        }

        void FixedUpdate()
        {
            Tick();
        }

        public Vector3 InterpolatedPosition(float fraction)
        {
            return Vector3.Lerp(PrevPosition, Position, fraction);
        }

        public void SetPosition(Vector3 position)
        {
            PrevPosition = Position;
            Position = position;
        }

        public void Tick()
        {
            PrevPosition = Position;

            frameTracker.Tick();
        }

        public void Dispose()
        {
            Destroy(gameObject);
            entityNode.List.Remove(entityNode);
        }

        private void UpdateSpriteMesh()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (!meshRenderer)
                return;

            meshRenderer.sharedMaterial = frameTracker.Frame.SpriteRotations[0];

            // MeshFilter meshFilter = GetComponent<MeshFilter>();
            // if (!meshFilter)
            //     return;
        }
    }
}
