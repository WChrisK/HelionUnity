using System;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Helion.Util.Geometry.Vectors
{
    /// <summary>
    /// An immutable 3D vector designed for the stack.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public readonly struct Vec3D
    {
        /// <summary>
        /// A point at the origin.
        /// </summary>
        public static readonly Vec3D Zero = new Vec3D(0, 0, 0);

        /// <summary>
        /// The X coordinate.
        /// </summary>
        public readonly double X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// The Z coordinate.
        /// </summary>
        public readonly double Z;

        /// <summary>
        /// Creates a new vector from an X and Y component.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public Vec3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Allows for creation of a 3D vector from a tuple of doubles.
        /// </summary>
        /// <param name="pair">The tuple.</param>
        /// <returns>A new 3D vector.</returns>
        public static implicit operator Vec3D(ValueTuple<double, double, double> pair)
        {
            return new Vec3D(pair.Item1, pair.Item2, pair.Item3);
        }

        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="self">The vector to negate.</param>
        /// <returns>The negated vector.</returns>
        public static Vec3D operator -(in Vec3D self) => new Vec3D(-self.X, -self.Y, -self.Z);

        /// <summary>
        /// Adds two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator +(in Vec3D self, in Vec3D other) => new Vec3D(self.X + other.X, self.Y + other.Y, self.Z + other.Z);

        /// <summary>
        /// Adds a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator +(in Vec3D self, double other) => new Vec3D(self.X + other, self.Y + other, self.Z + other);

        /// <summary>
        /// Subtracts two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator -(in Vec3D self, in Vec3D other) => new Vec3D(self.X - other.X, self.Y - other.Y, self.Z - other.Z);

        /// <summary>
        /// Subtracts a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator -(in Vec3D self, double other) => new Vec3D(self.X - other, self.Y - other, self.Z - other);

        /// <summary>
        /// Does a component-wise multiplication (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator *(in Vec3D self, in Vec3D other) => new Vec3D(self.X * other.X, self.Y * other.Y, self.Z * other.Z);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator *(in Vec3D self, double value) => new Vec3D(self.X * value, self.Y * value, self.Z * value);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="value">The left side scalar.</param>
        /// <param name="self">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator *(double value, in Vec3D self) => new Vec3D(self.X * value, self.Y * value, self.Z * value);

        /// <summary>
        /// Does a component-wise division (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator /(in Vec3D self, in Vec3D other) => new Vec3D(self.X / other.X, self.Y / other.Y, self.Z / other.Z);

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator /(in Vec3D self, double value) => self * (1.0 / value);

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="value">The left side scalar.</param>
        /// <param name="self">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3D operator /(double value, in Vec3D self) => self * (1.0 / value);

        /// <summary>
        /// Checks for bitwise equality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are equal, false if not.</returns>
        public static bool operator ==(in Vec3D self, in Vec3D other) => self.X == other.X && self.Y == other.Y && self.Z == other.Z;

        /// <summary>
        /// Checks for bitwise inequality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are not equal, false if they are.</returns>
        public static bool operator !=(in Vec3D self, in Vec3D other) => !(self == other);

        /// <summary>
        /// Takes some radian value and calculates the unit circle vector. This
        /// is for Z pointing up/down.
        /// </summary>
        /// <remarks>
        /// This uses spherical coordinates, so the pitch must range from 0..pi
        /// radians; 0 radians is the sphere's top and pi is the bottom.
        /// </remarks>
        /// <param name="yawRadians">The horizontal radian angle.</param>
        /// <param name="pitchRadians">The vertical radian angle.</param>
        /// <returns>A unit vector to a point on a unit circle based on the
        /// provided angles.</returns>
        public static Vec3D SphereUnit(double yawRadians, double pitchRadians)
        {
            double cosYaw = Math.Cos(yawRadians);
            double sinYaw = Math.Sin(yawRadians);
            double sinPitch = Math.Sin(pitchRadians);

            return new Vec3D(sinPitch * cosYaw, sinPitch * sinYaw, cosYaw);
        }

        /// <summary>
        /// Deconstructs a vector to a tuple.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public void Deconstruct(out double x, out double y, out double z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        /// <summary>
        /// Creates a class version of this vector.
        /// </summary>
        /// <returns>The class version of this vector.</returns>
        public Vector3D ToClass() => new Vector3D(X, Y, Z);

        /// <summary>
        /// Converts to a 2D vector by dropping the Z component.
        /// </summary>
        /// <returns>The 2D vector without the Z component.</returns>
        [Pure]
        public Vec2D To2D() => new Vec2D(X, Y);

        /// <summary>
        /// Creates a new vector but with a new X component.
        /// </summary>
        /// <param name="x">The new X component.</param>
        /// <returns>A new vector with the X component.</returns>
        [Pure]
        public Vec3D WithX(double x) => new Vec3D(x, Y, Z);

        /// <summary>
        /// Creates a new vector but with a new Y component.
        /// </summary>
        /// <param name="y">The new Y component.</param>
        /// <returns>A new vector with the Y component.</returns>
        [Pure]
        public Vec3D WithY(double y) => new Vec3D(X, y, Z);

        /// <summary>
        /// Creates a new vector but with a new Y component.
        /// </summary>
        /// <param name="z">The new z component.</param>
        /// <returns>A new vector with the Y component.</returns>
        [Pure]
        public Vec3D WithZ(double z) => new Vec3D(X, Y, z);

        /// <summary>
        /// Floors the components and returns the result.
        /// </summary>
        /// <returns>A floored vector.</returns>
        [Pure]
        public Vec3D Floor() => new Vec3D(Math.Floor(X), Math.Floor(Y), Math.Floor(Z));

        /// <summary>
        /// Ceiling the components and returns the result.
        /// </summary>
        /// <returns>A ceiling vector.</returns>
        [Pure]
        public Vec3D Ceil() => new Vec3D(Math.Ceiling(X), Math.Ceiling(Y), Math.Ceiling(Z));

        /// <summary>
        /// Returns an absolute version of the vector.
        /// </summary>
        /// <returns>A vector with absoluted components.</returns>
        [Pure]
        public Vec3D Abs() => new Vec3D(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));

        /// <summary>
        /// Calculates the unit vector from this vector.
        /// </summary>
        /// <returns>A unit vector from this vector.</returns>
        [Pure]
        public Vec3D Unit() => this / Length();

        /// <summary>
        /// Calculates the dot product with another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        [Pure]
        public double Dot(in Vec3D other) => (X * other.X) + (Y * other.Y) + (Z * other.Z);

        /// <summary>
        /// Calculates the squared length.
        /// </summary>
        /// <returns>The squared length.</returns>
        [Pure]
        public double LengthSquared() => (X * X) + (Y * Y) + (Z * Z);

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
        public double Component(in Vec3D onto) => Dot(onto) / onto.Length();

        /// <summary>
        /// Projects this vector onto the provided vector.
        /// </summary>
        /// <param name="onto">The vector to project onto.</param>
        /// <returns>The projection.</returns>
        [Pure]
        public Vec3D Projection(in Vec3D onto) => Dot(onto) / onto.LengthSquared() * onto;

        /// <summary>
        /// Gets the distance from this to the other vector squared.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The squared distance.</returns>
        [Pure]
        public double DistanceSquared(in Vec3D other) => (this - other).LengthSquared();

        /// <summary>
        /// Gets the distance from this to the other vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The distance.</returns>
        [Pure]
        public double Distance(in Vec3D other) => (this - other).Length();

        /// <summary>
        /// Interpolates between the provided vector.
        /// </summary>
        /// <param name="end">The end point.</param>
        /// <param name="t">The time, usually between [0, 1].</param>
        /// <returns>The point along the vector from this to the end vector, at
        /// the time provided.</returns>
        [Pure]
        public Vec3D Interpolate(in Vec3D end, double t) => this + (t * (end - this));

        /// <summary>
        /// Creates a floating point vector from the double coordinate vector.
        /// </summary>
        /// <returns>A 3D floating point vector.</returns>
        /// [Pure]
        public Vector3 Float() => new Vector3((float)X, (float)Y, (float)Z);

        /// <summary>
        /// Checks if it is equal to another vector. Does an Equals()
        /// comparison using the two doubles.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>True if so, false if not.</returns>
        public bool Equals(in Vec3D other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        public override bool Equals(object obj) => obj is Vec3D other && Equals(other);

        // TODO: This is probably not good.
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

        public override string ToString() => $"{X}, {Y}, {Z}";
    }
}
