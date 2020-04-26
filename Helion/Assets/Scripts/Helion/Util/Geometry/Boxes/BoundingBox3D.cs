using System;
using System.Linq;
using Helion.Util.Extensions;
using Helion.Util.Geometry.Vectors;
using MoreLinq;
using UnityEngine;

namespace Helion.Util.Geometry.Boxes
{
    /// <summary>
    /// A three dimensional box, which follows the cartesian coordinate system.
    /// </summary>
    public class BoundingBox3D
    {
        /// <summary>
        /// The minimum point in the box. This is equal to the bottom left
        /// corner.
        /// </summary>
        public Vec3D Min;

        /// <summary>
        /// The maximum point in the box. This is equal to the top right
        /// corner.
        /// </summary>
        public Vec3D Max;

        /// <summary>
        /// The top value of the box.
        /// </summary>
        public double Top => Max.Z;

        /// <summary>
        /// The bottom value of the box.
        /// </summary>
        public double Bottom => Min.Z;

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
        public BoundingBox3D(double minX, double minY, double minZ, double maxX, double maxY, double maxZ) :
            this(new Vec3D(minX, minY, minZ), new Vec3D(maxX, maxY, maxZ))
        {
        }

        /// <summary>
        /// Creates a box from a bottom left and top right point. It is an
        /// error if the min has any coordinate greater the maximum point.
        /// </summary>
        /// <param name="min">The bottom left point.</param>
        /// <param name="max">The top right point.</param>
        public BoundingBox3D(Vec3D min, Vec3D max)
        {
            Debug.Assert(min.X <= max.X && min.Y <= max.Y && min.Z <= max.Z, "Box3D min >= max");

            Min = min;
            Max = max;
        }

        /// <summary>
        /// Allows for creation of a 3D box from a start/end point pair.
        /// </summary>
        /// <param name="pair">The tuple of start (Item1) and end (Item2)
        /// points.</param>
        /// <returns>A new 3D bounding box.</returns>
        public static implicit operator BoundingBox3D(ValueTuple<Vec3D, Vec3D> pair)
        {
            return new BoundingBox3D(pair.Item1, pair.Item2);
        }

        /// <summary>
        /// Creates a bigger box from a series of smaller boxes, returning such
        /// a box that encapsulates minimally all the provided arguments.
        /// </summary>
        /// <param name="boxes">The boxes.</param>
        /// <returns>A box that encases all of the args tightly. If the list is
        /// empty, the result returned is the default generated box.</returns>
        public static BoundingBox3D Combine(params BoundingBox3D[] boxes)
        {
            if (boxes.Empty())
                return new BoundingBox3D(0, 0, 0, 0, 0, 0);

            (double minX, double minY, double minZ) = boxes[0].Min;
            (double maxX, double maxY, double maxZ) = boxes[0].Max;

            boxes.Skip(1).ForEach(box =>
            {
                minX = Math.Min(minX, box.Min.X);
                minY = Math.Min(minY, box.Min.Y);
                minZ = Math.Min(minZ, box.Min.Z);

                maxX = Math.Max(maxX, box.Max.X);
                maxY = Math.Max(maxY, box.Max.Y);
                maxZ = Math.Max(maxZ, box.Max.Z);
            });

            return new BoundingBox3D((minX, minY, minZ), (maxX, maxY, maxZ));
        }

        /// <summary>
        /// Checks if the box contains the point. Being on the edge is not
        /// considered to be containing.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it is inside, false if not.</returns>
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
        public bool Contains(in Vec3D point)
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
        public bool Overlaps(in Box3D box)
        {
            return !(Min.X >= box.Max.X || Max.X <= box.Min.X ||
                     Min.Y >= box.Max.Y || Max.Y <= box.Min.Y ||
                     Min.Z >= box.Max.Z || Max.Z <= box.Min.Z);
        }

        /// <summary>
        /// Calculates the sides of this bounding box.
        /// </summary>
        /// <returns>The sides of the bounding box.</returns>
        public Vec3D Sides() => Max - Min;

        /// <summary>
        /// Gets a 2-dimensional box by dropping the Z axis.
        /// </summary>
        /// <returns>The two dimensional representation of this box.</returns>
        public Box2D To2D() => new Box2D(Min.To2D(), Max.To2D());

        public override string ToString() => $"({Min}), ({Max})";
    }
}
