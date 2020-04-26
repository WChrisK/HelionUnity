using System;
using Helion.Util.Extensions;
using Helion.Util.Geometry.Boxes;
using Helion.Util.Geometry.Vectors;

namespace Helion.Util.Geometry.Segments
{
    /// <summary>
    /// The full class of a 2D segment for the type provided. Inheritance can
    /// be done, and various values are cached (such as line direction, bbox,
    /// and other things) to allow for faster computation.
    /// </summary>
    /// <remarks>
    /// For quick temporary stack-based values, use <see cref="Seg2D"/> over
    /// this one.
    /// </remarks>
    public class Segment2D<TVertex> where TVertex : Vector2D
    {
        /// <summary>
        /// The beginning point of the segment.
        /// </summary>
        public readonly TVertex Start;

        /// <summary>
        /// The ending point of the segment.
        /// </summary>
        public readonly TVertex End;

        /// <summary>
        /// The difference between the start to the end. This means that
        /// Start + Delta = End.
        /// </summary>
        public readonly Vec2D Delta;

        /// <summary>
        /// The inversed components of the delta.
        /// </summary>
        public Vec2D DeltaInverse;

        /// <summary>
        /// The bounding box of this segment.
        /// </summary>
        public Box2D Box;

        /// <summary>
        /// The direction this segment goes.
        /// </summary>
        public SegmentDirection Direction;

        public Vector2D StartVector => Start;

        public Vector2D EndVector => End;

        /// <summary>
        /// Creates a new segment. The start and endpoints must be different.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        public Segment2D(TVertex start, TVertex end)
        {
            Start = start;
            End = end;
            Delta = EndVector.Subtract(StartVector);
            DeltaInverse = new Vec2D(1.0f / Delta.X, 1.0f / Delta.Y);
            Box = MakeBox(Start, End);
            Direction = CalculateDirection(Delta);
        }

        /// <summary>
        /// Allows for creation of a 2D segment from a start/end point pair.
        /// </summary>
        /// <param name="pair">The tuple.</param>
        /// <returns>A new 2D segment.</returns>
        public static implicit operator Segment2D<TVertex>(ValueTuple<TVertex, TVertex> pair)
        {
            return new Segment2D<TVertex>(pair.Item1, pair.Item2);
        }

        /// <summary>
        /// Gets the endpoint from the enumeration.
        /// </summary>
        /// <param name="endpoint">The endpoint to get.</param>
        /// <returns>The endpoint for the enumeration.</returns>
        public TVertex this[Endpoint endpoint] => endpoint == Endpoint.Start ? Start : End;

        /// <summary>
        /// Calculates the 'double triangle' area which is the triangle formed
        /// from the three points, but doubled.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <param name="third">The third point.</param>
        /// <returns>The doubled area of the triangles.</returns>
        public static double DoubleTriArea(Vector2D first, Vector2D second, Vector2D third)
        {
            return ((first.X - third.X) * (second.Y - third.Y)) - ((first.Y - third.Y) * (second.X - third.X));
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
        public static Rotation GetRotation(Vector2D first, Vector2D second, Vector2D third, double epsilon = 0.000001)
        {
            return new Seg2D(first.Struct(), second.Struct()).ToSide(third.Struct(), epsilon);
        }

        /// <summary>
        /// Gets the opposite endpoint from the enumeration.
        /// </summary>
        /// <param name="endpoint">The opposite endpoint to get.</param>
        /// <returns>The opposite endpoint for the enumeration.</returns>
        public TVertex Opposite(Endpoint endpoint) => endpoint == Endpoint.Start ? End : Start;

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
        public double ToTime(Vector2D point)
        {
            if (!Delta.X.ApproxZero())
                return (point.X - Start.X) * DeltaInverse.X;
            return (point.Y - Start.Y) * DeltaInverse.Y;
        }

        /// <summary>
        /// Checks if the segments overlap. This assumes collinearity.
        /// </summary>
        /// <param name="seg">The segment to check.</param>
        /// <returns>True if they overlap, false otherwise.</returns>
        public bool Overlaps<TOtherVertex>(Segment2D<TOtherVertex> seg) where TOtherVertex : Vector2D
        {
            double tStart = ToTime(seg.Start);
            double tEnd = ToTime(seg.End);
            return (tStart > 0.0 && tStart < 1.0) || (tEnd > 0.0 && tEnd < 1.0);
        }

        /// <summary>
        /// Checks if the box intersects this segment.
        /// </summary>
        /// <param name="box">The box to check.</param>
        /// <returns>True if it intersects, false if not.</returns>
        public bool Intersects(Box2D box)
        {
            if (!Box.Overlaps(box))
                return false;

            switch (Direction)
            {
            case SegmentDirection.Vertical:
                return box.Min.X < Start.X && Start.X < box.Max.X;
            case SegmentDirection.Horizontal:
                return box.Min.Y < Start.Y && Start.Y < box.Max.Y;
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
        public Vec2D FromTime(double t) => (Start + (Delta * t)).Struct();

        /// <summary>
        /// Checks if both segments go in the same direction, with respect for
        /// the Start -> End direction.
        /// </summary>
        /// <param name="seg">The other segment to compare against.</param>
        /// <returns>True if they go the same direction, false otherwise.
        /// </returns>
        public bool SameDirection<TOtherVertex>(Segment2D<TOtherVertex> seg) where TOtherVertex : Vector2D
        {
            return SameDirection(seg.Delta);
        }

        /// <summary>
        /// Same as SameDirection() but uses a delta to check.
        /// </summary>
        /// <param name="delta">The delta direction.</param>
        /// <returns>True if they go the same direction, false otherwise.
        /// </returns>
        public bool SameDirection(Vec2D delta) => !Delta.X.DifferentSign(delta.X) && !Delta.Y.DifferentSign(delta.Y);

        /// <summary>
        /// Calculates the perpendicular dot product. This also may be known as
        /// the wedge product.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The perpendicular dot product.</returns>
        public double PerpDot(in Vec2D point)
        {
            return (Delta.X * (point.Y - Start.Y)) - (Delta.Y * (point.X - Start.X));
        }

        /// <summary>
        /// Calculates the perpendicular dot product. This also may be known as
        /// the wedge product.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The perpendicular dot product.</returns>
        public double PerpDot(Vector2D point)
        {
            return (Delta.X * (point.Y - Start.Y)) - (Delta.Y * (point.X - Start.X));
        }

        /// <summary>
        /// Checks if the point is on the right side of this segment (or on the
        /// seg itself).
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it's on the right (or on the line), false if on
        /// the left.</returns>
        public bool OnRight(in Vec2D point) => PerpDot(point) <= 0;

        /// <summary>
        /// Checks if the point is on the right side of this segment (or on the
        /// seg itself).
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it's on the right (or on the line), false if on
        /// the left.</returns>
        public bool OnRight(Vector2D point) => PerpDot(point) <= 0;

        /// <summary>
        /// Calculates the perpendicular dot product. This also may be known as
        /// the wedge product.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The perpendicular dot product.</returns>
        public double PerpDot(Vector3D point)
        {
            return (Delta.X * (point.Y - Start.Y)) - (Delta.Y * (point.X - Start.X));
        }

        /// <summary>
        /// Gets the side the point is on relative to this segment.
        /// </summary>
        /// <param name="point">The point to get.</param>
        /// <param name="epsilon">An optional epsilon for comparison.</param>
        /// <returns>The side it's on.</returns>
        public Rotation ToSide(Vector2D point, double epsilon = 0.000001)
        {
            double value = PerpDot(point);
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
        public bool OnRight(Vector3D point) => PerpDot(point) <= 0;

        /// <summary>
        /// Checks if the segment has both endpoints on this or on the right of
        /// this.
        /// </summary>
        /// <param name="seg">The segment to check.</param>
        /// <returns>True if the segment has both points on/to the right, or
        /// false if one or more points is on the left.</returns>
        public bool OnRight<TOtherVertex>(Segment2D<TOtherVertex> seg) where TOtherVertex : Vector2D
        {
            return OnRight(seg.Start) && OnRight(seg.End);
        }

        /// <summary>
        /// Checks if the box has all the points on the right side.
        /// </summary>
        /// <param name="box">The box to check.</param>
        /// <returns>True if the box has all the points on the right side or
        /// on the segment, false otherwise.</returns>
        public bool OnRight(BoundingBox2D box) => OnRight(box.Min);

        /// <summary>
        /// Checks if the two points are on different sides of this segment.
        /// This considers a point on the segment to be on the right side.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>True if they are, false if not.</returns>
        public bool DifferentSides(in Vec2D first, in Vec2D second) => OnRight(first) != OnRight(second);

        /// <summary>
        /// Checks if the two points are on different sides of this segment.
        /// This considers a point on the segment to be on the right side.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>True if they are, false if not.</returns>
        public bool DifferentSides(Vector2D first, Vector2D second) => OnRight(first) != OnRight(second);

        /// <summary>
        /// Checks if the two points of the segment are on different sides of
        /// this segment. This considers a point on the segment to be on the
        /// right side.
        /// </summary>
        /// <param name="seg">The segment endpoints to check.</param>
        /// <returns>True if it is, false if not.</returns>
        public bool DifferentSides<TOtherVertex>(Segment2D<TOtherVertex> seg) where TOtherVertex : Vector2D
        {
            return OnRight(seg.Start) != OnRight(seg.End);
        }

        /// <summary>
        /// Checks if the segment provided is parallel.
        /// </summary>
        /// <param name="seg">The segment to check.</param>
        /// <param name="epsilon">An optional comparison epsilon.</param>
        /// <returns>True if it's parallel, false if not.</returns>
        public bool Parallel<TOtherVertex>(Segment2D<TOtherVertex> seg, double epsilon = 0.000001)
            where TOtherVertex : Vector2D
        {
            // If both slopes are the same for seg 1 and 2, then we know the
            // slopes are the same, meaning: d1y / d1x = d2y / d2x. Therefore
            // d1y * d2x == d2y * d1x. This also avoids weird division by zero
            // errors and all that fun stuff from any vertical lines.
            return (Delta.Y * seg.Delta.X).Approx(Delta.X * seg.Delta.Y, epsilon);
        }

        /// <summary>
        /// Checks if the segments are collinear to each other.
        /// </summary>
        /// <param name="seg">The segment to check.</param>
        /// <returns>True if collinear, false if not.</returns>
        public bool Collinear<TOtherVertex>(Segment2D<TOtherVertex> seg)
            where TOtherVertex : Vector2D
        {
            return CollinearHelper(seg.Start, Start, End) && CollinearHelper(seg.End, Start, End);
        }

        /// <summary>
        /// Gets the closest distance from the point provided to this segment.
        /// </summary>
        /// <param name="point">The point to evaluate.</param>
        /// <returns>The closest distance.</returns>
        public double ClosestDistance(Vector2D point)
        {
            // Source: https://math.stackexchange.com/questions/2193720/find-a-point-on-a-line-segment-which-is-the-closest-to-other-point-not-on-the-li
            Vec2D pointToStartDelta = Start.Struct() - point;
            double t = -Delta.Dot(pointToStartDelta) / Delta.Dot(Delta);

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
        public bool Intersects<TOtherVertex>(Segment2D<TOtherVertex> other)
            where TOtherVertex : Vector2D
        {
            return Intersection(other, out double t) && (t >= 0 && t <= 1);
        }

        /// <summary>
        /// Gets the intersection with a segment. This is not intended for line
        /// extension intersection, see the '...AsLine() methods for that.
        /// </summary>
        /// <remarks>
        /// See the multiple IntersectionAsLine() functions for being able to
        /// get either one or both intersections as per use case.
        /// </remarks>
        /// <param name="seg">The segment to check.</param>
        /// <param name="t">The output intersection time. If this function
        /// returns true, it is between [0.0, 1.0]. Otherwise it is a default
        /// value.</param>
        /// <returns>True if they intersect, false if not.</returns>
        public bool Intersection<TOtherVertex>(Segment2D<TOtherVertex> seg, out double t)
            where TOtherVertex : Vector2D
        {
            double areaStart = DoubleTriArea(Start, End, seg.End);
            double areaEnd = DoubleTriArea(Start, End, seg.Start);

            if (areaStart.DifferentSign(areaEnd))
            {
                double areaThisStart = DoubleTriArea(seg.Start, seg.End, Start);
                double areaThisEnd = DoubleTriArea(seg.Start, seg.End, End);

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
        public bool IntersectionAsLine<TOtherVertex>(Segment2D<TOtherVertex> seg, out double tThis)
            where TOtherVertex : Vector2D
        {
            double determinant = (-seg.Delta.X * Delta.Y) + (Delta.X * seg.Delta.Y);
            if (determinant.ApproxZero())
            {
                tThis = default;
                return false;
            }

            Vec2D startDelta = Start.Struct() - seg.Start.Struct();
            tThis = ((seg.Delta.X * startDelta.Y) - (seg.Delta.Y * startDelta.X)) / determinant;
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
        public bool IntersectionAsLine<TOtherVertex>(Segment2D<TOtherVertex> seg, out double tThis, out double tOther)
            where TOtherVertex : Vector2D
        {
            double determinant = (-seg.Delta.X * Delta.Y) + (Delta.X * seg.Delta.Y);
            if (determinant.ApproxZero())
            {
                tThis = default;
                tOther = default;
                return false;
            }

            Vec2D startDelta = Start.Struct() - seg.Start.Struct();
            double inverseDeterminant = 1.0f / determinant;
            tThis = ((seg.Delta.X * startDelta.Y) - (seg.Delta.Y * startDelta.X)) * inverseDeterminant;
            tOther = ((-Delta.Y * startDelta.X) + (Delta.X * startDelta.Y)) * inverseDeterminant;
            return true;
        }

        /// <summary>
        /// Gets the length of the segment.
        /// </summary>
        /// <returns>The length of the segment.</returns>
        public double Length() => Delta.Length();

        /// <summary>
        /// Gets the squared length of the segment.
        /// </summary>
        /// <returns>The squared length of the segment.</returns>
        public double LengthSquared() => Delta.LengthSquared();

        /// <summary>
        /// Gets the normal for this segment, which is equal to rotating the
        /// delta to the right by 90 degrees.
        /// </summary>
        /// <returns>The 90 degree right angle rotation of the delta.</returns>
        public Vec2D RightNormal() => Delta.Right90();

        public override string ToString() => $"({Start}), ({End})";

        private static bool CollinearHelper(Vector2D first, Vector2D second, Vector2D third)
        {
            double determinant = (first.X * (second.Y - third.Y)) +
                                 (second.X * (third.Y - first.Y)) +
                                 (third.X * (first.Y - second.Y));
            return determinant.ApproxZero();
        }

        private static Box2D MakeBox(Vector2D start, Vector2D end)
        {
            return new Box2D(
                new Vec2D(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y)),
                new Vec2D(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y)));
        }

        private static SegmentDirection CalculateDirection(Vec2D delta)
        {
            return delta.X.ApproxZero() ? SegmentDirection.Vertical :
                   delta.Y.ApproxZero() ? SegmentDirection.Horizontal :
                   delta.X.DifferentSign(delta.Y) ? SegmentDirection.NegativeSlope :
                                                    SegmentDirection.PositiveSlope;
        }
    }
}
