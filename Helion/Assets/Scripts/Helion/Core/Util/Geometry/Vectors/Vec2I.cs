using System;
using UnityEngine;

namespace Helion.Core.Util.Geometry
{
    public struct Vec2I
    {
        /// <summary>
        /// A point at the origin.
        /// </summary>
        public static readonly Vec2I Zero = new Vec2I(0, 0);

        /// <summary>
        /// The X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public readonly int Y;

         /// <summary>
        /// Creates a new vector from an X and Y component.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        public Vec2I(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Allows for creation of a 2D vector from a tuple of doubles.
        /// </summary>
        /// <param name="pair">The tuple.</param>
        /// <returns>A new 2D vector.</returns>
        public static implicit operator Vec2I(ValueTuple<int, int> pair) => new Vec2I(pair.Item1, pair.Item2);

        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="self">The vector to negate.</param>
        /// <returns>The negated vector.</returns>
        public static Vec2I operator -(in Vec2I self) => new Vec2I(-self.X, -self.Y);

        /// <summary>
        /// Adds two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2I operator +(in Vec2I self, in Vec2I other) => new Vec2I(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Adds a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2I operator +(in Vec2I self, int other) => new Vec2I(self.X + other, self.Y + other);

        /// <summary>
        /// Subtracts two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2I operator -(in Vec2I self, in Vec2I other) => new Vec2I(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Subtracts a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2I operator -(in Vec2I self, int other) => new Vec2I(self.X - other, self.Y - other);

        /// <summary>
        /// Does a component-wise multiplication (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2I operator *(in Vec2I self, in Vec2I other) => new Vec2I(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2I operator *(in Vec2I self, int value) => new Vec2I(self.X * value, self.Y * value);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="value">The left side scalar.</param>
        /// <param name="self">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2I operator *(int value, in Vec2I self) => new Vec2I(self.X * value, self.Y * value);

        /// <summary>
        /// Divides a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2I operator /(in Vec2I self, int value) => new Vec2I(self.X / value, self.Y / value);

        /// <summary>
        /// Divides a vector component-wise.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2I operator /(in Vec2I self, in Vec2I other) => new Vec2I(self.X / other.X, self.Y / other.Y);

        /// <summary>
        /// Checks for bitwise equality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are equal, false if not.</returns>
        public static bool operator ==(in Vec2I self, in Vec2I other) => self.X == other.X && self.Y == other.Y;

        /// <summary>
        /// Checks for bitwise inequality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are not equal, false if they are.</returns>
        public static bool operator !=(in Vec2I self, in Vec2I other) => !(self == other);

        /// <summary>
        /// Deconstructs a vector to a tuple.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Creates a new vector with a new X component, while using
        /// the Y component of the current vector.
        /// </summary>
        /// <param name="x">The new X value.</param>
        /// <returns>A copied vector but with a new X value.</returns>
        public Vec2I WithX(int x) => new Vec2I(x, Y);

        /// <summary>
        /// Creates a new vector with a new Y component, while using
        /// the X component of the current vector.
        /// </summary>
        /// <param name="y">The new Y value.</param>
        /// <returns>A copied vector but with a new Y value.</returns>
        public Vec2I WithY(int y) => new Vec2I(X, y);

        /// <summary>
        /// Gets the vector in float format.
        /// </summary>
        /// <returns>A floating point vector.</returns>
        public Vector2 Float() => new Vector2(X, Y);

        public override string ToString() => $"{X}, {Y}";

        public override bool Equals(object obj) => obj is Vec2I v && X == v.X && Y == v.Y;

        public override int GetHashCode() => X ^ Y;
    }
}
