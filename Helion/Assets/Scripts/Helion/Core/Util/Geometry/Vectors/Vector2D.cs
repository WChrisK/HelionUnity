using System;
using System.Diagnostics;
using UnityEngine;

namespace Helion.Core.Util.Geometry.Vectors
{
    /// <summary>
    /// A mutable 2D vector.
    /// </summary>
    public class Vector2D
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public double Y;

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
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Creates a new vector from an X and Y vector.
        /// </summary>
        /// <param name="vec">The 2D vector to get components from.</param>
        public Vector2D(in Vec2D vec)
        {
            X = vec.X;
            Y = vec.Y;
        }

        /// <summary>
        /// Allows for creation of a 2D vector from a tuple of doubles.
        /// </summary>
        /// <param name="pair">The tuple.</param>
        /// <returns>A new 2D vector.</returns>
        public static implicit operator Vector2D(ValueTuple<double, double> pair)
        {
            return new Vector2D(pair.Item1, pair.Item2);
        }

        /// <summary>
        /// Creates a new vector at the origin.
        /// </summary>
        /// <returns>A new vector, at the origin.</returns>
        public static Vector2D FromZero() => new Vector2D(0, 0);

        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="self">The vector to negate.</param>
        /// <returns>The negated vector.</returns>
        public static Vector2D operator -(Vector2D self) => new Vector2D(-self.X, -self.Y);

        /// <summary>
        /// Adds two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator +(Vector2D self, in Vec2D other) => new Vector2D(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Adds two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator +(Vector2D self, Vector2D other) => new Vector2D(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Adds a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator +(Vector2D self, double other) => new Vector2D(self.X + other, self.Y + other);

        /// <summary>
        /// Subtracts two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator -(Vector2D self, in Vec2D other) => new Vector2D(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Subtracts two vectors together for a new result.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator -(Vector2D self, Vector2D other) => new Vector2D(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Subtracts a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator -(Vector2D self, double other) => new Vector2D(self.X - other, self.Y - other);

        /// <summary>
        /// Does a component-wise multiplication (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator *(Vector2D self, in Vec2D other) => new Vector2D(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Does a component-wise multiplication (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator *(Vector2D self, Vector2D other) => new Vector2D(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator *(Vector2D self, double value) => new Vector2D(self.X * value, self.Y * value);

        /// <summary>
        /// Multiplies a scalar to a vector.
        /// </summary>
        /// <param name="value">The left side scalar.</param>
        /// <param name="self">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator *(double value, Vector2D self) => new Vector2D(self.X * value, self.Y * value);

        /// <summary>
        /// Does a component-wise division (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator /(Vector2D self, in Vec2D other) => new Vector2D(self.X / other.X, self.Y / other.Y);

        /// <summary>
        /// Does a component-wise division (like the Hadamard product).
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator /(Vector2D self, Vector2D other) => new Vector2D(self.X / other.X, self.Y / other.Y);

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="value">The right side scalar.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D operator /(Vector2D self, double value) => new Vector2D(self.X / value, self.Y / value);

        /// <summary>
        /// Checks for bitwise equality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are equal, false if not or if any argument is
        /// null.</returns>
        public static bool operator ==(Vector2D self, Vector2D other)
        {
            bool selfNull = ReferenceEquals(self, null);
            bool otherNull = ReferenceEquals(other, null);
            if (selfNull && otherNull)
                return true;
            if (selfNull || otherNull)
                return false;
            return self.X == other.X && self.Y == other.Y;
        }

        /// <summary>
        /// Checks for bitwise equality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are equal, false if not.</returns>
        public static bool operator ==(Vector2D self, Vec2D other)
        {
            if (self == null)
                return false;
            return self.X == other.X && self.Y == other.Y;
        }

        /// <summary>
        /// Checks for bitwise inequality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are not equal, false if they are.</returns>
        public static bool operator !=(Vector2D self, in Vec2D other) => !(self == other);

        /// <summary>
        /// Checks for bitwise inequality between components.
        /// </summary>
        /// <param name="self">The left side vector.</param>
        /// <param name="other">The right side vector.</param>
        /// <returns>True if they are not equal, false if they are.</returns>
        public static bool operator !=(Vector2D self, Vector2D other) => !(self == other);

        /// <summary>
        /// Takes some radian value and calculates the unit circle vector.
        /// </summary>
        /// <param name="radians">The radian angle.</param>
        /// <returns>A unit vector to a point on a unit circle based on the
        /// provided angle.</returns>
        public static Vector2D RadianUnit(double radians) => new Vector2D(Math.Cos(radians), Math.Sin(radians));

        /// <summary>
        /// Deconstructs a vector to a tuple.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void Deconstruct(out double x, out double y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Creates an struct value from the current instance.
        /// </summary>
        /// <returns>A new struct using the current instance values.</returns>
        public Vec2D Struct() => new Vec2D(X, Y);

        /// <summary>
        /// Creates a new vector but with a new X component.
        /// </summary>
        /// <param name="x">The new X component.</param>
        /// <returns>A new vector with the X component.</returns>
        public Vector2D WithX(double x) => new Vector2D(x, Y);

        /// <summary>
        /// Creates a new vector but with a new Y component.
        /// </summary>
        /// <param name="y">The new Y component.</param>
        /// <returns>A new vector with the Y component.</returns>
        public Vector2D WithY(double y) => new Vector2D(X, y);

        /// <summary>
        /// Creates a new 3D vector with the Z component.
        /// </summary>
        /// <param name="z">The new Z component.</param>
        /// <returns>A new 3D vector with the Z component.</returns>
        public Vector3D WithZ(double z) => new Vector3D(X, Y, z);

        /// <summary>
        /// Adds who vectors together. This can be used when inheritance and
        /// operator overloading do not play nicely. This avoids needing to
        /// convert the object to a struct.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>A new vector with the components added.</returns>
        public Vec2D Add(Vector2D other)
        {
            return new Vec2D(this.X + other.X, this.Y + other.Y);
        }

        /// <summary>
        /// Subtracts one vector from the other. This can be used when
        /// inheritance and operator overloading do not play nicely. This
        /// avoids needing to convert the object to a struct.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>A new vector with the subtraction result.</returns>
        public Vec2D Subtract(Vector2D other)
        {
            return new Vec2D(this.X - other.X, this.Y - other.Y);
        }

        /// <summary>
        /// Floors the components and returns the result.
        /// </summary>
        /// <returns>A floored vector.</returns>
        public Vector2D Floor() => new Vector2D(Math.Floor(X), Math.Floor(Y));

        /// <summary>
        /// Ceiling the components and returns the result.
        /// </summary>
        /// <returns>A ceiling vector.</returns>
        public Vector2D Ceil() => new Vector2D(Math.Ceiling(X), Math.Ceiling(Y));

        /// <summary>
        /// Returns an absolute version of the vector.
        /// </summary>
        /// <returns>A vector with absoluted components.</returns>
        public Vector2D Abs() => new Vector2D(Math.Abs(X), Math.Abs(Y));

        /// <summary>
        /// Calculates the unit vector from this vector. Creates a new vector;
        /// if you want to normalize mutably then use <see cref="Normalize"/>.
        /// </summary>
        /// <returns>A unit vector from this vector.</returns>
        public Vector2D Unit() => this / Length();

        /// <summary>
        /// Normalizes the current vector.
        /// </summary>
        public void Normalize()
        {
            double length = Length();
            X /= length;
            Y /= length;
        }

        /// <summary>
        /// Calculates the dot product with another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        public double Dot(in Vec2D other) => (X * other.X) + (Y * other.Y);

        /// <summary>
        /// Calculates the dot product with another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        public double Dot(Vector2D other) => (X * other.X) + (Y * other.Y);

        /// <summary>
        /// Calculates the squared length.
        /// </summary>
        /// <returns>The squared length.</returns>
        public double LengthSquared() => (X * X) + (Y * Y);

        /// <summary>
        /// Calculates the length of the vector (with respect to the origin).
        /// </summary>
        /// <returns>The vector length.</returns>
        public double Length() => Math.Sqrt(LengthSquared());

        /// <summary>
        /// Calculates the component of the vector relative to the provided
        /// vector.
        /// </summary>
        /// <param name="onto">The vector to get the component along.</param>
        /// <returns>The component value of the vector.</returns>
        public double Component(in Vec2D onto) => Dot(onto) / onto.Length();

        /// <summary>
        /// Calculates the component of the vector relative to the provided
        /// vector.
        /// </summary>
        /// <param name="onto">The vector to get the component along.</param>
        /// <returns>The component value of the vector.</returns>
        public double Component(Vector2D onto) => Dot(onto) / onto.Length();

        /// <summary>
        /// Projects this vector onto the provided vector.
        /// </summary>
        /// <param name="onto">The vector to project onto.</param>
        /// <returns>The projection.</returns>
        public Vector2D Projection(in Vec2D onto) => (Dot(onto) / onto.LengthSquared() * onto).Instance();

        /// <summary>
        /// Projects this vector onto the provided vector.
        /// </summary>
        /// <param name="onto">The vector to project onto.</param>
        /// <returns>The projection.</returns>
        public Vector2D Projection(Vector2D onto) => Dot(onto) / onto.LengthSquared() * onto;

        /// <summary>
        /// Gets the distance from this to the other vector squared.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The squared distance.</returns>
        public double DistanceSquared(in Vec2D other) => (this - other).LengthSquared();

        /// <summary>
        /// Gets the distance from this to the other vector squared.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The squared distance.</returns>
        public double DistanceSquared(Vector2D other) => (this - other).LengthSquared();

        /// <summary>
        /// Gets the distance from this to the other vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The distance.</returns>
        public double Distance(in Vec2D other) => (this - other).Length();

        /// <summary>
        /// Gets the distance from this to the other vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The distance.</returns>
        public double Distance(Vector2D other) => (this - other).Length();

        /// <summary>
        /// Interpolates between the provided vector.
        /// </summary>
        /// <param name="end">The end point.</param>
        /// <param name="t">The time, usually between [0, 1].</param>
        /// <returns>The point along the vector from this to the end vector, at
        /// the time provided.</returns>
        public Vector2D Interpolate(in Vec2D end, double t) => this + (t * (end - this));

        /// <summary>
        /// Gets a new vector that is the right 90 degree rotation with respect
        /// to the origin.
        /// </summary>
        /// <returns>The right angle rotation.</returns>
        public Vector2D Right90() => new Vector2D(Y, -X);

        /// <summary>
        /// Gets a new vector that is the left 90 degree rotation with respect
        /// to the origin.
        /// </summary>
        /// <returns>The left angle rotation.</returns>
        public Vector2D Left90() => new Vector2D(-Y, X);

        /// <summary>
        /// Creates an integer vector from the current vector.
        /// </summary>
        /// <remarks>
        /// The coordinates are generated by casting them to an integer.
        /// </remarks>
        /// <returns>A new integer vector.</returns>
        public Vec2I Int() => new Vec2I((int)X, (int)Y);

        /// <summary>
        /// Creates a floating point vector from the double coordinate vector.
        /// </summary>
        /// <returns>A 2D floating point vector.</returns>
        public Vector2 Float() => new Vector2((float)X, (float)Y);

        /// <summary>
        /// Checks if it is equal to another vector. Does an Equals()
        /// comparison using the two doubles.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>True if so, false if not.</returns>
        public bool Equals(Vector2D other) => X.Equals(other.X) && Y.Equals(other.Y);

        public override bool Equals(object obj) => obj is Vector2D other && Equals(other);

        // TODO: This is probably not good.
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        public override string ToString() => $"{X}, {Y}";
    }
}
