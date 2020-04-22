using System;
using System.Collections.Generic;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Util.Geometry;
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

        public BitAngle Angle;

        // TODO: The following sucks, we may need to extract this out so we can get a constructor...
        internal LinkedListNode<Entity> entityNode;
        internal World world;
        internal FrameTracker frameTracker;

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

            int rotation = 0; //BitAngle.CalculateSpriteRotation(this, world.ConsolePlayerEntity);
            meshRenderer.sharedMaterial = frameTracker.Frame.SpriteRotations[rotation];

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (!meshFilter)
                return;
        }
    }
}
