using System;
using System.Security.Cryptography;
using System.Text;
using MoreLinq;

namespace Helion.Core.Util.Extensions
{
    /// <summary>
    /// A collection of extensions for primitive numbers.
    /// </summary>
    public static class PrimitiveExtensions
    {
        /// <summary>
        /// Calculates the MD5 hash of the data.
        /// </summary>
        /// <param name="data">The data to calculate from.</param>
        /// <returns>The MD5 hash in upper case.</returns>
        public static UpperString CalculateMD5(this byte[] data)
        {
            // Source: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.md5?view=netframework-4.8
            using (MD5 md5Hash = MD5.Create())
            {
                StringBuilder builder = new StringBuilder();
                byte[] hash = md5Hash.ComputeHash(data);
                hash.ForEach(b => builder.Append(b.ToString("x2")));
                return builder.ToString();
            }
        }

        /// <summary>
        /// Checks if the value is equal to zero within some epsilon.
        /// </summary>
        /// <param name="f">The value to check.</param>
        /// <param name="epsilon">The epsilon value.</param>
        /// <returns>True if it's within some epsilon, false otherwise.
        /// </returns>
        public static bool ApproxZero(this float f, float epsilon = 0.0001f) => Math.Abs(f) < epsilon;

        /// <summary>
        /// Checks if the value is equal to zero within some epsilon.
        /// </summary>
        /// <param name="d">The value to check.</param>
        /// <param name="epsilon">The epsilon value.</param>
        /// <returns>True if it's within some epsilon, false otherwise.
        /// </returns>
        public static bool ApproxZero(this double d, double epsilon = 0.00001) => Math.Abs(d) < epsilon;

        /// <summary>
        /// Checks if the values are approximately equal to one another.
        /// </summary>
        /// <param name="f">The first value to compare.</param>
        /// <param name="other">The second value to compare.</param>
        /// <param name="epsilon">The acceptable delta between both.</param>
        /// <returns>True if they're approximately equal, false if not.
        /// </returns>
        public static bool Approx(this float f, float other, float epsilon) => Math.Abs(f - other) < epsilon;

        /// <summary>
        /// Checks if the values are approximately equal to one another.
        /// </summary>
        /// <param name="d">The first value to compare.</param>
        /// <param name="other">The second value to compare.</param>
        /// <param name="epsilon">The acceptable delta between both.</param>
        /// <returns>True if they're approximately equal, false if not.
        /// </returns>
        public static bool Approx(this double d, double other, double epsilon) => Math.Abs(d - other) < epsilon;

        /// <summary>
        /// Checks if the values are bitwise-equal to one another.
        /// </summary>
        /// <param name="f">The first value to compare.</param>
        /// <param name="other">The second value to compare.</param>
        /// <returns>True if they're equal, false if not.</returns>
        public static bool AreEqual(this float f, float other) => f == other;

        /// <summary>
        /// Checks if the values are bitwise-equal to one another.
        /// </summary>
        /// <param name="d">The first value to compare.</param>
        /// <param name="other">The second value to compare.</param>
        /// <returns>True if they're equal, false if not.</returns>
        public static bool AreEqual(this double d, double other) => d == other;

        /// <summary>
        /// Checks if the signs are different. If one of the arguments are zero
        /// then the result is always false.
        /// </summary>
        /// <param name="f">The first value to check.</param>
        /// <param name="other">The second value to check.</param>
        /// <returns>True if the signs are different, false if not.</returns>
        public static bool DifferentSign(this float f, float other) => (f * other) < 0;

        /// <summary>
        /// Checks if the signs are different. If one of the arguments are zero
        /// then the result is always false.
        /// </summary>
        /// <param name="d">The first value to check.</param>
        /// <param name="other">The second value to check.</param>
        /// <returns>True if the signs are different, false if not.</returns>
        public static bool DifferentSign(this double d, double other) => (d * other) < 0;

        /// <summary>
        /// Checks if the value is in the [0.0, 1.0] range.
        /// </summary>
        /// <param name="f">The value to check.</param>
        /// <returns>True if it is, false if not.</returns>
        public static bool InNormalRange(this float f) => f >= 0 && f <= 1;

        /// <summary>
        /// Checks if the value is in the [0.0, 1.0] range.
        /// </summary>
        /// <param name="d">The value to check.</param>
        /// <returns>True if it is, false if not.</returns>
        public static bool InNormalRange(this double d) => d >= 0 && d <= 1;

        /// <summary>
        /// Interpolates the value from the current to the provided value by
        /// some time t.
        /// </summary>
        /// <param name="start">The initial point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="t">The fraction along the way, where 0.0 would yield
        /// start and 1.0 would yield end.</param>
        /// <returns>The value based on t.</returns>
        public static float Interpolate(this float start, float end, float t) => start + (t * (end - start));

        /// <summary>
        /// Interpolates the value from the current to the provided value by
        /// some time t.
        /// </summary>
        /// <param name="start">The initial point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="t">The fraction along the way, where 0.0 would yield
        /// start and 1.0 would yield end.</param>
        /// <returns>The value based on t.</returns>
        public static double Interpolate(this double start, double end, double t) => start + (t * (end - start));

        /// <summary>
        /// Clamps the value between two ranges inclusively and returns the
        /// result.
        /// </summary>
        /// <param name="b">The value to clamp.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>The clamped value.</returns>
        public static byte Clamp(this byte b, byte low, byte high) => b < low ? low : (b > high ? high : b);

        /// <summary>
        /// Clamps the value between two ranges inclusively and returns the
        /// result.
        /// </summary>
        /// <param name="s">The value to clamp.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>The clamped value.</returns>
        public static short Clamp(this short s, short low, short high) => s < low ? low : (s > high ? high : s);

        /// <summary>
        /// Clamps the value between two ranges inclusively and returns the
        /// result.
        /// </summary>
        /// <param name="s">The value to clamp.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>The clamped value.</returns>
        public static ushort Clamp(this ushort s, ushort low, ushort high) => s < low ? low : (s > high ? high : s);

        /// <summary>
        /// Clamps the value between two ranges inclusively and returns the
        /// result.
        /// </summary>
        /// <param name="i">The value to clamp.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>The clamped value.</returns>
        public static int Clamp(this int i, int low, int high) => i < low ? low : (i > high ? high : i);

        /// <summary>
        /// Clamps the value between two ranges inclusively and returns the
        /// result.
        /// </summary>
        /// <param name="i">The value to clamp.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>The clamped value.</returns>
        public static uint Clamp(this uint i, uint low, uint high) => i < low ? low : (i > high ? high : i);

        /// <summary>
        /// Clamps the value between two ranges inclusively and returns the
        /// result.
        /// </summary>
        /// <param name="l">The value to clamp.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>The clamped value.</returns>
        public static long Clamp(this long l, long low, long high) => l < low ? low : (l > high ? high : l);

        /// <summary>
        /// Clamps the value between two ranges inclusively and returns the
        /// result.
        /// </summary>
        /// <param name="l">The value to clamp.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>The clamped value.</returns>
        public static ulong Clamp(this ulong l, ulong low, ulong high) => l < low ? low : (l > high ? high : l);

        /// <summary>
        /// Clamps the value between two ranges inclusively and returns the
        /// result.
        /// </summary>
        /// <param name="f">The value to clamp.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(this float f, float low, float high) => f < low ? low : (f > high ? high : f);

        /// <summary>
        /// Clamps the value between two ranges inclusively and returns the
        /// result.
        /// </summary>
        /// <param name="d">The value to clamp.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>The clamped value.</returns>
        public static double Clamp(this double d, double low, double high) => d < low ? low : (d > high ? high : d);

        /// <summary>
        /// Checks if the bits in the bitmask is set on the value.
        /// </summary>
        /// <remarks>
        /// This will pass vacuously if the bitmask is zero.
        /// </remarks>
        /// <param name="value">The value to check.</param>
        /// <param name="bitmask">The bits to check.</param>
        /// <returns>True if all the bits in bitmask are set on value, false
        /// otherwise.</returns>
        public static bool HasBits(this int value, int bitmask) => (value & bitmask) == bitmask;

        /// <summary>
        /// Checks if the bits in the bitmask is set on the value.
        /// </summary>
        /// <remarks>
        /// This will pass vacuously if the bitmask is zero.
        /// </remarks>
        /// <param name="value">The value to check.</param>
        /// <param name="bitmask">The bits to check.</param>
        /// <returns>True if all the bits in bitmask are set on value, false
        /// otherwise.</returns>
        public static bool HasBits(this uint value, uint bitmask) => (value & bitmask) == bitmask;

        /// <summary>
        /// Gets the value without the bits from bitmask.
        /// </summary>
        /// <param name="value">The value to use.</param>
        /// <param name="bitmask">The bits to remove.</param>
        /// <returns>A new value without the bits in bitmask.</returns>
        public static int WithoutBits(this int value, int bitmask) => value & ~bitmask;

        /// <summary>
        /// Gets the value without the bits from bitmask.
        /// </summary>
        /// <param name="value">The value to use.</param>
        /// <param name="bitmask">The bits to remove.</param>
        /// <returns>A new value without the bits in bitmask.</returns>
        public static uint WithoutBits(this uint value, uint bitmask) => value & ~bitmask;

        /// <summary>
        /// Gets the value with the bits set from bitmask.
        /// </summary>
        /// <param name="value">The value to use.</param>
        /// <param name="bitmask">The bits to set.</param>
        /// <returns>The value with the bits in bitmask set.</returns>
        public static int WithBits(this int value, int bitmask) => value | bitmask;

        /// <summary>
        /// Gets the value with the bits set from bitmask.
        /// </summary>
        /// <param name="value">The value to use.</param>
        /// <param name="bitmask">The bits to set.</param>
        /// <returns>The value with the bits in bitmask set.</returns>
        public static uint WithBits(this uint value, uint bitmask) => value | bitmask;

        /// <summary>
        /// Performs a min/max on the two numbers.
        /// </summary>
        /// <param name="value">The current number.</param>
        /// <param name="other">The other number.</param>
        /// <returns>An ordered tuple of the min, and max number.</returns>
        public static (int min, int max) MinMax(this int value, int other)
        {
            return value < other ? (value, other) : (other, value);
        }

        /// <summary>
        /// Performs a min/max on the two numbers.
        /// </summary>
        /// <param name="value">The current number.</param>
        /// <param name="other">The other number.</param>
        /// <returns>An ordered tuple of the min, and max number.</returns>
        public static (uint min, uint max) MinMax(this uint value, uint other)
        {
            return value < other ? (value, other) : (other, value);
        }

        /// <summary>
        /// Performs a min/max on the two numbers.
        /// </summary>
        /// <param name="value">The current number.</param>
        /// <param name="other">The other number.</param>
        /// <returns>An ordered tuple of the min, and max number.</returns>
        public static (float min, float max) MinMax(this float value, float other)
        {
            return value < other ? (value, other) : (other, value);
        }

        /// <summary>
        /// Performs a min/max on the two numbers.
        /// </summary>
        /// <param name="value">The current number.</param>
        /// <param name="other">The other number.</param>
        /// <returns>An ordered tuple of the min, and max number.</returns>
        public static (double min, double max) MinMax(this double value, double other)
        {
            return value < other ? (value, other) : (other, value);
        }

        /// <summary>
        /// Converts the number into map units by scaling it to the Unity
        /// world dimensions.
        /// </summary>
        /// <param name="number">The number to scale.</param>
        /// <returns>The scaled number.</returns>
        public static float MapUnit(this int number) => number * Constants.MapUnit;

        /// <summary>
        /// Converts the number into map units by scaling it to the Unity
        /// world dimensions.
        /// </summary>
        /// <param name="number">The number to scale.</param>
        /// <returns>The scaled number.</returns>
        public static float MapUnit(this float number) => number * Constants.MapUnit;

        /// <summary>
        /// Checks if the integer is in the range provided. Equal to checking
        /// if value is in [low, high].
        /// </summary>
        /// <param name="i">The number.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (inclusive).</param>
        /// <returns>True if in range, false if outside.</returns>
        public static bool InRangeInclusive(this int i, int low, int high) => i >= low && i <= high;

        /// <summary>
        /// Checks if the integer is in the range provided. Equal to checking
        /// if value is in [low, high).
        /// </summary>
        /// <param name="i">The number.</param>
        /// <param name="low">The lower bound (inclusive).</param>
        /// <param name="high">The upper bound (exclusive).</param>
        /// <returns>True if in range, false if outside.</returns>
        public static bool InRangeExclusive(this int i, int low, int high) => i >= low && i < high;
    }
}
