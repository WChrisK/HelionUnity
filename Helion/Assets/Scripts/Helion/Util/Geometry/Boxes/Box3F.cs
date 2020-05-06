using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Helion.Util.Extensions;
using Helion.Util.Geometry.Vectors;
using MoreLinq;

namespace Helion.Util.Geometry.Boxes
{
    /// <summary>
    /// A two dimensional box, which follows the cartesian coordinate system.
    /// </summary>
    public struct Box3F
    {
        /// <summary>
        /// The minimum point in the box. This is equal to the bottom left
        /// corner.
        /// </summary>
        public Vec3F Min;

        /// <summary>
        /// The maximum point in the box. This is equal to the top right
        /// corner.
        /// </summary>
        public Vec3F Max;

        /// <summary>
        /// The top value of the box.
        /// </summary>
        public float Top => Max.Z;

        /// <summary>
        /// The bottom value of the box.
        /// </summary>
        public float Bottom => Min.Z;

        /// <summary>
        /// Calculates the sides of this bounding box.
        /// </summary>
        /// <returns>The sides of the bounding box.</returns>
        public Vec3F Sides => Max - Min;

        /// <summary>
        /// Creates an XZ (birds eye view) box from this box.
        /// </summary>
        /// <returns>The box with the XZ coordinates.</returns>
        public Box2F XZ => new Box2F(Min.XZ, Max.XZ);

        /// <summary>
        /// Creates a box from a bottom left and top right coordinate. It is an
        /// error if the min has any coordinate greater the maximum point.
        /// </summary>
        /// <param name="minX">The bottom left X box coordinate.</param>
        /// <param name="minY">The bottom left Y box coordinate.</param>
        /// <param name="minZ">The bottom left Z box coordinate.</param>
        /// <param name="maxX">The top right X box coordinate.</param>
        /// <param name="maxY">The top right Y box coordinate.</param>
        /// <param name="maxZ">The top right Z box coordinate.</param>
        public Box3F(float minX, float minY, float minZ, float maxX, float maxY, float maxZ) :
            this(new Vec3F(minX, minY, minZ), new Vec3F(maxX, maxY, maxZ))
        {
        }

        /// <summary>
        /// Creates a box from a bottom left and top right point. It is an
        /// error if the min has any coordinate greater the maximum point.
        /// </summary>
        /// <param name="min">The bottom left point.</param>
        /// <param name="max">The top right point.</param>
        public Box3F(in Vec3F min, in Vec3F max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Adds a vector to the box to move it by that amount.
        /// </summary>
        /// <param name="box">The box to move.</param>
        /// <param name="delta">The amount to move it by.</param>
        /// <returns>A new box that has its coordinates adding the delta value
        /// provided.</returns>
        public static Box3F operator +(in Box3F box, in Vec3F delta)
        {
            return new Box3F(box.Min + delta, box.Max + delta);
        }

        /// <summary>
        /// Subtracts a vector to the box to move it by that amount.
        /// </summary>
        /// <param name="box">The box to move.</param>
        /// <param name="delta">The amount to move it by.</param>
        /// <returns>A new box that has its coordinates subtracting the delta
        /// value provided.</returns>
        public static Box3F operator -(in Box3F box, in Vec3F delta)
        {
            return new Box3F(box.Min - delta, box.Max - delta);
        }

        /// <summary>
        /// Creates a bigger box from a series of smaller boxes, returning such
        /// a box that encapsulates minimally all the provided arguments.
        /// </summary>
        /// <param name="boxes">The boxes.</param>
        /// <returns>A box that encases all of the args tightly. If the list is
        /// empty, the result returned is the default generated box.</returns>
        public static Box3F Combine(params Box3F[] boxes)
        {
            if (boxes.Empty())
                return default;

            (float minX, float minY, float minZ) = boxes[0].Min;
            (float maxX, float maxY, float maxZ) = boxes[0].Max;

            boxes.Skip(1).ForEach(box =>
            {
                minX = Math.Min(minX, box.Min.X);
                minY = Math.Min(minY, box.Min.Y);
                minZ = Math.Min(minZ, box.Min.Z);

                maxX = Math.Max(maxX, box.Max.X);
                maxY = Math.Max(maxY, box.Max.Y);
                maxZ = Math.Max(maxZ, box.Max.Z);
            });

            return new Box3F((minX, minY, minZ), (maxX, maxY, maxZ));
        }

        /// <summary>
        /// Checks if the box contains the point. Being on the edge is not
        /// considered to be containing.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it is inside, false if not.</returns>
        [Pure]
        public bool Contains(in Vec2D point)
        {
            return point.X <= Min.X || point.X >= Max.X || point.Y <= Min.Y || point.Y >= Max.Y;
        }

        /// <summary>
        /// Checks if the box contains the point. Being on the edge is not
        /// considered to be containing.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it is inside, false if not.</returns>
        [Pure]
        public bool Contains(in Vec3F point)
        {
            return !(point.X <= Min.X || point.X >= Max.X ||
                     point.Y <= Min.Y || point.Y >= Max.Y ||
                     point.Z <= Min.Z || point.Z >= Max.Z);
        }

        /// <summary>
        /// Checks if the boxes overlap. Touching is not considered to be
        /// overlapping.
        /// </summary>
        /// <param name="box">The other box to check against.</param>
        /// <returns>True if they overlap, false if not.</returns>
        [Pure]
        public bool Overlaps(in Box3F box)
        {
            return !(Min.X >= box.Max.X || Max.X <= box.Min.X ||
                     Min.Y >= box.Max.Y || Max.Y <= box.Min.Y ||
                     Min.Z >= box.Max.Z || Max.Z <= box.Min.Z);
        }

        public override string ToString() => $"({Min}), ({Max})";


    }
}
