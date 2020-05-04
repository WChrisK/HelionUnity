using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Helion.Util.Extensions;
using Helion.Util.Geometry.Segments;
using Helion.Util.Geometry.Vectors;
using MoreLinq;
using UnityEngine;

namespace Helion.Util.Geometry.Boxes
{
    /// <summary>
    /// A two dimensional box, which follows the cartesian coordinate system.
    /// </summary>
    public readonly struct Box2F
    {
        /// <summary>
        /// The minimum point in the box. This is equal to the bottom left
        /// corner.
        /// </summary>
        public readonly Vec2F Min;

        /// <summary>
        /// The maximum point in the box. This is equal to the top right
        /// corner.
        /// </summary>
        public readonly Vec2F Max;

        /// <summary>
        /// The top left corner of the box.
        /// </summary>
        public Vec2F TopLeft => new Vec2F(Min.X, Max.Y);

        /// <summary>
        /// The bottom left corner of the box.
        /// </summary>
        public Vec2F BottomLeft => Min;

        /// <summary>
        /// The bottom right corner of the box.
        /// </summary>
        public Vec2F BottomRight => new Vec2F(Max.X, Min.Y);

        /// <summary>
        /// The top right corner of the box.
        /// </summary>
        public Vec2F TopRight => Max;

        /// <summary>
        /// The top value of the box.
        /// </summary>
        public float Top => Max.Y;

        /// <summary>
        /// The bottom value of the box.
        /// </summary>
        public float Bottom => Min.Y;

        /// <summary>
        /// The left value of the box.
        /// </summary>
        public float Left => Min.X;

        /// <summary>
        /// The right value of the box.
        /// </summary>
        public float Right => Max.X;

        /// <summary>
        /// Calculates the sides of this bounding box.
        /// </summary>
        /// <returns>The sides of the bounding box.</returns>
        public Vec2F Sides => Max - Min;

        /// <summary>
        /// A property that calculates the width of the box.
        /// </summary>
        public float Width => Max.X - Min.X;

        /// <summary>
        /// A property that calculates the height of the box.
        /// </summary>
        public float Height => Max.Y - Min.Y;

        /// <summary>
        /// Gets the center point in the box.
        /// </summary>
        public Vec2F Center => BottomLeft + (Sides * 0.5f);

        /// <summary>
        /// Creates a box from a bottom left and top right coordinate.
        /// </summary>
        /// <param name="minX">The coordinate for the bottom left X corner.
        /// </param>
        /// <param name="minY">The coordinate for the bottom left Y corner.
        /// </param>
        /// <param name="maxX">The coordinate for the top right X corner.
        /// </param>
        /// <param name="maxY">The coordinate for the top right Y corner.
        /// </param>
        public Box2F(float minX, float minY, float maxX, float maxY) :
            this(new Vec2F(minX, minY), new Vec2F(maxX, maxY))
        {
        }

        /// <summary>
        /// Creates a box from a bottom left and top right point. It is an
        /// error if the min has any coordinate greater the maximum point.
        /// </summary>
        /// <param name="min">The bottom left point.</param>
        /// <param name="max">The top right point.</param>
        public Box2F(in Vec2F min, in Vec2F max)
        {
            Debug.Assert(min.X <= max.X && min.Y <= max.Y, "Box2F min >= max");

            Min = min;
            Max = max;
        }

        /// <summary>
        /// Allows for creation of a 2D box from a start/end point pair.
        /// </summary>
        /// <param name="pair">The tuple of start (Item1) and end (Item2)
        /// points.</param>
        /// <returns>A new 2D segment.</returns>
        public static implicit operator Box2F(ValueTuple<Vec2F, Vec2F> pair) => new Box2F(pair.Item1, pair.Item2);

        /// <summary>
        /// Creates a bigger box from a series of smaller boxes, returning such
        /// a box that encapsulates minimally all the provided arguments.
        /// </summary>
        /// <param name="boxes">The boxes.</param>
        /// <returns>A box that encases all of the args tightly. If the list is
        /// empty, the result returned is the default generated box.</returns>
        public static Box2F Combine(params Box2F[] boxes) => Combine(boxes);

        /// <summary>
        /// Creates a bigger box from a series of smaller boxes, returning such
        /// a box that encapsulates minimally all the provided arguments.
        /// </summary>
        /// <param name="boxes">The boxes.</param>
        /// <returns>A box that encases all of the args tightly. If the list is
        /// empty, the result returned is the default generated box.</returns>
        public static Box2F Combine(IEnumerable<Box2F> boxes)
        {
            if (boxes.Empty())
                return default;

            (float minX, float minY) = boxes.ElementAt(0).Min;
            (float maxX, float maxY) = boxes.ElementAt(0).Max;

            boxes.Skip(1).ForEach(box =>
            {
                minX = Math.Min(minX, box.Min.X);
                minY = Math.Min(minY, box.Min.Y);

                maxX = Math.Max(maxX, box.Max.X);
                maxY = Math.Max(maxY, box.Max.Y);
            });

            return new Box2F(new Vec2F(minX, minY), new Vec2F(maxX, maxY));
        }

        /// <summary>
        /// Bounds all the segments by a tight axis aligned bounding box.
        /// </summary>
        /// <param name="segments">The segments to bound. This should contain
        /// at least one element.</param>
        /// <returns>A box that bounds the segments.</returns>
        public static Box2F BoundSegments(IEnumerable<Seg2F> segments)
        {
            if (segments.Empty())
                return default;
            return Combine(segments.Select(s => s.Box).ToArray());
        }

        /// <summary>
        /// Takes some offset delta and creates a copy of the box at the offset
        /// provided.
        /// </summary>
        /// <param name="newCenter">The new center of the box.</param>
        /// <returns>The box at the position.</returns>
        [Pure]
        public Box2F CopyTo(in Vec2F newCenter)
        {
            Vec2F delta = Sides * 0.5f;
            return new Box2F(newCenter - delta, newCenter + delta);
        }

        /// <summary>
        /// Copies the current box to a new location by some offset amount.
        /// </summary>
        /// <param name="offset">The offset to copy.</param>
        /// <returns>The new box.</returns>
        [Pure]
        public Box2F CopyOffset(in Vec2F offset) => new Box2F(Min + offset, Max + offset);

        /// <summary>
        /// Checks if the box contains the point. Being on the edge is not
        /// considered to be containing.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it is inside, false if not.</returns>
        [Pure]
        public bool Contains(in Vec2F point)
        {
            return point.X > Min.X && point.X < Max.X && point.Y > Min.Y && point.Y < Max.Y;
        }

        /// <summary>
        /// Checks if the box contains the point. Being on the edge is not
        /// considered to be containing.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it is inside, false if not.</returns>
        [Pure]
        public bool Contains(in Vector3 point)
        {
            return point.x > Min.X && point.x < Max.X && point.y > Min.Y && point.y < Max.Y;
        }

        /// <summary>
        /// Checks if the boxes overlap. Touching is not considered to be
        /// overlapping.
        /// </summary>
        /// <param name="box">The other box to check against.</param>
        /// <returns>True if they overlap, false if not.</returns>
        [Pure]
        public bool Overlaps(in Box2F box)
        {
            return !(Min.X >= box.Max.X || Max.X <= box.Min.X || Min.Y >= box.Max.Y || Max.Y <= box.Min.Y);
        }

        /// <summary>
        /// Gets the spanning edge to some point. If along diagonals, it instead
        /// will return the diagonal. The vertex pair returned will always have
        /// the provided point be on the right side of the line.
        /// </summary>
        /// <remarks>
        /// The spanning edge is the edge of the box, or the diagonal, based on
        /// where the point is located.
        /// </remarks>
        /// <param name="position">The position to look from.</param>
        /// <returns>The start and end points along the bounding box.</returns>
        [Pure]
        public Seg2F GetSpanningEdge(in Vec2F position)
        {
            // This is best understood by asking ourselves how we'd classify
            // where we are along a 1D line. Suppose we want to find out which
            // one of the spans were in along the X axis:
            //
            //      0     1     2
            //   A-----B-----C-----D
            //
            // We want to know if we're in span 0, 1, or 2. We can just check
            // by doing `if x > B` for span 1 or 2, and `if x > C` for span 2.
            // Instead of doing if statements, we can just cast the bool to an
            // int and add them up.
            //
            // Next we do this along the Y axis.
            //
            // After our results, we can merge the bits such that the higher
            // two bits are the Y value, and the lower 2 bits are the X value.
            // This gives us: 0bYYXX.
            //
            // Since each coordinate in the following image has its own unique
            // bitcode, we can switch on the bitcode to get the corners.
            //
            //       XY values           Binary codes
            //
            //      02 | 12 | 22       1000|1001|1010
            //         |    |           8  | 9  | A
            //     ----o----o----      ----o----o----
            //      01 | 11 | 21       0100|0101|0110
            //         |    |           4  | 5  | 6
            //     ----o----o----      ----o----o----
            //      00 | 10 | 20       0000|0001|0010
            //         |    |           0  | 1  | 2
            //
            // Note this is my optimization to the Cohen-Sutherland algorithm
            // bitcode detector.
            uint horizontalBits = Convert.ToUInt32(position.X > Left) + Convert.ToUInt32(position.X > Right);
            uint verticalBits = Convert.ToUInt32(position.Y > Bottom) + Convert.ToUInt32(position.Y > Top);

            switch (horizontalBits | (verticalBits << 2))
            {
            case 0x0: // Bottom left
                return new Seg2F(TopLeft, BottomRight);
            case 0x1: // Bottom middle
                return new Seg2F(BottomLeft, BottomRight);
            case 0x2: // Bottom right
                return new Seg2F(BottomLeft, TopRight);
            case 0x4: // Middle left
                return (TopLeft, BottomLeft);
            case 0x5: // Center (this shouldn't be a case via precondition).
                return new Seg2F(TopLeft, BottomRight);
            case 0x6: // Middle right
                return new Seg2F(BottomRight, TopRight);
            case 0x8: // Top left
                return new Seg2F(TopRight, BottomLeft);
            case 0x9: // Top middle
                return new Seg2F(TopRight, TopLeft);
            case 0xA: // Top right
                return new Seg2F(BottomRight, TopLeft);
            default:
                Debug.Assert(false, "Unexpected spanning edge bit code");
                return new Seg2F(TopLeft, BottomRight);
            }
        }

        /// <summary>
        /// Checks if a box is equal to another.
        /// </summary>
        /// <param name="other">The other box.</param>
        /// <returns>True if so, false otherwise.</returns>
        [Pure]
        public bool Equals(Box2F other) => Min.Equals(other.Min) && Max.Equals(other.Max);

        public override string ToString() => $"({Min}), ({Max})";

        public override bool Equals(object obj)
        {
            return obj is Box2F other && Equals(other);
        }

        public override int GetHashCode()
        {
            return unchecked((Min.GetHashCode() * 397) ^ Max.GetHashCode());
        }
    }
}
