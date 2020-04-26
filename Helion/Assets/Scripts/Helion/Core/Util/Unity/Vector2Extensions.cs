using System;
using Helion.Core.Util.Geometry;
using Helion.Core.Util.Geometry.Vectors;
using UnityEngine;

namespace Helion.Core.Util.Unity
{
    /// <summary>
    /// Helper methods for Unity's Vector3 class.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Deconstructs a vector to a tuple.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public static void Deconstruct(this Vector2 vec, out float x, out float y)
        {
            x = vec.x;
            y = vec.y;
        }

        /// <summary>
        /// Creates a new vector but with a new X component.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="x">The new X component.</param>
        /// <returns>A new vector with the X component.</returns>
        public static Vector2 WithX(this Vector2 vec, float x) => new Vector2(x, vec.y);

        /// <summary>
        /// Creates a new vector but with a new Y component.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="y">The new Y component.</param>
        /// <returns>A new vector with the Y component.</returns>
        public static Vector2 WithY(this Vector2 vec, float y) => new Vector2(vec.x, y);

        /// <summary>
        /// Creates a new 3D vector with the Z component.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="z">The new Z component.</param>
        /// <returns>A new 3D vector with the Z component.</returns>
        public static Vector3 WithZ(this Vector2 vec, float z) => new Vector3(vec.x, vec.y, z);

        /// <summary>
        /// Floors the components and returns the result.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>A floored vector.</returns>
        public static Vector2 Floor(this Vector2 vec)
        {
            return new Vector2((float)Math.Floor(vec.x), (float)Math.Floor(vec.y));
        }

        /// <summary>
        /// Ceiling the components and returns the result.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>A ceiling vector.</returns>
        public static Vector2 Ceil(this Vector2 vec)
        {
            return new Vector2((float)Math.Ceiling(vec.x), (float)Math.Ceiling(vec.y));
        }

        /// <summary>
        /// Returns an absolute version of the vector.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>A vector with absoluted components.</returns>
        public static Vector2 Abs(this Vector2 vec)
        {
            return new Vector2(Math.Abs(vec.x), Math.Abs(vec.y));
        }

        /// <summary>
        /// Calculates the unit vector from this vector.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>A unit vector from this vector.</returns>
        public static Vector2 Unit(this Vector2 vec) => vec / vec.Length();

        /// <summary>
        /// Calculates the dot product with another vector.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        public static float Dot(this Vector2 vec, in Vector2 other)
        {
            return (vec.x * other.x) + (vec.y * other.y);
        }

        /// <summary>
        /// Calculates the squared length.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>The squared length.</returns>
        public static float LengthSquared(this Vector2 vec) => (vec.x * vec.x) + (vec.y * vec.y);

        /// <summary>
        /// Calculates the length of the vector (with respect to the origin).
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>The vector length.</returns>
        public static float Length(this Vector2 vec) => (float)Math.Sqrt(vec.LengthSquared());

        /// <summary>
        /// Calculates the component of the vector relative to the provided
        /// vector.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="onto">The vector to get the component along.</param>
        /// <returns>The component value of the vector.</returns>
        public static float Component(this Vector2 vec, in Vector2 onto) => vec.Dot(onto) / onto.Length();

        /// <summary>
        /// Projects this vector onto the provided vector.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="onto">The vector to project onto.</param>
        /// <returns>The projection.</returns>
        public static Vector2 Projection(this Vector2 vec, in Vector2 onto) => vec.Dot(onto) / onto.LengthSquared() * onto;

        /// <summary>
        /// Gets the distance from this to the other vector squared.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="other">The other vector.</param>
        /// <returns>The squared distance.</returns>
        public static float DistanceSquared(this Vector2 vec, in Vector2 other) => (vec - other).LengthSquared();

        /// <summary>
        /// Gets the distance from this to the other vector.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="other">The other vector.</param>
        /// <returns>The distance.</returns>
        public static float Distance(this Vector2 vec, in Vector2 other) => (vec - other).Length();

        /// <summary>
        /// Interpolates between the provided vector.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <param name="end">The end point.</param>
        /// <param name="t">The time, usually between [0, 1].</param>
        /// <returns>The point along the vector from this to the end vector, at
        /// the time provided.</returns>
        public static Vector2 Interpolate(this Vector2 vec, in Vector2 end, float t)
        {
            return vec + (t * (end - vec));
        }

        /// <summary>
        /// Gets a new vector that is the right 90 degree rotation with respect
        /// to the origin.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>The right angle rotation.</returns>
        public static Vector2 Right90(this Vector2 vec) => new Vector2(vec.y, -vec.x);

        /// <summary>
        /// Gets a new vector that is the left 90 degree rotation with respect
        /// to the origin.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>The left angle rotation.</returns>
        public static Vector2 Left90(this Vector2 vec) => new Vector2(-vec.y, vec.x);

        /// <summary>
        /// Creates a floating point vector from the double coordinate vector.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>A 2D floating point vector.</returns>
        public static Vec2I Int(this Vector2 vec) => new Vec2I((int)vec.x, (int)vec.y);

        /// <summary>
        /// Creates a floating point vector from the double coordinate vector.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>A 2D floating point vector.</returns>
        public static Vec2D Double(this Vector2 vec) => new Vec2D(vec.x, vec.y);

        /// <summary>
        /// Scales the vector to Unity's map unit scaling. This should be
        /// called on any world position vector that is to be placed in a
        /// Unity scene.
        /// </summary>
        /// <param name="vec">The vector to operate on.</param>
        /// <returns>The map unit scaled vector.</returns>
        public static Vector2 MapUnit(this Vector2 vec) => vec * Constants.MapUnit;
    }
}
