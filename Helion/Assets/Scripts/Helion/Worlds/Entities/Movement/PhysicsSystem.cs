using Helion.Util.Extensions;
using Helion.Util.Geometry.Vectors;
using Helion.Util.Unity;
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
    public partial class PhysicsSystem
    {
        public const float ColliderThickness = 0.02f;
        private const float MinMovementThreshold = 0.06f;

        /// <summary>
        /// This is a cached version that is used repeatedly so that we have no
        /// GC allocations. As such, this makes it not thread-safe.
        /// </summary>
        private readonly CollisionData collisionData = new CollisionData();
        private readonly World world;

        public PhysicsSystem(World world)
        {
            this.world = world;
        }

        public void TryMove(Entity entity)
        {
            TryMoveXZ(entity);
            TryMoveY(entity);

            ClampVelocity(entity);
        }

        private void ClampVelocity(Entity entity)
        {
            // TODO: This does not create an intermediate object on the heap, right?
            (float x, float y, float z) = entity.Velocity;

            if (x.ApproxZero(MinMovementThreshold))
                x = 0;
            if (y.ApproxZero(MinMovementThreshold))
                y = 0;
            if (z.ApproxZero(MinMovementThreshold))
                z = 0;

            entity.Velocity = new Vec3F(x, y, z);
        }

        private CollisionData FindCollisions(Entity entity, in Vec3F position)
        {
            float radius = entity.Radius;
            float halfHeight = entity.HalfHeight;
            Vector3 center = new Vec3F(position.X, position.Y + halfHeight, position.Z).MapUnit();
            Vector3 halfExtents = new Vector3(radius, halfHeight, radius).MapUnit();

            Vector3 min = center - halfExtents;
            Vector3 max = center + halfExtents;
            Debug.Log($"[{world.GameTick}] Checking {min.x} {min.y} {min.z} -> {max.x} {max.y} {max.z}");
            collisionData.Populate(center, halfExtents, entity);

            // If we didn't run up to the end of the array, then we can exit
            // early without having to do more checks.
            if (collisionData.collisionCount != CollisionData.MaxColliders)
                return collisionData;

            // If we did run into a potential overflow, then we'll take the GC
            // allocation hit and figure out all of the collisions. This should
            // be very rare so it is okay if it adds a bit of GC pressure.
            Collider[] allColliders = Physics.OverlapBox(center, halfExtents);
            return new CollisionData(allColliders, entity);
        }
    }
}
