using System;
using Helion.Core.Util.Logging;
using UnityEngine.Windows;

namespace Helion.Cache
{
    public static class Caches
    {
        public const string Folder = "caches";
        private static readonly Log Log = LogManager.Instance();

        static Caches()
        {
            try
            {
                if (!Directory.Exists(Folder))
                    Directory.CreateDirectory(Folder);
            }
            catch (Exception e)
            {
                Log.Error($"Unable to create cache directory: {e.Message}");
            }
        }
    }
}
