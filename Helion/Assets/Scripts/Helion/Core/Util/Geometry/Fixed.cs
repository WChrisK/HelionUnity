using System;

namespace Helion.Core.Util.Geometry
{
    /// <summary>
    /// An implementation of a 16.16 fixed point number as a 32-bit integer.
    /// </summary>
    /// <remarks>
    /// Certain operators have been left off of this class, in particular the
    /// multiplication and division of this by doubles and floats. This is
    /// because we don't want them to be possibly mixed up by mistake and the
    /// user ends up doing some operation they don't want. If a user does want
    /// to do arithmetic between fixed and float/double, they will have to do
    /// a manual opt-in by converting it to the other type. This way, there are
    /// no surprises. For convenience however, multiplication will be supported
    /// with integer values.
    /// </remarks>
    public struct Fixed
    {
        /// <summary>
        /// How many bits make up a single fractional unit.
        /// </summary>
        public const int UnitBits = 16;

        /// <summary>
        /// The mask for the lower fractional bits.
        /// </summary>
        public const int FractionalMask = 0x0000FFFF;

        /// <summary>
        /// The mask for the upper integral bits.
        /// </summary>
        public const int IntegralMask = FractionalMask << UnitBits;

        /// <summary>
        /// A small epsilon value that can be used in comparisons.
        /// </summary>
        public static readonly Fixed Epsilon = new Fixed(0x00000008);

        /// <summary>
        /// A representation of 0.0.
        /// </summary>
        public static readonly Fixed Zero = FromInt(0);

        /// <summary>
        /// A representation of 1.0.
        /// </summary>
        public static readonly Fixed One = FromInt(1);

        /// <summary>
        /// A representation of -1.0.
        /// </summary>
        public static readonly Fixed NegativeOne = FromInt(-1);

        /// <summary>
        /// The most negative fixed point value.
        /// </summary>
        public static readonly Fixed Lowest = new Fixed(0x80000000);

        /// <summary>
        /// The largest fixed point value.
        /// </summary>
        public static readonly Fixed Max = new Fixed(0x7FFFFFFF);

        /// <summary>
        /// The bits that make up the fixed point number.
        /// </summary>
        public readonly int Bits;

        /// <summary>
        /// Gets the integral part of the fixed point number.
        /// </summary>
        /// <remarks>
        /// If the number is 2.5 in fixed point, this return 2.
        /// </remarks>
        public int Integral => Bits >> 16;

        /// <summary>
        /// Gets the integral part of the fixed point number.
        /// </summary>
        /// <remarks>
        /// If the number is 2.5 in fixed point, this return 0.5 in fixed point
        /// or what would be the lower 16 bits.
        /// </remarks>
        public int Fractional => Bits & 0x0000FFFF;

        /// <summary>
        /// Creates a fixed point number from the bits provided.
        /// </summary>
        /// <param name="bits">The bits to make up the number.</param>
        public Fixed(int bits) => Bits = bits;

        /// <summary>
        /// Creates a fixed point value from the number provided.
        /// </summary>
        /// <param name="f">The floating point value to convert.</param>
        public Fixed(float f) : this((int)(f * 65536.0f))
        {
        }

        /// <summary>
        /// Creates a fixed point value from the number provided.
        /// </summary>
        /// <param name="d">The double to convert.</param>
        public Fixed(double d) : this((int)(d * 65536.0))
        {
        }

        /// <summary>
        /// Creates a fixed point value from and upper 16 and lower 16 bit set
        /// of value.
        /// </summary>
        /// <param name="upper">The upper 16 bits.</param>
        /// <param name="lower">The lower 16 bits.</param>
        public Fixed(short upper, ushort lower) : this(BitsFromUpperAndLower(upper, lower))
        {
        }

        /// <summary>
        /// Takes an integer and turns it into fixed point. This means a value
        /// of 24 would become 24.0 in fixed point, not 24/65536.
        /// </summary>
        /// <param name="i">The integer to make into fixed point.</param>
        /// <returns>The fixed point value for the integer.</returns>
        public static Fixed FromInt(int i) => new Fixed(i << UnitBits);

        public static Fixed operator -(Fixed value) => new Fixed(-value.Bits);

        public static Fixed operator +(Fixed self, Fixed other) => new Fixed(self.Bits + other.Bits);

        public static Fixed operator +(Fixed self, int value) => new Fixed(self.Bits + (value << UnitBits));

        public static Fixed operator -(Fixed self, Fixed other) => new Fixed(self.Bits - other.Bits);

        public static Fixed operator -(Fixed self, int value) => new Fixed(self.Bits - (value << UnitBits));

        public static Fixed operator *(Fixed self, Fixed other) => new Fixed((int)(((long)self.Bits * other.Bits) >> UnitBits));

        public static Fixed operator *(Fixed self, int value) => new Fixed((int)((long)self.Bits * value));

        public static Fixed operator *(int value, Fixed self) => new Fixed((int)((long)self.Bits * value));

        public static Fixed operator /(Fixed numerator, int value) => new Fixed(numerator.Bits / value);

        public static Fixed operator /(Fixed numerator, Fixed denominator)
        {
            // This is not an optimization anymore, but it prevents numbers
            // that are really far apart from overflowing or becoming zero.
            if ((Math.Abs(numerator.Bits) >> 14) >= Math.Abs(denominator.Bits))
                return new Fixed((numerator.Bits ^ denominator.Bits) < 0 ? 0x80000000 : 0x7FFFFFFF);
            return new Fixed((int)(((long)numerator.Bits << UnitBits) / denominator.Bits));
        }

        public static Fixed operator <<(Fixed self, int bits) => new Fixed(self.Bits << bits);

        public static Fixed operator >>(Fixed self, int bits) => new Fixed(self.Bits >> bits);

        public static Fixed operator &(Fixed numerator, int bits) => new Fixed(numerator.Bits & bits);

        public static Fixed operator |(Fixed numerator, int bits) => new Fixed(numerator.Bits | bits);

        public static bool operator ==(Fixed self, Fixed other) => self.Bits == other.Bits;

        public static bool operator !=(Fixed self, Fixed other) => !(self == other);

        public static bool operator >(Fixed self, Fixed value) => self.Bits > value.Bits;

        public static bool operator >=(Fixed self, Fixed value) => self.Bits >= value.Bits;

        public static bool operator <(Fixed self, Fixed value) => self.Bits < value.Bits;

        public static bool operator <=(Fixed self, Fixed value) => self.Bits <= value.Bits;

        public static bool operator >(Fixed self, int value) => self.Bits > (value << UnitBits);

        public static bool operator >=(Fixed self, int value) => self.Bits >= (value << UnitBits);

        public static bool operator <(Fixed self, int value) => self.Bits < (value << UnitBits);

        public static bool operator <=(Fixed self, int value) => self.Bits <= (value << UnitBits);

        /// <summary>
        /// Gets the integral value.
        /// </summary>
        /// <returns>The integral value.</returns>
        public int Int() => Integral;

        /// <summary>
        /// Gets the floating point value.
        /// </summary>
        /// <returns>The floating point value.</returns>
        public float Float() => Bits / 65536.0f;

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <returns>The double value.</returns>
        public double Double() => Bits / 65536.0;

        public override string ToString() => $"{Double()}";

        public override bool Equals(object obj) => obj is Fixed f && Bits == f.Bits;

        public override int GetHashCode() => Bits.GetHashCode();

        private static int BitsFromUpperAndLower(short upper, ushort lower)
        {
            uint bits = (uint)((ushort)upper << UnitBits);
            return (int)(bits | lower);
        }
    }
}
