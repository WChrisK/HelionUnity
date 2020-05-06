using System;
using Helion.Util.Geometry.Vectors;
using UnityEngine;

namespace Helion.Worlds.Entities.Movement
{
    /// <summary>
    /// This partial file contains all the information for the XZ movement.
    /// </summary>
    public partial class PhysicsSystem
    {
        private const int MaxSlides = 3;
        private const float Friction = 0.90625f;
        private const float SlideStepBackTime = 1.0f / 32.0f;
        private static readonly Vec3F FrictionXZScale = (Friction, 1, Friction);

        private static bool HasNoVelocityXZ(Entity entity) => entity.Velocity.XZ == Vec2F.Zero;

        private void TryMoveXZ(Entity entity)
        {
            if (HasNoVelocityXZ(entity))
                return;

            PerformMovesXZ(entity);
            ApplyFrictionXZ(entity);
        }

        private void PerformMovesXZ(Entity entity)
        {
            int slidesLeft = MaxSlides;
            Vec3F position = entity.Position.Current;
            Vec3F velocityXZ = entity.Velocity.WithY(0);

            // We advance in small steps that are smaller than the radius of
            // the actor so we don't skip over any lines or things due to fast
            // entity speed.
            int numMoves = CalculateStepsXZ(velocityXZ, entity.Radius);
            Vec3F stepDelta = velocityXZ / numMoves;

            for (int movesLeft = numMoves; movesLeft > 0; movesLeft--)
            {
                if (stepDelta == Vec3F.Zero)
                    break;

                Vec3F nextPosition = position + stepDelta;
                CollisionData collisions = FindCollisions(entity, nextPosition);

                if (collisions.HasNone)
                {
                    position = nextPosition;
                    continue;
                }

                if (slidesLeft > 0 && entity.Flags.Slides)
                {
                    HandleSlideXZ(entity, ref position, ref stepDelta, ref movesLeft);
                    slidesLeft--;
                    continue;
                }

                ClearVelocityXZ(entity);
                break;
            }

            entity.SetPosition(position);
        }

        private int CalculateStepsXZ(in Vec3F velocity, float radius)
        {
            Debug.Assert(radius > 0.5f, $"Cannot safely calculate moving steps for such a small radius ({radius})");

            // We want to pick some atomic distance to keep moving our bounding
            // box. It can't be bigger than the radius because we could end up
            // skipping over a line.
            float moveDistance = radius - 0.5f;
            float biggerAxis = Math.Max(Math.Abs(velocity.X), Math.Abs(velocity.Z));
            return (int)(biggerAxis / moveDistance) + 1;
        }

        private void HandleSlideXZ(Entity entity, ref Vec3F position, ref Vec3F stepDelta, ref int movesLeft)
        {
            // TODO
        }

        private void ClearVelocityXZ(Entity entity)
        {
            entity.Velocity = Vec3F.Zero.WithY(entity.Velocity.Y);
        }

        private void ApplyFrictionXZ(Entity entity)
        {
            entity.Velocity *= FrictionXZScale;
        }
    }
}
