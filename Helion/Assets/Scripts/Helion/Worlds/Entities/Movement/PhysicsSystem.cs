using Helion.Util.Extensions;
using Helion.Util.Geometry.Boxes;
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
            (float x, float y, float z) = entity.Velocity;
            if (x.ApproxZero(MinMovementThreshold))
                x = 0;
            if (y.ApproxZero(MinMovementThreshold))
                y = 0;
            if (z.ApproxZero(MinMovementThreshold))
                z = 0;

            entity.Velocity = new Vec3F(x, y, z);
        }

        /// <summary>
        /// Finds all the collisions for some entity at a position/box.
        /// </summary>
        /// <param name="entity">The entity that is to collide with things.
        /// </param>
        /// <param name="position">The center point, which should also be the
        /// bottom center of the box (at the feet).</param>
        /// <param name="box">The bounding box at the position to check.
        /// </param>
        /// <returns>The things collided with. This should never be mutated.
        /// </returns>
        private CollisionData FindCollisions(Entity entity, in Vec3F position, in Box3F box)
        {
            float radius = entity.Radius;
            float halfHeight = entity.HalfHeight;
            Vector3 center = new Vec3F(position.X, position.Y + halfHeight, position.Z).MapUnit();
            Vector3 halfExtents = new Vector3(radius, halfHeight, radius).MapUnit();

            collisionData.Populate(center, halfExtents, entity, box);

            // If we didn't run up to the end of the array, then we can exit
            // early without having to do more checks.
            if (collisionData.collisionCount != CollisionData.MaxColliders)
                return collisionData;

            // If we did run into a potential overflow, then we'll take the GC
            // allocation hit and figure out all of the collisions. This should
            // be very rare so it is okay if it adds a bit of GC pressure.
            Collider[] allColliders = Physics.OverlapBox(center, halfExtents);
            return new CollisionData(allColliders, entity, box);
        }

        /// <summary>
        /// Checks if we can move to the position at 'next position'. Populates
        /// a reference with collision data, which should not be mutated.
        /// </summary>
        /// <param name="entity">The entity that is trying to move.</param>
        /// <param name="nextPosition">The center bottom of where we want to
        /// move to.</param>
        /// <param name="nextBox">The collision box for the entity. This should
        /// also have its bottom center at the exact same point as the next
        /// position vector.</param>
        /// <param name="collisions">The output with the found collisions.
        /// </param>
        /// <returns>True if we can move to the spot without any collisions,
        /// false if there was one or more blockers.</returns>
        private bool CanMoveTo(Entity entity, in Vec3F nextPosition, in Box3F nextBox, out CollisionData collisions)
        {
            collisions = FindCollisions(entity, nextPosition, nextBox);
            return collisions.HasNone;
        }

        /// <summary>
        /// Performs the actual move to the position provided. This will update
        /// the entity and do any item pickups.
        /// </summary>
        /// <param name="entity">The entity to move to the position.</param>
        /// <param name="position">The bottom center position to move the
        /// entity to.</param>
        /// <param name="collisions">The collision info from the call to
        /// <see cref="CanMoveTo"/>.</param>
        private void MoveTo(Entity entity, in Vec3F position, CollisionData collisions)
        {
            entity.SetPosition(position);

            if (entity.Flags.Pickup)
                PerformItemPickups(entity, collisions);
        }

        private void PerformItemPickups(Entity entity, CollisionData collisions)
        {
            for (int i = 0; i < collisions.NonBlockingEntityCount; i++)
            {
                Entity nonBlockingEntity = collisions.NonBlockingEntities[i];
                if (nonBlockingEntity.Definition.ActorType.Inventory)
                {
                    if (entity.TryPickup(nonBlockingEntity))
                    {
                        // TODO: Should we remove them from the `collisions` object as well?
                        nonBlockingEntity.Dispose();
                    }
                }
            }
        }
    }
}
