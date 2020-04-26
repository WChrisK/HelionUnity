using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Helion.Util.Extensions
{
    /// <summary>
    /// A collection of extensions for IEnumerable.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Checks if the enumerable container is empty. This will only do the
        /// minimum iterations to determine if there is an element using the
        /// .Any() extension method.
        /// </summary>
        /// <param name="enumerable">The element to check.</param>
        /// <typeparam name="T">The type in the enumerable.</typeparam>
        /// <returns>True if it has no elements, false otherwise.</returns>
        public static bool Empty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

        /// <summary>
        /// Takes a nullable enumerable and returns an enumerable without any
        /// null values. It also allows you to use the non-nullable type, so
        /// an enumerable of T? can become an enumerable of T.
        /// </summary>
        /// <param name="enumerable">The element to iterate over.</param>
        /// <typeparam name="T">The backing enumerable type.</typeparam>
        /// <returns>A new enumerable without any null values.</returns>
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable) where T : class
        {
            // Since we're porting from nullable references to an older version
            // of C#, the last step drops the damn-it operator on the return
            // value of the Select call.
            return enumerable.Where(e => e != null).Select(e => e);
        }

        /// <summary>
        /// Checks if all the elements are distinct. Will try to early out for
        /// performance reasons.
        /// </summary>
        /// <param name="enumerable">The enumerable to check.</param>
        /// <typeparam name="T">The enumerable type.</typeparam>
        /// <returns>True if all the elements are unique, false otherwise. Will
        /// also return true if the enumerable is empty.</returns>
        public static bool AllDistinct<T>(this IEnumerable<T> enumerable)
        {
            HashSet<T> set = new HashSet<T>();

            foreach (T element in enumerable)
                if (!set.Add(element))
                    return false;

            return true;
        }

        /// <summary>
        /// Allows iterating over each element while also returning the
        /// underlying enumerable so they can be chained with other calls.
        /// </summary>
        /// <remarks>
        /// This makes it possible to insert actions into the pipeline of a
        /// sequence of linq calls. The order of iteration is the same as the
        /// ForEach() implementation it calls.
        /// </remarks>
        /// <param name="enumerable">The enumerable object.</param>
        /// <param name="func">The function to apply to each element.</param>
        /// <typeparam name="T">The enumerable type.</typeparam>
        /// <returns>The enumerable this was called with, allowing chaining.
        /// </returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> enumerable, Action<T> func)
        {
            enumerable.ForEach(func);
            return enumerable;
        }

        /// <summary>
        /// An alias for linq's "Select".
        /// </summary>
        /// <param name="enumerable">The enumerable object.</param>
        /// <param name="func">The mapping function.</param>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <typeparam name="R">The resulting output type.</typeparam>
        /// <returns>A new enumerable with the mapped type.</returns>
        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> enumerable, Func<T, R> func)
        {
            return enumerable.Select(func);
        }

        /// <summary>
        /// An alias for linq's "Where".
        /// </summary>
        /// <param name="enumerable">The enumerable object.</param>
        /// <param name="func">The filtering function.</param>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <returns>A new enumerable with only the filtered objects.</returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> enumerable, Func<T, bool> func)
        {
            return enumerable.Where(func);
        }

        /// <summary>
        /// An alias for linq's "Aggregate".
        /// </summary>
        /// <param name="enumerable">The enumerable object.</param>
        /// <param name="init">The initial value.</param>
        /// <param name="accumulatorFunc">The function that performs the
        /// accumulation.</param>
        /// <typeparam name="T">The enumerable type.</typeparam>
        /// <typeparam name="TAcc">The accumulator type.</typeparam>
        /// <returns>A reduced value from the enumerable.</returns>
        public static TAcc Reduce<T, TAcc>(this IEnumerable<T> enumerable, TAcc init,
            Func<TAcc, T, TAcc> accumulatorFunc)
        {
            return enumerable.Aggregate(init, accumulatorFunc);
        }
    }
}
