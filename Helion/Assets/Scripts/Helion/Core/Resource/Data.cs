using System.Collections.Generic;
using Helion.Core.Archives;

namespace Helion.Core.Resource
{
    /// <summary>
    /// A collection of all of the data
    /// </summary>
    public static class Data
    {
        private static List<IArchive> archives = new List<IArchive>();

        public static bool Load(params string[] files)
        {
            // TODO
            return false;
        }
    }
}
