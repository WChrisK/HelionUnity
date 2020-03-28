using System.Collections;
using System.Collections.Generic;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Util
{
    /// <summary>
    /// A helper class that makes sure all its characters are upper case.
    /// </summary>
    /// <remarks>
    /// When we want to compare strings without worrying about the underlying
    /// case, or to automatically convert it to an upper case string, this
    /// helper class will do so without having to be verbose in converting to
    /// upper case.
    /// Note that this will allocate a whole new string, so it should not be
    /// used in any high-performing code if you do not want GC pressure.
    /// </remarks>
    public sealed class UpperString : IEnumerable<char>
    {
        private readonly string str;

        /// <summary>
        /// The length of the string.
        /// </summary>
        public int Length => str.Length;

        /// <summary>
        /// If the string is an empty (zero length) string.
        /// </summary>
        public bool Empty => str.Empty();

        /// <summary>
        /// If the string is has any characters.
        /// </summary>
        public bool NotEmpty => str.NotEmpty();

        private UpperString(string s)
        {
            str = s.ToUpper();
        }

        /// <summary>
        /// Creates an upper string from an existing string implicitly.
        /// </summary>
        /// <param name="s">The string to create an upper string from.</param>
        /// <returns>The upper version of the string.</returns>
        public static implicit operator UpperString(string s) => new UpperString(s);

        /// <summary>
        /// Gets the character at the index provided. Throws if out of range.
        /// </summary>
        /// <param name="index">The index of the character.</param>
        public char this[int index] => str[index];

        /// <summary>
        /// Checks if two upper strings are equal. Handles null as well.
        /// </summary>
        /// <param name="current">The current string.</param>
        /// <param name="other">The other string.</param>
        /// <returns>True if so, false if not.</returns>
        public static bool operator ==(UpperString current, UpperString other)
        {
            if (ReferenceEquals(current, null) && ReferenceEquals(other, null))
                return true;
            if (ReferenceEquals(current, null) || ReferenceEquals(other, null))
                return false;
            return current.str.Equals(other.str);
        }

        /// <summary>
        /// Checks if two strings do not equal.
        /// </summary>
        /// <param name="current">The current string.</param>
        /// <param name="other">The other string.</param>
        /// <returns>True if not equal, false if so.</returns>
        public static bool operator !=(UpperString current, UpperString other)
        {
            return !(current == other);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
            case UpperString otherUpperString:
                return str.Equals(otherUpperString.str);
            case string otherString:
                return str.Equals(otherString);
            default:
                return false;
            }
        }

        public override int GetHashCode() => str.GetHashCode();

        public override string ToString() => str;

        public IEnumerator<char> GetEnumerator() => str.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
