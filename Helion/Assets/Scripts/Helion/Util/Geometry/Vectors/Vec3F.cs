using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Helion.Util.Geometry.Vectors
{
    /// <summary>
    /// An immutable 3D vector designed for the stack.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct Vec3F
    {
        /// <summary>
        /// A point at the origin.
        /// </summary>
        public static readonly Vec3F Zero = new Vec3F(0, 0, 0);

        /// <summary>
        /// The X coordinate.
        /// </summary>
        public readonly float X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public readonly float Y;

        /// <summary>
        /// The Z coordinate.
        /// </summary>
        public readonly float Z;

        /// <summary>
        /// Gets a 2D vector from the X/Z coordinates. This is the same as
        /// projecting it from the birds eye view in the map.
        /// </summary>
        public Vec2F XZ => new Vec2F(X, Z);

        /// <summary>
        /// Creates a new vector from an X and Y component.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public Vec3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Allows for creation of a 3D vector from a tuple of floats.
        /// </summary>
        /// <param name="pair">The tuple.</param>
        /// <returns>A new 3D vector.</returns>
        public static implicit operator Vec3F(ValueTuple<float, float, float> pair)
        {
            return new Vec3F(pair.Item1, pair.Item2, pair.Item3);
        }

        /// <summary>
        /// Allows for casting of our floating point vector to Unity's type.
        /// </summary>
        /// <param name="vector">The current vector.</param>
        /// <returns>A new 3D vector in Unity's struct.</returns>
        public static implicit operator Vector3(Vec3F vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="self">The vector to negate.</param>
        /// <returns>The negated vector.</returns>
        public static Vec3F operator -(in Vec3F self) => new Vec3F(-self.X, -self.Y, -self.Z);

        /// <summary>
        /// Adds two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator +(in Vec3F self, in Vec3F other) => new Vec3F(self.X + other.X, self.Y + other.Y, self.Z + other.Z);

        /// <summary>
        /// Adds a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator +(in Vec3F self, float other) => new Vec3F(self.X + other, self.Y + other, self.Z + other);

        /// <summary>
        /// Subtracts two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator -(in Vec3F self, in Vec3F other) => new Vec3F(self.X - other.X, self.Y - other.Y, self.Z - other.Z);

        /// <summary>
        /// Subtracts a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator -(in Vec3F self, float other) => new Vec3F(self.X - other, self.Y - other, self.Z - other);

        /// <summary>
        /// Does a component-wise multiplication (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator *(in Vec3F self, in Vec3F other) => new Vec3F(self.X * other.X, self.Y * other.Y, self.Z * other.Z);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator *(in Vec3F self, float value) => new Vec3F(self.X * value, self.Y * value, self.Z * value);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="value">The left side scalar.</param>
        /// <param name="self">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator *(float value, in Vec3F self) => new Vec3F(self.X * value, self.Y * value, self.Z * value);

        /// <summary>
        /// Does a component-wise division (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator /(in Vec3F self, in Vec3F other) => new Vec3F(self.X / other.X, self.Y / other.Y, self.Z / other.Z);

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator /(in Vec3F self, float value) => self * (1.0f / value);

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="value">The left side scalar.</param>
        /// <param name="self">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vec3F operator /(float value, in Vec3F self) => self * (1.0f / value);

        /// <summary>
        /// Checks for bitwise equality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are equal, false if not.</returns>
        public static bool operator ==(in Vec3F self, in Vec3F other) => self.X == other.X && self.Y == other.Y && self.Z == other.Z;

        /// <summary>
        /// Checks for bitwise inequality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are not equal, false if they are.</returns>
        public static bool operator !=(in Vec3F self, in Vec3F other) => !(self == other);

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
        public static Vec3F SphereUnit(float yawRadians, float pitchRadians)
        {
            float cosYaw = Mathf.Cos(yawRadians);
            float sinYaw = Mathf.Sin(yawRadians);
            float sinPitch = Mathf.Sin(pitchRadians);

            return new Vec3F(sinPitch * cosYaw, sinPitch * sinYaw, cosYaw);
        }

        /// <summary>
        /// Gets the vector on the XZ plane from the radians. This is defined
        /// as the +X axis being zero degrees and rotating counter-clockwise.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <returns>A vector with a zero Y component.</returns>
        public static Vec3F CircleUnit(float radians)
        {
            return new Vec3F(Mathf.Cos(radians), 0, Mathf.Sin(radians));
        }

        /// <summary>
        /// Gets the vector on the XZ plane from the degrees. This is defined
        /// as the +X axis being zero degrees and rotating counter-clockwise.
        /// </summary>
        /// <param name="degrees">The degrees.</param>
        /// <returns>A vector with a zero Y component.</returns>
        public static Vec3F CircleUnitDeg(float degrees) => CircleUnit(degrees * Mathf.Deg2Rad);

        /// <summary>
        /// Deconstructs a vector to a tuple.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public void Deconstruct(out float x, out float y, out float z)
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
        /// Creates a new vector but with a new X component.
        /// </summary>
        /// <param name="x">The new X component.</param>
        /// <returns>A new vector with the X component.</returns>
        [Pure]
        public Vec3F WithX(float x) => new Vec3F(x, Y, Z);

        /// <summary>
        /// Creates a new vector but with a new Y component.
        /// </summary>
        /// <param name="y">The new Y component.</param>
        /// <returns>A new vector with the Y component.</returns>
        [Pure]
        public Vec3F WithY(float y) => new Vec3F(X, y, Z);

        /// <summary>
        /// Creates a new vector but with a new Y component.
        /// </summary>
        /// <param name="z">The new z component.</param>
        /// <returns>A new vector with the Y component.</returns>
        [Pure]
        public Vec3F WithZ(float z) => new Vec3F(X, Y, z);

        /// <summary>
        /// Returns a vector with its X set to the incoming X, and Z set to the
        /// incoming Y.
        /// </summary>
        /// <param name="x">The new X value.</param>
        /// <param name="z">The new X value.</param>
        /// <returns>A new vector with the X/Z values set, and the Y value is
        /// unchanged.</returns>
        [Pure]
        public Vec3F WithXZ(float x, float z) => new Vec3F(x, Y, z);

        /// <summary>
        /// Returns a vector with its X set to the incoming X, and Z set to the
        /// incoming Y.
        /// </summary>
        /// <param name="vec">The X/Y values to map onto X/Z.</param>
        /// <returns>A new vector with the X/Z values set, and the Y value is
        /// unchanged.</returns>
        [Pure]
        public Vec3F WithXZ(in Vec2F vec) => new Vec3F(vec.X, Y, vec.Y);

        /// <summary>
        /// Floors the components and returns the result.
        /// </summary>
        /// <returns>A floored vector.</returns>
        [Pure]
        public Vec3F Floor() => new Vec3F(Mathf.Floor(X), Mathf.Floor(Y), Mathf.Floor(Z));

        /// <summary>
        /// Ceiling the components and returns the result.
        /// </summary>
        /// <returns>A ceiling vector.</returns>
        [Pure]
        public Vec3F Ceil() => new Vec3F(Mathf.Ceil(X), Mathf.Ceil(Y), Mathf.Ceil(Z));

        /// <summary>
        /// Returns an absolute version of the vector.
        /// </summary>
        /// <returns>A vector with absoluted components.</returns>
        [Pure]
        public Vec3F Abs() => new Vec3F(Mathf.Abs(X), Mathf.Abs(Y), Mathf.Abs(Z));

        /// <summary>
        /// Calculates the unit vector from this vector.
        /// </summary>
        /// <returns>A unit vector from this vector.</returns>
        [Pure]
        public Vec3F Unit() => this / Length();

        /// <summary>
        /// Calculates the dot product with another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        [Pure]
        public float Dot(in Vec3F other) => (X * other.X) + (Y * other.Y) + (Z * other.Z);

        /// <summary>
        /// Calculates the squared length.
        /// </summary>
        /// <returns>The squared length.</returns>
        [Pure]
        public float LengthSquared() => (X * X) + (Y * Y) + (Z * Z);

        /// <summary>
        /// Calculates the length of the vector (with respect to the origin).
        /// </summary>
        /// <returns>The vector length.</returns>
        [Pure]
        public float Length() => Mathf.Sqrt(LengthSquared());

        /// <summary>
        /// Calculates the component of the vector relative to the provided
        /// vector.
        /// </summary>
        /// <param name="onto">The vector to get the component along.</param>
        /// <returns>The component value of the vector.</returns>
        [Pure]
        public float Component(in Vec3F onto) => Dot(onto) / onto.Length();

        /// <summary>
        /// Projects this vector onto the provided vector.
        /// </summary>
        /// <param name="onto">The vector to project onto.</param>
        /// <returns>The projection.</returns>
        [Pure]
        public Vec3F Projection(in Vec3F onto) => Dot(onto) / onto.LengthSquared() * onto;

        /// <summary>
        /// Gets the distance from this to the other vector squared.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The squared distance.</returns>
        [Pure]
        public float DistanceSquared(in Vec3F other) => (this - other).LengthSquared();

        /// <summary>
        /// Gets the distance from this to the other vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The distance.</returns>
        [Pure]
        public float Distance(in Vec3F other) => (this - other).Length();

        /// <summary>
        /// Interpolates between the provided vector.
        /// </summary>
        /// <param name="end">The end point.</param>
        /// <param name="t">The time, usually between [0, 1].</param>
        /// <returns>The point along the vector from this to the end vector, at
        /// the time provided.</returns>
        [Pure]
        public Vec3F Interpolate(in Vec3F end, float t) => this + (t * (end - this));

        /// <summary>
        /// Creates a floating point vector from the float coordinate vector.
        /// </summary>
        /// <returns>A 3D floating point vector.</returns>
        [Pure]
        public Vector3 UnityFloat() => new Vector3((float)X, (float)Y, (float)Z);

        /// <summary>
        /// Converts to a Unity vector and then converts to a map unity by
        /// multiplying it by the factor.
        /// </summary>
        /// <returns>The map unit representation.</returns>
        [Pure]
        public Vector3 MapUnit() => UnityFloat() * Constants.MapUnit;

        /// <summary>
        /// Checks if it is equal to another vector. Does an Equals()
        /// comparison using the two floats.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>True if so, false if not.</returns>
        public bool Equals(in Vec3F other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        public override bool Equals(object obj) => obj is Vec3F other && Equals(other);

        // TODO: This is probably not good.
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

        public override string ToString() => $"{X}, {Y}, {Z}";
    }
}
