using System;
using System.Collections.Generic;
using System.Linq;

namespace Helion.Util.Extensions
{
    /// <summary>
    /// A collection of IList extension helpers.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Performs an action on a list in reverse. This is designed to have
        /// very little overhead and not mutate the list or create a whole new
        /// list to reverse-iterate over. It does it in place and efficiently.
        /// </summary>
        /// <param name="list">The list to operate on.</param>
        /// <param name="action">The action to perform at each element.</param>
        /// <typeparam name="T">The list generic type.</typeparam>
        public static void ForEachReverse<T>(this IList<T> list, Action<T> action)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                action(list[i]);
        }
    }
}
