namespace Helion.Util.Extensions
{
    /// <summary>
    /// A series of helper methods for an array.
    /// </summary>
    public static class Arrays
    {
        /// <summary>
        /// Creates an array with a certain size and filled value. Invalid
        /// values (like a negative length) will throw an exception.
        /// </summary>
        /// <param name="length">The array size, should be non-negative.
        /// </param>
        /// <param name="value">The value to fill the array with.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>A new array with the length provided, filled with the
        /// value provided in each slot.</returns>
        public static T[] Create<T>(int length, T value)
        {
            T[] array = new T[length];
            for (int i = 0; i < length; i++)
                array[i] = value;
            return array;
        }

        /// <summary>
        /// Fills an array with the value provided.
        /// </summary>
        /// <param name="array">The array to fill</param>
        /// <param name="value">The value to fill.</param>
        /// <typeparam name="T">The array element type.</typeparam>
        public static void Fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }
    }
}
