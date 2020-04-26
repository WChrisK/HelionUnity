using System;
using System.IO;
using Helion.Util.Extensions;

namespace Helion.Configs.Components
{
    [Serializable]
    public class ConfigResources
    {
        private static readonly string DirectorySeparator = Path.DirectorySeparatorChar.ToString();

        public string Directory = "";

        /// <summary>
        /// Gets the base directory. If not the current directory (as in, an
        /// empty string for <see cref="Directory"/>) this makes sure a slash
        /// is trailing. Calling this may mutate <see cref="Directory"/>!
        /// </summary>
        /// <returns>The base directory string. Will have a path separator at
        /// the end always unless the directory is empty.</returns>
        public string GetBaseDirectory()
        {
            // We want to make sure it has the path separator set. This should
            // not change any observable result if we add it when it's not
            // present.
            if (Directory.NotEmpty() && !Directory.EndsWith(DirectorySeparator))
                Directory += DirectorySeparator;

            return Directory;
        }
    }
}
