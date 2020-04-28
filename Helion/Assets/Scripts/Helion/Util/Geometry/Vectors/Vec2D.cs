using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Helion.Util.Geometry.Vectors
{
    /// <summary>
    /// An immutable 2D vector designed to go on the stack.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public readonly struct Vec2D
    {
        /// <summary>
        /// A point at the origin.
        /// </summary>
        public static readonly Vec2D Zero = new Vec2D(0, 0);

        /// <summary>
        /// The X coordinate.
        /// </summary>
        public readonly double X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// A representation of the U value when using UV coordinates.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public double U => X;

        /// <summary>
        /// A representation of the V value when using UV coordinates.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public double V => Y;

        /// <summary>
        /// Creates a new vector from an X and Y component.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        public Vec2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Allows for creation of a 2D vector from a tuple of doubles.
        /// </summary>
        /// <param name="pair">The tuple.</param>
        /// <returns>A new 2D vector.</returns>
        public static implicit operator Vec2D(ValueTuple<double, double> pair) => new Vec2D(pair.Item1, pair.Item2);

        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="self">The vector to negate.</param>
        /// <returns>The negated vector.</returns>
        public static Vec2D operator -(in Vec2D self) => new Vec2D(-self.X, -self.Y);

        /// <summary>
        /// Adds two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator +(in Vec2D self, in Vec2D other) => new Vec2D(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Adds two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator +(in Vec2D self, Vector2D other) => new Vec2D(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Adds a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator +(in Vec2D self, double other) => new Vec2D(self.X + other, self.Y + other);

        /// <summary>
        /// Subtracts two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator -(in Vec2D self, in Vec2D other) => new Vec2D(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Subtracts two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator -(in Vec2D self, Vector2D other) => new Vec2D(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Subtracts a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator -(in Vec2D self, double other) => new Vec2D(self.X - other, self.Y - other);

        /// <summary>
        /// Does a component-wise multiplication (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator *(in Vec2D self, in Vec2D other) => new Vec2D(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Does a component-wise multiplication (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator *(in Vec2D self, Vector2D other) => new Vec2D(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator *(in Vec2D self, double value) => new Vec2D(self.X * value, self.Y * value);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="value">The left side scalar.</param>
        /// <param name="self">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator *(double value, in Vec2D self) => new Vec2D(self.X * value, self.Y * value);

        /// <summary>
        /// Does a component-wise division (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator /(in Vec2D self, in Vec2D other) => new Vec2D(self.X / other.X, self.Y / other.Y);

        /// <summary>
        /// Does a component-wise division (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator /(in Vec2D self, Vector2D other) => new Vec2D(self.X / other.X, self.Y / other.Y);

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec2D operator /(in Vec2D self, double value) => new Vec2D(self.X / value, self.Y / value);

        /// <summary>
        /// Checks for bitwise equality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are equal, false if not.</returns>
        public static bool operator ==(in Vec2D self, in Vec2D other) => self.X == other.X && self.Y == other.Y;

        /// <summary>
        /// Checks for bitwise equality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are equal, false if not.</returns>
        public static bool operator ==(in Vec2D self, Vector2D other)
        {
            if (other == null)
                return false;
            return self.X == other.X && self.Y == other.Y;
        }

        /// <summary>
        /// Checks for bitwise inequality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are not equal, false if they are.</returns>
        public static bool operator !=(in Vec2D self, in Vec2D other) => !(self == other);

        /// <summary>
        /// Checks for bitwise inequality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are not equal, false if they are.</returns>
        public static bool operator !=(in Vec2D self, Vector2D other) => !(self == other);

        /// <summary>
        /// Takes some radian value and calculates the unit circle vector.
        /// </summary>
        /// <param name="radians">The radian angle.</param>
        /// <returns>A unit vector to a point on a unit circle based on the
        /// provided angle.</returns>
        public static Vec2D RadianUnit(double radians) => new Vec2D(Math.Cos(radians), Math.Sin(radians));

        /// <summary>
        /// Deconstructs a vector to a tuple.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        [Pure]
        public void Deconstruct(out double x, out double y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Creates an instance from the current struct.
        /// </summary>
        /// <returns>A new instance using the current struct values.</returns>
        [Pure]
        public Vector2D Instance() => new Vector2D(X, Y);

        /// <summary>
        /// Creates a new vector but with a new X component.
        /// </summary>
        /// <param name="x">The new X component.</param>
        /// <returns>A new vector with the X component.</returns>
        [Pure]
        public Vec2D WithX(double x) => new Vec2D(x, Y);

        /// <summary>
        /// Creates a new vector but with a new Y component.
        /// </summary>
        /// <param name="y">The new Y component.</param>
        /// <returns>A new vector with the Y component.</returns>
        [Pure]
        public Vec2D WithY(double y) => new Vec2D(X, y);

        /// <summary>
        /// Creates a new 3D vector with the Z component.
        /// </summary>
        /// <param name="z">The new Z component.</param>
        /// <returns>A new 3D vector with the Z component.</returns>
        [Pure]
        public Vec3D WithZ(double z) => new Vec3D(X, Y, z);

        /// <summary>
        /// Floors the components and returns the result.
        /// </summary>
        /// <returns>A floored vector.</returns>
        [Pure]
        public Vec2D Floor() => new Vec2D(Math.Floor(X), Math.Floor(Y));

        /// <summary>
        /// Ceiling the components and returns the result.
        /// </summary>
        /// <returns>A ceiling vector.</returns>
        [Pure]
        public Vec2D Ceil() => new Vec2D(Math.Ceiling(X), Math.Ceiling(Y));

        /// <summary>
        /// Returns an absolute version of the vector.
        /// </summary>
        /// <returns>A vector with absoluted components.</returns>
        [Pure]
        public Vec2D Abs() => new Vec2D(Math.Abs(X), Math.Abs(Y));

        /// <summary>
        /// Calculates the unit vector from this vector.
        /// </summary>
        /// <returns>A unit vector from this vector.</returns>
        [Pure]
        public Vec2D Unit() => this / Length();

        /// <summary>
        /// Calculates the dot product with another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        [Pure]
        public double Dot(in Vec2D other) => (X * other.X) + (Y * other.Y);

        /// <summary>
        /// Calculates the dot product with another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        [Pure]
        public double Dot(Vector2D other) => (X * other.X) + (Y * other.Y);

        /// <summary>
        /// Calculates the squared length.
        /// </summary>
        /// <returns>The squared length.</returns>
        [Pure]
        public double LengthSquared() => (X * X) + (Y * Y);

        /// <summary>
        /// Calculates the length of the vector (with respect to the origin).
        /// </summary>
        /// <returns>The vector length.</returns>
        [Pure]
        public double Length() => Math.Sqrt(LengthSquared());

        /// <summary>
        /// Calculates the component of the vector relative to the provided
        /// vector.
        /// </summary>
        /// <param name="onto">The vector to get the component along.</param>
        /// <returns>The component value of the vector.</returns>
        [Pure]
        public double Component(in Vec2D onto) => Dot(onto) / onto.Length();

        /// <summary>
        /// Calculates the component of the vector relative to the provided
        /// vector.
        /// </summary>
        /// <param name="onto">The vector to get the component along.</param>
        /// <returns>The component value of the vector.</returns>
        [Pure]
        public double Component(Vector2D onto) => Dot(onto) / onto.Length();

        /// <summary>
        /// Projects this vector onto the provided vector.
        /// </summary>
        /// <param name="onto">The vector to project onto.</param>
        /// <returns>The projection.</returns>
        [Pure]
        public Vec2D Projection(in Vec2D onto) => Dot(onto) / onto.LengthSquared() * onto;

        /// <summary>
        /// Projects this vector onto the provided vector.
        /// </summary>
        /// <param name="onto">The vector to project onto.</param>
        /// <returns>The projection.</returns>
        [Pure]
        public Vec2D Projection(Vector2D onto) => Dot(onto) / onto.LengthSquared() * onto.Struct();

        /// <summary>
        /// Gets the distance from this to the other vector squared.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The squared distance.</returns>
        [Pure]
        public double DistanceSquared(in Vec2D other) => (this - other).LengthSquared();

        /// <summary>
        /// Gets the distance from this to the other vector squared.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The squared distance.</returns>
        [Pure]
        public double DistanceSquared(Vector2D other) => (this - other).LengthSquared();

        /// <summary>
        /// Gets the distance from this to the other vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The distance.</returns>
        [Pure]
        public double Distance(in Vec2D other) => (this - other).Length();

        /// <summary>
        /// Gets the distance from this to the other vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The distance.</returns>
        [Pure]
        public double Distance(Vector2D other) => (this - other).Length();

        /// <summary>
        /// Interpolates between the provided vector.
        /// </summary>
        /// <param name="end">The end point.</param>
        /// <param name="t">The time, usually between [0, 1].</param>
        /// <returns>The point along the vector from this to the end vector, at
        /// the time provided.</returns>
        [Pure]
        public Vec2D Interpolate(in Vec2D end, double t) => this + (t * (end - this));

        /// <summary>
        /// Gets a new vector that is the right 90 degree rotation with respect
        /// to the origin.
        /// </summary>
        /// <returns>The right angle rotation.</returns>
        [Pure]
        public Vec2D Right90() => new Vec2D(Y, -X);

        /// <summary>
        /// Gets a new vector that is the left 90 degree rotation with respect
        /// to the origin.
        /// </summary>
        /// <returns>The left angle rotation.</returns>
        [Pure]
        public Vec2D Left90() => new Vec2D(-Y, X);

        /// <summary>
        /// Creates a floating point vector from the double coordinate vector.
        /// </summary>
        /// <returns>A 2D floating point vector.</returns>
        [Pure]
        public Vec2I Int() => new Vec2I((int)X, (int)Y);

        /// <summary>
        /// Creates a floating point vector from the double coordinate vector.
        /// </summary>
        /// <returns>A 2D floating point vector.</returns>
        [Pure]
        public Vector2 Float() => new Vector2((float)X, (float)Y);

        /// <summary>
        /// Checks if it is equal to another vector. Does an Equals()
        /// comparison using the two doubles.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>True if so, false if not.</returns>
        public bool Equals(in Vec2D other) => X.Equals(other.X) && Y.Equals(other.Y);

        public override bool Equals(object obj) => obj is Vec2D other && Equals(other);

        public override int GetHashCode() => HashCodes.Hash(X.GetHashCode(), Y.GetHashCode());

        public override string ToString() => $"{X}, {Y}";
    }
}
