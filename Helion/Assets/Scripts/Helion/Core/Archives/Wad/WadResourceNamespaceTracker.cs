using System.Collections.Generic;
using Helion.Core.Resource;
using Helion.Core.Util;

namespace Helion.Core.Archives.Wad
{
    /// <summary>
    /// Tracks namespace changes to always yield the proper namespace when
    /// processing a wad.
    /// </summary>
    public class WadResourceNamespaceTracker
    {
        private static readonly Dictionary<UpperString, ResourceNamespace> EntryToNamespace = new Dictionary<UpperString, ResourceNamespace>()
        {
            ["F_START"] = ResourceNamespace.Flats,
            ["F_END"] = ResourceNamespace.Global,
            ["FF_START"] = ResourceNamespace.Flats,
            ["FF_END"] = ResourceNamespace.Global,
            ["HI_START"] = ResourceNamespace.Textures,
            ["HI_END"] = ResourceNamespace.Textures,
            ["P_START"] = ResourceNamespace.Textures,
            ["P_END"] = ResourceNamespace.Global,
            ["PP_START"] = ResourceNamespace.Textures,
            ["PP_END"] = ResourceNamespace.Global,
            ["S_START"] = ResourceNamespace.Sprites,
            ["S_END"] = ResourceNamespace.Global,
            ["SS_START"] = ResourceNamespace.Sprites,
            ["SS_END"] = ResourceNamespace.Global,
            ["T_START"] = ResourceNamespace.Textures,
            ["T_END"] = ResourceNamespace.Global,
            ["TX_START"] = ResourceNamespace.Textures,
            ["TX_END"] = ResourceNamespace.Global,
        };

        private ResourceNamespace Current = ResourceNamespace.Global;

        /// <summary>
        /// Updates the tracker and returns the namespace enum that should be
        /// used for the entry.
        /// </summary>
        /// <param name="entry">The entry to evaluate.</param>
        /// <returns>The namespace to use for it.</returns>
        public ResourceNamespace Update(WadDirectoryEntry entry)
        {
            if (EntryToNamespace.TryGetValue(entry.Name, out ResourceNamespace resourceNamespace))
            {
                Current = resourceNamespace;
                return ResourceNamespace.Global;
            }

            return Current;
        }
    }
}
