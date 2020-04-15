using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Archives;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Resource.Textures.Definitions.Vanilla
{
    /// <summary>
    /// A helper class for handling the disaster that is pnames/textureX. Can
    /// be iterated over to get the pairs of pnames/textureX.
    /// </summary>
    public class VanillaTextureTracker : IEnumerable<(PNames, TextureX)>
    {
        /// <summary>
        /// A list of all the entries for each archive. A corollary is that
        /// each entry in the list is from the paired archive.
        /// </summary>
        private readonly List<(IArchive archive, List<IVanillaTextureDefinition> definitions)> archiveEntries = new List<(IArchive, List<IVanillaTextureDefinition>)>();

        /// <summary>
        /// Tracks an entry which came from the archive provided.
        /// </summary>
        /// <remarks>
        /// These should be submitted in the order they are seen from the
        /// archives. For example if wad A, B, and C are processed, then
        /// we should be calling all the entries in A before B, and likewise
        /// all the entries before C when doing B. If not, an invariant will
        /// be lost and the results will be wrong.
        /// </remarks>
        /// <param name="definition">The data to track.</param>
        /// <param name="archive">The archive the entry is part of.</param>
        public void Track(IVanillaTextureDefinition definition, IArchive archive)
        {
            if (archiveEntries.Empty())
            {
                archiveEntries.Add((archive, new List<IVanillaTextureDefinition> { definition }));
                return;
            }

            // Because we assume we are adding linearly, if we're still
            // processing the same archive then we want to bundle all of the
            // definition entries together so we can properly match up any
            // cross-archive disjoint definitions. We do this by comparing a
            // reference to the last archive seen.
            (IArchive lastArchive, List<IVanillaTextureDefinition> entries) = archiveEntries.Last();
            if (ReferenceEquals(lastArchive, archive))
                entries.Add(definition);
            else
                archiveEntries.Add((archive, new List<IVanillaTextureDefinition> { definition }));
        }

        public IEnumerator<(PNames, TextureX)> GetEnumerator()
        {
            for (int i = 0; i < archiveEntries.Count; i++)
            {
                Optional<PNames> pnames = GetLatestPNames(i);
                if (pnames)
                    foreach (TextureX textureX in GetLatestTextureXFor(i))
                        yield return (pnames.Value, textureX);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Optional<PNames> GetLatestPNames(int index)
        {
            // Start at the current index we're at and search backwards.
            for (int i = index; i >= 0; i--)
                foreach (IVanillaTextureDefinition definition in archiveEntries[i].definitions)
                    if (definition is PNames pnames)
                        return pnames;
            return Optional<PNames>.Empty();
        }

        private IEnumerable<TextureX> GetLatestTextureXFor(int index)
        {
            TextureX texture1 = null;
            TextureX texture2 = null;

            // Start at the current index we're at and search backwards. We
            // want to find only the latest texture1/2, and not find any other
            // thing (ex: TEXTURE3...) since apparently those are not supported
            // by source ports.
            for (int i = index; i >= 0; i--)
            {
                foreach (IVanillaTextureDefinition definition in archiveEntries[i].definitions)
                {
                    if (definition is TextureX textureX)
                    {
                        switch (textureX.TextureXNumber)
                        {
                        case 1:
                            texture1 = textureX;
                            break;
                        case 2:
                            texture2 = textureX;
                            break;
                        }
                    }
                }
            }

            List<TextureX> definitionList = new List<TextureX>();
            if (texture1 != null)
                definitionList.Add(texture1);
            if (texture2 != null)
                definitionList.Add(texture2);

            return definitionList;
        }
    }
}
