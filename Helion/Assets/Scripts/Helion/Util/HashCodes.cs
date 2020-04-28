namespace Helion.Util
{
    /// <summary>
    /// A helper class designed to yield the same results as C# does so we get
    /// the same hashing.
    /// </summary>
    public static class HashCodes
    {
        /// <summary>
        /// Hashes two 32-bit hashcodes together.
        /// </summary>
        /// <param name="first">The first hashcode.</param>
        /// <param name="second">The second hashcode.</param>
        /// <returns>A new combined hashcode.</returns>
        public static int Hash(int first, int second)
        {
            return (first << 5) + first ^ second;
        }

        /// <summary>
        /// Hashes three 32-bit hashcodes together.
        /// </summary>
        /// <param name="first">The first hashcode.</param>
        /// <param name="second">The second hashcode.</param>
        /// <param name="third">The third hashcode.</param>
        /// <returns>A new combined hashcode.</returns>
        public static int Hash(int first, int second, int third)
        {
            return Hash(Hash(first, second), third);
        }
    }
}
