using System.Collections.Generic;

namespace Helion.Util.Extensions
{
    /// <summary>
    /// Helper methods for dictionaries.
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Looks up a key/value pair and returns an optional.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <typeparam name="K">The key type.</typeparam>
        /// <typeparam name="T">The value type.</typeparam>
        /// <returns>An optional of whether the value was found or not.
        /// </returns>
        public static Optional<T> Find<K, T>(this IDictionary<K, T> dict, K key) where T : class
        {
            return dict.TryGetValue(key, out T value) ? value : Optional<T>.Empty();
        }
    }
}
