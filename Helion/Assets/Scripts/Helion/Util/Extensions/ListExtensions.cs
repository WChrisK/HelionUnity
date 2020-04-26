using System.Collections.Generic;
using System.Linq;

namespace Helion.Util.Extensions
{
    /// <summary>
    /// Helper functions for the <see cref="List{T}"/> class.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Gets a reverse iterator for a list without reversing the list.
        /// </summary>
        /// <param name="list">The list to iterate backwards over.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>An iterator for the list in a backwards direction.
        /// </returns>
        public static IEnumerable<T> Reversed<T>(this List<T> list)
        {
            return ((IList<T>)list).Reverse();
        }
    }
}
