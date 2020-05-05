using System;
using Helion.Unity;
using Helion.Util.Geometry.Vectors;
using UnityEngine;

namespace Helion.Worlds.Entities.Movement
{
    /// <summary>
    /// Responsible for doing all of the physics calculations.
    /// </summary>
    /// <remarks>
    /// This is not thread safe! The `colliders` value is shared for each call
    /// made to it.
    /// </remarks>
    public class PhysicsSystem
    {
        public const float ColliderThickness = 0.02f;
        private const int MaxColliders = 256;
        private const float Friction = 0.90625f;
        private static readonly Vec3F FrictionXZScale = (Friction, 1, Friction);

        private readonly World world;
        private readonly Collider[] colliders = new Collider[MaxColliders];

        public PhysicsSystem(World world)
        {
            this.world = world;
        }

        public void TryMove(Entity entity)
        {
            if (entity.Velocity == Vec3F.Zero)
                return;

            TryMoveHorizontal(entity);
            TryMoveVertical(entity);

            ClampVelocity(entity);
        }

        private void TryMoveHorizontal(Entity entity)
        {
            Vec3F velocityXZ = entity.Velocity.WithY(0);
            Vec3F newPosition = entity.Position.Current + velocityXZ;

            Vector3 center = newPosition.MapUnit();
            Vector3 halfExtents = Vector3.one;
            int intersections = Physics.OverlapBoxNonAlloc(center, halfExtents, colliders);
            for (int i = 0; i < intersections; i++)
            {
                CollisionInfo collisionInfo = colliders[i].gameObject.GetComponent<CollisionInfo>();
                Debug.Assert(collisionInfo != null, "Should not collide with something that has no collision info");

                switch (collisionInfo.InfoType)
                {
                case CollisionInfoType.Entity:
                    Debug.Log("Hit entity");
                    break;
                case CollisionInfoType.SubsectorPlane:
                    Debug.Log("Hit floor");
                    break;
                case CollisionInfoType.Wall:
                    Debug.Log("Hit wall");
                    break;
                default:
                    throw new Exception($"Unsupported collision info type: {collisionInfo.InfoType}");
                }
            }

            if (intersections == 0)
                entity.SetPosition(newPosition);

            ApplyFriction(entity);
        }

        private void ApplyFriction(Entity entity)
        {
            entity.Velocity *= FrictionXZScale;
        }

        private void TryMoveVertical(Entity entity)
        {
            // TODO
        }

        private void ClampVelocity(Entity entity)
        {
            (float x, float y, float z) = entity.Velocity;

            if (x < 0.001f && x > -0.001f)
                x = 0;
            if (y < 0.001f && y > -0.001f)
                y = 0;
            if (z < 0.001f && z > -0.001f)
                z = 0;

            entity.Velocity = new Vec3F(x, y, z);
        }
    }
}
