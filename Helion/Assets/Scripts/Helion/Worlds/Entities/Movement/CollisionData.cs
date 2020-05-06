using System;
using Helion.Unity;
using Helion.Util.Geometry.Boxes;
using Helion.Worlds.Geometry.Subsectors;
using Helion.Worlds.Geometry.Walls;
using UnityEngine;

namespace Helion.Worlds.Entities.Movement
{
    public class CollisionData
    {
        internal const int MaxColliders = 64;

        public int WallCount { get; private set; }
        public int PlaneCount { get; private set; }
        public int BlockingEntityCount { get; private set; }
        public int NonBlockingEntityCount { get; private set; }
        public readonly Wall[] Walls;
        public readonly SubsectorPlane[] Planes;
        public readonly Entity[] BlockingEntities;
        public readonly Entity[] NonBlockingEntities;
        internal int collisionCount { get; private set; }
        internal readonly Collider[] colliders;

        public int BlockingCount => WallCount + PlaneCount + BlockingEntityCount;
        public bool HasNone => BlockingCount == 0;

        public CollisionData()
        {
            colliders = new Collider[MaxColliders];
            Walls = new Wall[MaxColliders];
            Planes = new SubsectorPlane[MaxColliders];
            BlockingEntities = new Entity[MaxColliders];
            NonBlockingEntities = new Entity[MaxColliders];
        }

        public CollisionData(Collider[] allColliders, Entity entity, in Box3F box)
        {
            colliders = allColliders;
            collisionCount = allColliders.Length;

            // Because we don't know how many of each element there is until
            // we do the partitions, we'll just allocate space for the worst
            // case since this constructor should only be very rarely called.
            Walls = new Wall[collisionCount];
            Planes = new SubsectorPlane[collisionCount];
            BlockingEntities = new Entity[collisionCount];
            NonBlockingEntities = new Entity[collisionCount];

            PartitionCollisions(entity, box);
        }

        /// <summary>
        /// Uses a static allocator to find collisions. The center point and
        /// the box should overlap if they were in the same coordinate space
        /// (as center/halfExtents are in Unity map units, and the box is in
        /// world units).
        /// </summary>
        /// <param name="center">The center point in Unity map units.</param>
        /// <param name="halfExtents">The box extends in Unity map units.
        /// </param>
        /// <param name="entity">The entity that is to collide with things.
        /// </param>
        /// <param name="box">The bounding box at the position to check.
        /// </param>
        public void Populate(Vector3 center, Vector3 halfExtents, Entity entity, in Box3F box)
        {
            collisionCount = Physics.OverlapBoxNonAlloc(center, halfExtents, colliders);
            PartitionCollisions(entity, box);
        }

        private void PartitionCollisions(Entity entity, in Box3F box)
        {
            WallCount = 0;
            PlaneCount = 0;
            BlockingEntityCount = 0;
            NonBlockingEntityCount = 0;

            for (int i = 0; i < collisionCount; i++)
            {
                CollisionInfo collisionInfo = colliders[i].gameObject.GetComponent<CollisionInfo>();
                Debug.Assert(collisionInfo != null, "Should not collide with something that has no collision info");

                switch (collisionInfo.InfoType)
                {
                case CollisionInfoType.Entity:
                    if (ReferenceEquals(entity, collisionInfo.Entity))
                        continue;
                    if (entity.IsBlockedBy(collisionInfo.Entity))
                        BlockingEntities[BlockingEntityCount++] = collisionInfo.Entity;
                    else
                        NonBlockingEntities[NonBlockingEntityCount++] = collisionInfo.Entity;
                    break;

                case CollisionInfoType.SubsectorPlane:
                    if (collisionInfo.SubsectorPlane.IntersectedBy(box))
                        Planes[PlaneCount++] = collisionInfo.SubsectorPlane;
                    break;

                case CollisionInfoType.Wall:
                    if (collisionInfo.Wall.IntersectedBy(box))
                        Walls[WallCount++] = collisionInfo.Wall;
                    break;

                default:
                    throw new Exception($"Unsupported collision info type: {collisionInfo.InfoType}");
                }
            }
        }
    }
}
