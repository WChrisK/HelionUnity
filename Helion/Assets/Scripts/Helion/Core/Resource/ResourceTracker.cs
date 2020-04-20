using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Util;

namespace Helion.Core.Resource
{
    /// <summary>
    /// A tracker of loaded resources by name/namespace.
    /// </summary>
    /// <typeparam name="T">The type to track.</typeparam>
    public class ResourceTracker<T> : IEnumerable<T> where T : class
    {
        private readonly Dictionary<UpperString, Dictionary<ResourceNamespace, T>> table = new Dictionary<UpperString, Dictionary<ResourceNamespace, T>>();

        /// <summary>
        /// Checks if the key exists in the map.
        /// </summary>
        /// <param name="name">The name to look up.</param>
        /// <param name="resourceNamespace">The resource namespace.</param>
        /// <returns>True if a value exists, false if not.</returns>
        public bool Contains(UpperString name, ResourceNamespace resourceNamespace)
        {
            return table.TryGetValue(name, out Dictionary<ResourceNamespace, T> namespaceToEntry) &&
                   namespaceToEntry.ContainsKey(resourceNamespace);
        }

        /// <summary>
        /// Adds a new item to be tracked. Will overwrite old values, but will
        /// return the value that previously was there if overwriting occurs.
        /// Note: The return value may be null for optimization reasons.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="resourceNamespace">The namespace.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The value if it exists, or null if not. This does not
        /// return an optional to prevent heavy object spamming when invoking
        /// this function many times, so you have to check for null.</returns>
        public T Add(UpperString name, ResourceNamespace resourceNamespace, T value)
        {
            T existingValue = null;

            if (table.TryGetValue(name, out Dictionary<ResourceNamespace, T> namespaceToEntry))
            {
                namespaceToEntry.TryGetValue(resourceNamespace, out existingValue);
                namespaceToEntry[resourceNamespace] = value;
            }
            else
                table[name] = new Dictionary<ResourceNamespace, T> {[resourceNamespace] = value};

            return existingValue;
        }

        /// <summary>
        /// Tries to get the value if it exists. This is an exact match for the
        /// namespace provided. For non-exact namespace matching, call the
        /// function <see cref="TryGetAnyValue"/>.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="resourceNamespace">The namespace.</param>
        /// <param name="value">The value that will be set with the instance,
        /// or null if it does not exist.</param>
        /// <returns>True if found, false if not.</returns>
        public bool TryGetValue(UpperString name, ResourceNamespace resourceNamespace, out T value)
        {
            if (table.TryGetValue(name, out Dictionary<ResourceNamespace, T> namespaceToEntry))
            {
                return namespaceToEntry.TryGetValue(resourceNamespace, out value);
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Tries to get the value if it exists from any namespace, but will
        /// search the priority one first.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="priorityNamespace">The namespace to search first for.
        /// </param>
        /// <param name="value">The value that will be set with the instance,
        /// or null if it does not exist.</param>
        /// <returns>True if found, false if not.</returns>
        public bool TryGetAnyValue(UpperString name, ResourceNamespace priorityNamespace, out T value)
        {
            if (table.TryGetValue(name, out Dictionary<ResourceNamespace, T> namespaceToEntry))
            {
                if (namespaceToEntry.TryGetValue(priorityNamespace, out value))
                    return true;

                foreach (var pair in namespaceToEntry)
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        public IEnumerator<T> GetEnumerator() => table.Values.SelectMany(dict => dict.Values).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
