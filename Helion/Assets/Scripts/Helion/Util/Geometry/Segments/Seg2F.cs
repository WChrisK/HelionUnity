using System;
using System.Diagnostics.Contracts;
using Helion.Util.Extensions;
using Helion.Util.Geometry.Boxes;
using Helion.Util.Unity;
using UnityEngine;

namespace Helion.Util.Geometry.Segments
{
    /// <summary>
    /// The stack-based segment that is quick to create without any GC pressure
    /// and can be used for various line tasks.
    /// </summary>
    public struct Seg2F
    {
        /// <summary>
        /// The beginning point of the segment.
        /// </summary>
        public readonly Vector2 Start;

        /// <summary>
        /// The ending point of the segment.
        /// </summary>
        public readonly Vector2 End;

        /// <summary>
        /// The difference between the start to the end. This means that
        /// Start + Delta = End.
        /// </summary>
        public readonly Vector2 Delta;

        /// <summary>
        /// The inversed components of the delta.
        /// </summary>
        public Vector2 DeltaInverse => new Vector2(1.0f / Delta.x, 1.0f / Delta.y);

        /// <summary>
        /// The bounding box of this segment.
        /// </summary>
        public Box2F Box => MakeBox(Start, End);

        /// <summary>
        /// The direction this segment goes.
        /// </summary>
        public SegmentDirection Direction => CalculateDirection(Delta);

        /// <summary>
        /// Creates a new segment. The start and endpoints must be different.
        /// </summary>
        /// <param name="startX">The starting X coordinate.</param>
        /// <param name="startY">The starting Y coordinate.</param>
        /// <param name="endX">The ending X coordinate.</param>
        /// <param name="endY">The ending Y coordinate.</param>
        public Seg2F(float startX, float startY, float endX, float endY) :
            this(new Vector2(startX, startY), new Vector2(endX, endY))
        {
        }

        /// <summary>
        /// Creates a new segment. The start and endpoints must be different.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        public Seg2F(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
            Delta = end - start;
        }

        /// <summary>
        /// Allows for creation of a 2D segment from a start/end point pair.
        /// </summary>
        /// <param name="pair">The tuple.</param>
        /// <returns>A new 2D segment.</returns>
        public static implicit operator Seg2F(ValueTuple<Vector2, Vector2> pair) => new Seg2F(pair.Item1, pair.Item2);

        /// <summary>
        /// Gets the endpoint from the enumeration.
        /// </summary>
        /// <param name="endpoint">The endpoint to get.</param>
        /// <returns>The endpoint for the enumeration.</returns>
        public Vector2 this[Endpoint endpoint] => endpoint == Endpoint.Start ? Start : End;

        /// <summary>
        /// Calculates the 'float triangle' area which is the triangle formed
        /// from the three points, but floatd.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <param name="third">The third point.</param>
        /// <returns>The floatd area of the triangles.</returns>
        public static float floatTriArea(Vector2 first, Vector2 second, Vector2 third)
        {
            return ((first.x - third.x) * (second.y - third.y)) - ((first.y - third.y) * (second.x - third.x));
        }

        /// <summary>
        /// Gets the rotation from a point with respect to another two points
        /// that make a line.
        /// </summary>
        /// <remarks>
        /// <para>Calculates the side the third point is on.</para>
        /// <para>This assumes that `first` and `second` form a line segment (where first
        /// is the starting point and second is the ending point of the segment) and
        /// the third point is evaluated to be on the side of the line from the two
        /// points. It can be imagined like this:</para>
        /// <code>
        ///                |
        ///    Second o---------o First
        ///         _/
        ///         /      (rotation would be on the left side)
        ///  Third o
        /// </code>
        /// </remarks>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point between first/third.</param>
        /// <param name="third">The third point.</param>
        /// <param name="epsilon">An optional comparison value.</param>
        /// <returns>The side the third point is on relative to the first and
        /// the second point.</returns>
        public static Rotation GetRotation(Vector2 first, Vector2 second, Vector2 third, float epsilon = 0.00001f)
        {
            return new Seg2F(first, second).ToSide(third, epsilon);
        }

        /// <summary>
        /// Gets the opposite endpoint from the enumeration.
        /// </summary>
        /// <param name="endpoint">The opposite endpoint to get.</param>
        /// <returns>The opposite endpoint for the enumeration.</returns>
        [Pure]
        public Vector2 Opposite(Endpoint endpoint) => endpoint == Endpoint.Start ? End : Start;

        /// <summary>
        /// Gets the time the point would have on the segment. This does not
        /// need to be between the [0, 1] range.
        /// </summary>
        /// <remarks>
        /// If the point is not on the segment, then the result will be wrong.
        /// A corollary to this is that `Start + t*Delta = point`.
        /// </remarks>
        /// <param name="point">The point to get the time for.</param>
        /// <returns>The time the point is on this segment.</returns>
        [Pure]
        public float ToTime(Vector2 point)
        {
            if (!Delta.x.ApproxZero())
                return (point.x - Start.x) * DeltaInverse.x;
            return (point.y - Start.y) * DeltaInverse.y;
        }

        /// <summary>
        /// Checks if the segments overlap. This assumes collinearity.
        /// </summary>
        /// <param name="seg">The segment to check.</param>
        /// <returns>True if they overlap, false otherwise.</returns>
        [Pure]
        public bool Overlaps(Seg2F seg)
        {
            float tStart = ToTime(seg.Start);
            float tEnd = ToTime(seg.End);
            return (tStart > 0.0 && tStart < 1.0) || (tEnd > 0.0 && tEnd < 1.0);
        }

        /// <summary>
        /// Checks if the box intersects this segment.
        /// </summary>
        /// <param name="box">The box to check.</param>
        /// <returns>True if it intersects, false if not.</returns>
        [Pure]
        public bool Intersects(Box2F box)
        {
            if (!Box.Overlaps(box))
                return false;

            switch (Direction)
            {
            case SegmentDirection.Vertical:
                return box.Min.x < Start.x && Start.x < box.Max.x;
            case SegmentDirection.Horizontal:
                return box.Min.y < Start.y && Start.y < box.Max.y;
            case SegmentDirection.PositiveSlope:
                return DifferentSides(box.TopLeft, box.BottomRight);
            case SegmentDirection.NegativeSlope:
                return DifferentSides(box.BottomLeft, box.TopRight);
            default:
                throw new InvalidOperationException("Invalid box intersection direction enumeration");
            }
        }

        /// <summary>
        /// Gets a point from the time provided. This will also work even if
        /// the time is not in the [0.0, 1.0] range.
        /// </summary>
        /// <param name="t">The time (where 0.0 = start and 1.0 = end).</param>
        /// <returns>The point from the time provided.</returns>
        [Pure]
        public Vector2 FromTime(float t) => Start + (Delta * t);

        /// <summary>
        /// Checks if both segments go in the same direction, with respect for
        /// the Start -> End direction.
        /// </summary>
        /// <param name="seg">The other segment to compare against.</param>
        /// <returns>True if they go the same direction, false otherwise.
        /// </returns>
        [Pure]
        public bool SameDirection(Seg2F seg) => SameDirection(seg.Delta);

        /// <summary>
        /// Same as <see cref="SameDirection(Seg2F)"/> but uses a delta to
        /// check.
        /// </summary>
        /// <param name="delta">The delta direction.</param>
        /// <returns>True if they go the same direction, false otherwise.
        /// </returns>
        [Pure]
        public bool SameDirection(Vector2 delta) => !Delta.x.DifferentSign(delta.x) && !Delta.y.DifferentSign(delta.y);

        /// <summary>
        /// Calculates the perpendicular dot product. This also may be known as
        /// the wedge product.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The perpendicular dot product.</returns>
        [Pure]
        public float PerpDot(in Vector2 point)
        {
            return (Delta.x * (point.y - Start.y)) - (Delta.y * (point.x - Start.x));
        }

        /// <summary>
        /// Calculates the perpendicular dot product. This also may be known as
        /// the wedge product.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The perpendicular dot product.</returns>
        [Pure]
        public float PerpDot(in Vector3 point)
        {
            return (Delta.x * (point.y - Start.y)) - (Delta.y * (point.x - Start.x));
        }

        /// <summary>
        /// Gets the side the point is on relative to this segment.
        /// </summary>
        /// <param name="point">The point to get.</param>
        /// <param name="epsilon">An optional epsilon for comparison.</param>
        /// <returns>The side it's on.</returns>
        [Pure]
        public Rotation ToSide(Vector2 point, float epsilon = 0.00001f)
        {
            float value = PerpDot(point);
            bool approxZero = value.ApproxZero(epsilon);
            return approxZero ? Rotation.On : (value < 0 ? Rotation.Right : Rotation.Left);
        }

        /// <summary>
        /// Checks if the point is on the right side of this segment (or on the
        /// seg itself).
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it's on the right (or on the line), false if on
        /// the left.</returns>
        [Pure]
        public bool OnRight(in Vector2 point) => PerpDot(point) <= 0;

        /// <summary>
        /// Checks if the point is on the right side of this segment (or on the
        /// seg itself).
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it's on the right (or on the line), false if on
        /// the left.</returns>
        [Pure]
        public bool OnRight(in Vector3 point) => PerpDot(point) <= 0;

        /// <summary>
        /// Checks if the segment has both endpoints on this or on the right of
        /// this.
        /// </summary>
        /// <param name="seg">The segment to check.</param>
        /// <returns>True if the segment has both points on/to the right, or
        /// false if one or more points is on the left.</returns>
        [Pure]
        public bool OnRight(Seg2F seg) => OnRight(seg.Start) && OnRight(seg.End);

        /// <summary>
        /// Checks if the box has all the points on the right side.
        /// </summary>
        /// <param name="box">The box to check.</param>
        /// <returns>True if the box has all the points on the right side or
        /// on the segment, false otherwise.</returns>
        [Pure]
        public bool OnRight(Box2F box) => OnRight(box.Min);

        /// <summary>
        /// Checks if the two points are on different sides of this segment.
        /// This considers a point on the segment to be on the right side.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>True if they are, false if not.</returns>
        [Pure]
        public bool DifferentSides(Vector2 first, Vector2 second) => OnRight(first) != OnRight(second);

        /// <summary>
        /// Checks if the two points of the segment are on different sides of
        /// this segment. This considers a point on the segment to be on the
        /// right side.
        /// </summary>
        /// <param name="seg">The segment endpoints to check.</param>
        /// <returns>True if it is, false if not.</returns>
        [Pure]
        public bool DifferentSides(Seg2F seg) => OnRight(seg.Start) != OnRight(seg.End);

        /// <summary>
        /// Checks if the segment provided is parallel.
        /// </summary>
        /// <param name="seg">The segment to check.</param>
        /// <param name="epsilon">An optional comparison epsilon.</param>
        /// <returns>True if it's parallel, false if not.</returns>
        [Pure]
        public bool Parallel(Seg2F seg, float epsilon = 0.00001f)
        {
            // If both slopes are the same for seg 1 and 2, then we know the
            // slopes are the same, meaning: d1y / d1x = d2y / d2x. Therefore
            // d1y * d2x == d2y * d1x. This also avoids weird division by zero
            // errors and all that fun stuff from any vertical lines.
            return (Delta.y * seg.Delta.x).Approx(Delta.x * seg.Delta.y, epsilon);
        }

        /// <summary>
        /// Checks if the segments are collinear to each other.
        /// </summary>
        /// <param name="seg">The segment to check.</param>
        /// <returns>True if collinear, false if not.</returns>
        [Pure]
        public bool Collinear(Seg2F seg)
        {
            return CollinearHelper(seg.Start, Start, End) && CollinearHelper(seg.End, Start, End);
        }

        /// <summary>
        /// Gets the closest distance from the point provided to this segment.
        /// </summary>
        /// <param name="point">The point to evaluate.</param>
        /// <returns>The closest distance.</returns>
        [Pure]
        public float ClosestDistance(Vector2 point)
        {
            // Source: https://math.stackexchange.com/questions/2193720/find-a-point-on-a-line-segment-which-is-the-closest-to-other-point-not-on-the-li
            Vector2 pointToStartDelta = Start - point;
            float t = -Delta.Dot(pointToStartDelta) / Delta.Dot(Delta);

            if (t <= 0.0)
                return point.Distance(Start);
            if (t >= 1.0)
                return point.Distance(End);
            return point.Distance(FromTime(t));
        }

        /// <summary>
        /// Checks if an intersection exists. This treats both of the segments
        /// as segments, not as infinite lines.
        /// </summary>
        /// <param name="other">The other segment to check.</param>
        /// <returns>True if an intersection exists, false if not.</returns>
        [Pure]
        public bool Intersects(Seg2F other) => Intersection(other, out float t) && (t >= 0 && t <= 1);

        /// <summary>
        /// Gets the intersection with a segment. This is not intended for line
        /// extension intersection, see the '...AsLine() methods for that.
        /// </summary>
        /// <remarks>
        /// See <see cref="IntersectionAsLine(Seg2F, out float)"/> for one
        /// and <see cref="IntersectionAsLine(Seg2F, out float, out float)"/>
        /// for both intersection times.
        /// </remarks>
        /// <param name="seg">The segment to check.</param>
        /// <param name="t">The output intersection time. If this function
        /// returns true, it is between [0.0, 1.0]. Otherwise it is a default
        /// value.</param>
        /// <returns>True if they intersect, false if not.</returns>
        [Pure]
        public bool Intersection(Seg2F seg, out float t)
        {
            float areaStart = floatTriArea(Start, End, seg.End);
            float areaEnd = floatTriArea(Start, End, seg.Start);

            if (areaStart.DifferentSign(areaEnd))
            {
                float areaThisStart = floatTriArea(seg.Start, seg.End, Start);
                float areaThisEnd = floatTriArea(seg.Start, seg.End, End);

                if (areaStart.DifferentSign(areaEnd))
                {
                    t = areaThisStart / (areaThisStart - areaThisEnd);
                    return t >= 0.0 && t <= 1.0;
                }
            }

            t = default;
            return false;
        }

        /// <summary>
        /// Treats intersection as if they are lines, so intersection points
        /// from this function are possibly found outside of the [0, 1] range.
        /// </summary>
        /// <param name="seg">The segment to test against.</param>
        /// <param name="tThis">The time of intersection located on this
        /// segment (not the parameter one). This has a default value if the
        /// method returns false.</param>
        /// <returns>True if an intersection exists, false if not.</returns>
        [Pure]
        public bool IntersectionAsLine(Seg2F seg, out float tThis)
        {
            float determinant = (-seg.Delta.x * Delta.y) + (Delta.x * seg.Delta.y);
            if (determinant.ApproxZero())
            {
                tThis = default;
                return false;
            }

            Vector2 startDelta = Start - seg.Start;
            tThis = ((seg.Delta.x * startDelta.y) - (seg.Delta.y * startDelta.x)) / determinant;
            return true;
        }

        /// <summary>
        /// Treats intersection as if they are lines, so intersection points
        /// from this function are possibly found outside of the [0, 1] range.
        /// </summary>
        /// <param name="seg">The segment to test against.</param>
        /// <param name="tThis">The time of intersection located on this
        /// segment (not the parameter one). This has a default value if the
        /// method returns false.</param>
        /// <param name="tOther">Same as `tThis`, but for the other segment.
        /// </param>
        /// <returns>True if an intersection exists, false if not.</returns>
        [Pure]
        public bool IntersectionAsLine(Seg2F seg, out float tThis, out float tOther)
        {
            float determinant = (-seg.Delta.x * Delta.y) + (Delta.x * seg.Delta.y);
            if (determinant.ApproxZero())
            {
                tThis = default;
                tOther = default;
                return false;
            }

            Vector2 startDelta = Start - seg.Start;
            float inverseDeterminant = 1.0f / determinant;
            tThis = ((seg.Delta.x * startDelta.y) - (seg.Delta.y * startDelta.x)) * inverseDeterminant;
            tOther = ((-Delta.y * startDelta.x) + (Delta.x * startDelta.y)) * inverseDeterminant;
            return true;
        }

        /// <summary>
        /// Gets the length of the segment.
        /// </summary>
        /// <returns>The length of the segment.</returns>
        [Pure]
        public float Length() => Delta.Length();

        /// <summary>
        /// Gets the squared length of the segment.
        /// </summary>
        /// <returns>The squared length of the segment.</returns>
        [Pure]
        public float LengthSquared() => Delta.LengthSquared();

        /// <summary>
        /// Gets the normal for this segment, which is equal to rotating the
        /// delta to the right by 90 degrees.
        /// </summary>
        /// <returns>The 90 degree right angle rotation of the delta.</returns>
        [Pure]
        public Vector2 RightNormal() => Delta.Right90();

        public override string ToString() => $"({Start}), ({End})";

        private static bool CollinearHelper(in Vector2 first, in Vector2 second, in Vector2 third)
        {
            float determinant = (first.x * (second.y - third.y)) +
                                (second.x * (third.y - first.y)) +
                                (third.x * (first.y - second.y));
            return determinant.ApproxZero();
        }

        private static Box2F MakeBox(Vector2 start, Vector2 end)
        {
            return new Box2F(
                new Vector2(Math.Min(start.x, end.x), Math.Min(start.y, end.y)),
                new Vector2(Math.Max(start.x, end.x), Math.Max(start.y, end.y)));
        }

        private static SegmentDirection CalculateDirection(Vector2 delta)
        {
            return delta.x.ApproxZero() ? SegmentDirection.Vertical :
                   delta.y.ApproxZero() ? SegmentDirection.Horizontal :
                   delta.x.DifferentSign(delta.y) ? SegmentDirection.NegativeSlope :
                                                    SegmentDirection.PositiveSlope;
        }
    }
}
