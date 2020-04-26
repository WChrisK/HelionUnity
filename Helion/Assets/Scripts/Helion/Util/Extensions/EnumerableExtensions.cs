using System;
using System.Collections.Generic;
using System.Linq;

namespace Helion.Util.Extensions
{
    /// <summary>
    /// A group of enumerable range helpers.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Creates an enumerable range from [0, upperExclusive). Will do no
        /// iterations of the upperExclusive value is negative or zero.
        /// </summary>
        /// <param name="upperExclusive">The top element to go to but not visit
        /// starting from zero.</param>
        /// <returns>An enumerable of all the values.</returns>
        public static IEnumerable<int> Range(int upperExclusive)
        {
            return Enumerable.Range(0, Math.Max(0, upperExclusive));
        }
    }
}
