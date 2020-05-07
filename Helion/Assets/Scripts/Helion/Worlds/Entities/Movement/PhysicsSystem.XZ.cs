using System;
using Helion.Util.Geometry.Boxes;
using Helion.Util.Geometry.Segments;
using Helion.Util.Geometry.Vectors;
using Helion.Worlds.Geometry.Walls;
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

        /// <summary>
        /// Takes care of all of the horizontal movement for the entity. This
        /// may adjust the Y value if there are steps or slopes however.
        /// </summary>
        /// <param name="entity">The entity to move.</param>
        private void PerformMovesXZ(Entity entity)
        {
            int slidesLeft = MaxSlides;
            Vec3F position = entity.Position.Current;
            Vec3F velocityXZ = entity.Velocity.WithY(0);
            Box3F box = entity.Box;

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
                Box3F nextBox = box + stepDelta;

                if (CanMoveTo(entity, nextPosition, nextBox, out CollisionData collisions))
                {
                    MoveTo(entity, nextPosition, collisions);
                    position = nextPosition;
                    box = nextBox;
                    continue;
                }

                if (slidesLeft > 0 && entity.Flags.Slides)
                {
                    HandleSlideXZ(entity, collisions, ref position, ref box, ref stepDelta, ref movesLeft);
                    slidesLeft--;
                    continue;
                }

                ClearVelocityXZ(entity);
                break;
            }
        }

        /// <summary>
        /// Calculates how many times we must break up the velocity such that
        /// we only move a small enough distance to prevent phasing through
        /// anything like walls.
        /// </summary>
        /// <param name="velocity">The velocity of the entity.</param>
        /// <param name="radius">The radius of the entity.</param>
        /// <returns>The number of steps that should be done.</returns>
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

        /// <summary>
        /// Performs a slide along an entity or wall.
        /// </summary>
        /// <param name="entity">The entity that is moving.</param>
        /// <param name="collisions">The collisions that occurred when we
        /// checked if we could move.</param>
        /// <param name="position">The current position the entity is known to
        /// be able to move to safely.</param>
        /// <param name="box">The box at the position (which that position is
        /// the bottom center).</param>
        /// <param name="stepDelta">The delta we move for each step.</param>
        /// <param name="movesLeft">How many step moves are left.</param>
        private void HandleSlideXZ(Entity entity, CollisionData collisions, ref Vec3F position,
            ref Box3F box, ref Vec3F stepDelta, ref int movesLeft)
        {
            if (TryFindClosestBlockingWall(entity, collisions, box, stepDelta, out Wall wall, out float t))
            {
                if (MoveCloseToBlockingWall(entity, stepDelta, ref position, ref box, t, out Vec3F residualStep))
                {
                    ReorientToSlideAlong(entity, wall, residualStep, ref stepDelta, ref movesLeft);
                    return;
                }
            }

            // If we can't find a blocking line, that means we either have a
            // jagged edge that our corner tracers couldn't find, or it was
            // some blocking entity. For compatibility reasons, we attempt to
            // move along the X/Z axes (with Z tried first for compatibility
            // reasons as well). This also handles the case where we are being
            // blocked by some entity.
            if (AttemptAxisMoveZ(entity, ref position, ref box, ref stepDelta))
                return;
            if (AttemptAxisMoveX(entity, ref position, ref box, ref stepDelta))
                return;

            // If we cannot find the line or thing that is blocking us, then we
            // are fully done moving along the XZ direction.
            ClearVelocityXZ(entity);
            stepDelta = Vec3F.Zero;
            movesLeft = 0;
        }

        /// <summary>
        /// Searches for the closest blocking wall to tell if we are to slide
        /// or not.
        /// </summary>
        /// <param name="entity">The entity that is moving.</param>
        /// <param name="collisions">The collisions we found when trying to
        /// move originally.</param>
        /// <param name="box">The box around the entity at the position to
        /// evaluate.</param>
        /// <param name="stepDelta">The delta we move for each step.</param>
        /// <param name="wall">The closest wall we found, or null if this
        /// returns false.</param>
        /// <param name="t">The time along the stepDelta to which the
        /// intersection occurs.</param>
        /// <returns>True if we found a blocking wall, false if not. If false
        /// then the out parameters should not be used.</returns>
        private bool TryFindClosestBlockingWall(Entity entity, CollisionData collisions, in Box3F box,
            in Vec3F stepDelta, out Wall wall, out float t)
        {
            wall = null;
            t = float.MaxValue;

            // We shoot out 3 tracers from the corners in the direction we're
            // travelling to see if there's a blocking line as follows:
            //    _  _
            //    /| /|   If we're travelling northeast, then from the
            //   /  /_    top right corners of the bounding box we will
            //  o--o /|   shoot out tracers in the direction we are going
            //  |  |/     to step to see if we hit anything
            //  o--o
            //
            // This obviously can miss things, but this is how vanilla does it
            // and we want to have compatibility with the mods that rely on it.
            BoxCornerTracers tracers = CalculateCornerTracers(box.XZ, stepDelta);
            CheckCornerTracerIntersection(entity, tracers.First, collisions, ref wall, ref t);
            CheckCornerTracerIntersection(entity, tracers.Second, collisions, ref wall, ref t);
            CheckCornerTracerIntersection(entity, tracers.Third, collisions, ref wall, ref t);

            return t < float.MaxValue;
        }

        /// <summary>
        /// Calculates the corner tracers that should be used when scanning for
        /// any walls.
        /// </summary>
        /// <param name="box">The box to shoot the tracers out of.</param>
        /// <param name="stepDelta">The direction we're stepping.</param>
        /// <returns>The corner deltas to evaluate.</returns>
        private BoxCornerTracers CalculateCornerTracers(in Box2F box, in Vec3F stepDelta)
        {
            // We do it this way so we do zero allocations and that everything
            // stays on the stack.
            Vec2F firstCorner;
            Vec2F secondCorner;
            Vec2F thirdCorner;

            if (stepDelta.X >= 0)
            {
                if (stepDelta.Z >= 0)
                {
                    firstCorner = box.TopLeft;
                    secondCorner = box.TopRight;
                    thirdCorner = box.BottomRight;
                }
                else
                {
                    firstCorner = box.TopRight;
                    secondCorner = box.BottomRight;
                    thirdCorner = box.BottomLeft;
                }
            }
            else
            {
                if (stepDelta.Z >= 0)
                {
                    firstCorner = box.TopRight;
                    secondCorner = box.TopLeft;
                    thirdCorner = box.BottomLeft;
                }
                else
                {
                    firstCorner = box.TopLeft;
                    secondCorner = box.BottomLeft;
                    thirdCorner = box.BottomRight;
                }
            }

            Vec2F stepDeltaXZ = stepDelta.XZ;
            Seg2F first = new Seg2F(firstCorner, firstCorner + stepDeltaXZ);
            Seg2F second = new Seg2F(secondCorner, secondCorner + stepDeltaXZ);
            Seg2F third = new Seg2F(thirdCorner, thirdCorner + stepDeltaXZ);
            return new BoxCornerTracers(first, second, third);
        }

        /// <summary>
        /// Checks for intersections from the corner by firing out the tracers
        /// from the box corners to see if it hits anything.
        /// </summary>
        /// <param name="entity">The entity that is moving.</param>
        /// <param name="tracer">The tracer to check against. This should
        /// originate from the corner of the box where the entity has been
        /// last cleared to safely move to.</param>
        /// <param name="collisions">The collision data found when originally
        /// trying to move.</param>
        /// <param name="wall">The wall to set if it is closer than the
        /// earliest intersection.</param>
        /// <param name="t">The closest found time. Will be set with a closer
        /// time if one is found.</param>
        private void CheckCornerTracerIntersection(Entity entity, in Seg2F tracer,
            CollisionData collisions, ref Wall wall, ref float t)
        {
            for (int i = 0; i < collisions.WallCount; i++)
            {
                Wall collidedWall = collisions.Walls[i];

                if (tracer.Intersection(collidedWall.Line.Segment, out float intersectionTime) &&
                    intersectionTime < t)
                {
                    t = intersectionTime;
                    wall = collidedWall;
                }
            }
        }

        private bool MoveCloseToBlockingWall(Entity entity, in Vec3F stepDelta, ref Vec3F position,
            ref Box3F box, float t, out Vec3F residualStep)
        {
            // If it's close enough that stepping back would move us further
            // back than we currently are (or move us nowhere), we don't need
            // to do anything. This also means the residual step is equal to
            // the entire step since we're not stepping anywhere.
            if (t <= SlideStepBackTime)
            {
                residualStep = stepDelta;
                return true;
            }

            t -= SlideStepBackTime;
            Vec3F usedStepDelta = stepDelta * t;
            residualStep = stepDelta - usedStepDelta;

            Vec3F nextPosition = position + usedStepDelta;
            Box3F nextBox = box + usedStepDelta;

            if (CanMoveTo(entity, nextPosition, nextBox, out CollisionData collisions))
            {
                MoveTo(entity, nextPosition, collisions);
                position = nextPosition;
                box = nextBox;
                return true;
            }

            // Something else is blocking us from moving up to the wall, like
            // an entity or maybe even a plane.
            residualStep = Vec3F.Zero;
            return false;
        }

        private void ReorientToSlideAlong(Entity entity, Wall wall, in Vec3F residualStep,
            ref Vec3F stepDelta, ref int movesLeft)
        {
            Vec2F stepDeltaXZ = stepDelta.XZ;

            // Our slide direction depends on if we're going along with the
            // line or against the line. If the dot product is negative, it
            // means we are facing away from the line and should slide in
            // the opposite direction from the way the line is pointing.
            Vec2F unitDirection = wall.Line.Segment.Delta.Unit();
            if (stepDeltaXZ.Dot(unitDirection) < 0)
                unitDirection = -unitDirection;

            // Because we moved up to the wall, it's almost always the case
            // that we didn't make 100% of a step. For example if we have some
            // movement of 5 map units towards a wall and run into the wall at
            // 3 (leaving 2 map units unhandled), we want to work that residual
            // map unit movement into the existing step length. The following
            // does that by finding the total movement scalar and applying it
            // to the direction we need to slide.
            //
            // We also must take into account that we're adding some scalar to
            // another scalar, which means we'll end up with usually a larger
            // one. This means our step delta could grow beyond the size of the
            // radius of the entity and cause it to skip lines in pathological
            // situations. I haven't encountered such a case yet but it is at
            // least theoretically possible this can happen. Because of this,
            // the movesLeft is incremented by 1 to make sure the stepDelta
            // at the end of this function stays smaller than the radius.
            // TODO: If we have the unit vector, is projection overkill? Can we just multiply by the component instead?
            Vec2F stepProjection = stepDeltaXZ.Projection(unitDirection);
            Vec2F residualProjection = residualStep.XZ.Projection(unitDirection);

            // TODO: This is almost surely not how it's done... but it feels okay.
            // TODO: Eventually would need to use sector friction, not a hardcoded value.
            // TODO: Why multiply friction? Won't this just cause friction to be applied twice? (now, then later as well?)
            entity.Velocity = entity.Velocity.WithXZ(stepProjection * Friction);

            float totalRemainingDistance = ((stepProjection * movesLeft) + residualProjection).Length();
            movesLeft += 1;

            Vec2F newStepDelta = unitDirection * totalRemainingDistance / movesLeft;
            stepDelta = stepDelta.WithXZ(newStepDelta);
        }

        private bool AttemptAxisMoveX(Entity entity, ref Vec3F position, ref Box3F box, ref Vec3F stepDelta)
        {
            return AttemptAxisMove(entity, ref position, ref box, ref stepDelta, true);
        }

        private bool AttemptAxisMoveZ(Entity entity, ref Vec3F position, ref Box3F box, ref Vec3F stepDelta)
        {
            return AttemptAxisMove(entity, ref position, ref box, ref stepDelta, false);
        }

        private bool AttemptAxisMove(Entity entity, ref Vec3F position, ref Box3F box,
            ref Vec3F stepDelta, bool isAxisX)
        {
            if (isAxisX)
            {
                Vec3F deltaX = new Vec3F(stepDelta.X, 0, 0);
                Vec3F nextPosition = position + deltaX;
                Box3F nextBox = box + deltaX;

                if (CanMoveTo(entity, nextPosition, nextBox, out CollisionData collisions))
                {
                    MoveTo(entity, nextPosition, collisions);
                    position = nextPosition;
                    box = nextBox;

                    entity.Velocity = entity.Velocity.WithZ(0);
                    stepDelta = stepDelta.WithZ(0);

                    return true;
                }
            }
            else
            {
                Vec3F deltaZ = new Vec3F(0, 0, stepDelta.Z);
                Vec3F nextPosition = position + deltaZ;
                Box3F nextBox = box + deltaZ;

                if (CanMoveTo(entity, nextPosition, nextBox, out CollisionData collisions))
                {
                    MoveTo(entity, nextPosition, collisions);
                    position = nextPosition;
                    box = nextBox;

                    entity.Velocity = entity.Velocity.WithX(0);
                    stepDelta = stepDelta.WithX(0);

                    return true;
                }
            }

            return false;
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
