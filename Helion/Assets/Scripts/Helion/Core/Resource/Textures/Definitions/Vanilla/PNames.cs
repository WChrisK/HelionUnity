using System.Collections.Generic;
using Helion.Core.Util;
using Helion.Core.Util.Bytes;

namespace Helion.Core.Resource.Textures.Definitions.Vanilla
{
    /// <summary>
    /// A collection of patch names from a pnames entry.
    /// </summary>
    public class PNames
    {
        /// <summary>
        /// The list of patch names.
        /// </summary>
        public readonly IReadOnlyList<UpperString> Names;

        /// <summary>
        /// The number of patch names available.
        /// </summary>
        public int Count => Names.Count;

        /// <summary>
        /// Creates a new pnames object from an existing set of names.
        /// </summary>
        /// <param name="names">The texture names for each index.</param>
        private PNames(IReadOnlyList<UpperString> names)
        {
            Names = names;
        }

        /// <summary>
        /// Reads a pnames data from the entry provided.
        /// </summary>
        /// <param name="data">The data to read from.</param>
        /// <returns>A read pnames object from the entry, or an empty value if
        /// the entry has the wrong data amount or is corrupt.</returns>
        public static Optional<PNames> From(byte[] data)
        {
            try
            {
                ByteReader reader = ByteReader.From(ByteOrder.Little, data);
                int numStrings = reader.Int();

                List<UpperString> pnames = new List<UpperString>(numStrings);
                for (int i = 0; i < numStrings; i++)
                    pnames.Add(reader.StringWithoutNulls(8));

                return new PNames(pnames);
            }
            catch
            {
                return Optional<PNames>.Empty();
            }
        }

        /// <summary>
        /// Gets the patch name at the index provided.
        /// </summary>
        /// <param name="index">The index of the patch name.</param>
        public UpperString this[int index] => Names[index];
    }
}
