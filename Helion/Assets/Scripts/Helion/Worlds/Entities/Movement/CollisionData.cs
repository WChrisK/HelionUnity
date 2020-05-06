using System;
using Helion.Unity;
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

        public CollisionData(Collider[] allColliders, Entity entity)
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

            PartitionCollisions(entity);
        }

        public void Populate(Vector3 center, Vector3 halfExtents, Entity entity)
        {
            collisionCount = Physics.OverlapBoxNonAlloc(center, halfExtents, colliders);
            PartitionCollisions(entity);
        }

        private void PartitionCollisions(Entity entity)
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
                    // TODO: Perform more accurate check now.
                    Planes[PlaneCount++] = collisionInfo.SubsectorPlane;
                    break;

                case CollisionInfoType.Wall:
                    // TODO: Check that we actually cross the line.
                    // TODO: Check that we aren't touching (PhysX considers touching a collision).
                    Walls[WallCount++] = collisionInfo.Wall;
                    break;

                default:
                    throw new Exception($"Unsupported collision info type: {collisionInfo.InfoType}");
                }
            }
        }
    }
}
