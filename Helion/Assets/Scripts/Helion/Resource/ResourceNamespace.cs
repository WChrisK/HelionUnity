using System.Linq;
using Helion.Archives;
using Helion.Util.Extensions;

namespace Helion.Resource
{
    /// <summary>
    /// All the available namespaces in an archive.
    /// </summary>
    public enum ResourceNamespace
    {
        Global,
        ACS,
        Flats,
        Fonts,
        Graphics,
        Music,
        Sounds,
        Sprites,
        Textures
    }

    /// <summary>
    /// A helper class for resource namespaces.
    /// </summary>
    public static class ResourceNamespaceHelper
    {
        /// <summary>
        /// Takes an entry path and gets the namespace from it.
        /// </summary>
        /// <param name="path">The entry path.</param>
        /// <returns>The namespace for the path.</returns>
        public static ResourceNamespace From(EntryPath path)
        {
            if (path.Folders.Empty())
                return ResourceNamespace.Global;

            switch (path.Folders.First().ToUpper())
            {
                case "ACS":
                    return ResourceNamespace.ACS;
                case "FLATS":
                    return ResourceNamespace.Flats;
                case "FONTS":
                    return ResourceNamespace.Fonts;
                case "GRAPHICS":
                    return ResourceNamespace.Graphics;
                case "MUSIC":
                    return ResourceNamespace.Music;
                case "SOUNDS":
                    return ResourceNamespace.Sounds;
                case "SPRITES":
                    return ResourceNamespace.Sprites;
                case "TEXTURES":
                    return ResourceNamespace.Textures;
                default:
                    return ResourceNamespace.Global;
            }
        }
    }
}
