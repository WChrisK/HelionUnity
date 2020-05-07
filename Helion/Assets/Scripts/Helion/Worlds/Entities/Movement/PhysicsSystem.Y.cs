using System;
using Helion.Util.Geometry.Boxes;
using Helion.Util.Geometry.Vectors;
using Helion.Worlds.Geometry;
using Helion.Worlds.Geometry.Subsectors;
using UnityEngine;

namespace Helion.Worlds.Entities.Movement
{
    public partial class PhysicsSystem
    {
        private const float Gravity = 1.0f;

        private bool HasNoVelocityY(Entity entity) => entity.Velocity.Y == 0;

        private void TryMoveY(Entity entity)
        {
            if (!entity.Flags.NoGravity)
                ApplyGravity(entity);

            if (HasNoVelocityY(entity))
                return;

            PerformMovesY(entity);
        }

        private void ApplyGravity(Entity entity)
        {
            float newY = entity.Velocity.Y - Gravity;
            entity.Velocity = entity.Velocity.WithY(newY);
        }

        private void PerformMovesY(Entity entity)
        {
            Vec3F position = entity.Position.Current;
            Vec3F velocityY = entity.Velocity.WithXZ(0, 0);
            Box3F box = entity.Box;

            int numMoves = CalculateStepsY(velocityY, entity.HalfHeight);
            Vec3F stepDelta = velocityY / numMoves;

            for (int movesLeft = numMoves; movesLeft > 0; movesLeft--)
            {
                if (stepDelta == Vec3F.Zero)
                    break;

                Vec3F nextPosition = position + stepDelta;
                Box3F nextBox = box + stepDelta;

                if (CanMoveTo(entity, nextPosition, nextBox, out CollisionData collisions))
                {
                    MoveTo(entity, nextPosition, collisions);
                    position = nextPosition;
                    box = nextBox;
                    continue;
                }

                SetToVerticalCollidedElementY(entity, position, collisions);
                ClearVelocityY(entity);
                break;
            }
        }

        private int CalculateStepsY(in Vec3F velocity, float halfHeight)
        {
            // Note: This means any objects of height 1 will have a fair amount
            // of steps. We may want to change this depending on the size.
            Debug.Assert(halfHeight > 0.45f, $"Cannot safely calculate moving steps for such a small half height ({halfHeight})");

            // We want to pick some atomic distance to keep moving our bounding
            // box. It can't be bigger than the radius because we could end up
            // skipping over a line.
            float moveDistance = halfHeight - 0.45f;
            float totalMoveDistance = Math.Abs(velocity.Y);
            return (int)(totalMoveDistance / moveDistance) + 1;
        }

        private void SetToVerticalCollidedElementY(Entity entity, in Vec3F lastPosition, CollisionData collisions)
        {
            // TODO: Needs to handle entities we land on.

            if (collisions.PlaneCount == 0)
                return;

            bool hitFloor = TryFindHighestFloorPlane(out SubsectorPlane highestFloor);
            bool hitCeiling = TryFindLowestCeilingPlane(out SubsectorPlane lowestCeiling);

            if (hitFloor)
            {
                Vec3F nextPosition = lastPosition.WithY(highestFloor.SectorPlane.Height);
                entity.SetPosition(nextPosition);
            }

            if (hitCeiling)
            {
                float y = lowestCeiling.SectorPlane.Height - entity.Height;
                Vec3F nextPosition = lastPosition.WithY(y);
                entity.SetPosition(nextPosition);
            }

            bool TryFindHighestFloorPlane(out SubsectorPlane floorPlane)
            {
                floorPlane = null;
                float highestFloorY = float.MinValue;

                for (int i = 0; i < collisions.PlaneCount; i++)
                {
                    SubsectorPlane subsectorPlane = collisions.Planes[i];
                    SectorPlane sectorPlane = subsectorPlane.SectorPlane;
                    if (sectorPlane.IsCeiling)
                        continue;

                    if (sectorPlane.Height > highestFloorY)
                    {
                        floorPlane = subsectorPlane;
                        highestFloorY = sectorPlane.Height;
                    }
                }

                return floorPlane != null;
            }

            bool TryFindLowestCeilingPlane(out SubsectorPlane ceilingPlane)
            {
                ceilingPlane = null;
                float lowestCeilingY = float.MaxValue;

                for (int i = 0; i < collisions.PlaneCount; i++)
                {
                    SubsectorPlane subsectorPlane = collisions.Planes[i];
                    SectorPlane sectorPlane = subsectorPlane.SectorPlane;
                    if (sectorPlane.IsFloor)
                        continue;

                    if (sectorPlane.Height < lowestCeilingY)
                    {
                        ceilingPlane = subsectorPlane;
                        lowestCeilingY = sectorPlane.Height;
                    }
                }

                return ceilingPlane != null;
            }
        }

        private void ClearVelocityY(Entity entity)
        {
            entity.Velocity = entity.Velocity.WithY(0);
        }
    }
}
