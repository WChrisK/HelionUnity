using System;
using Object = UnityEngine.Object;

namespace Helion.Core.Util.Unity
{
    /// <summary>
    /// Helper functions for game objects.
    /// </summary>
    public class GameObjectHelper
    {
        internal static Action<Object> destroyFunc = obj => throw new Exception(errorMessage);
        private static string errorMessage = "CRITICAL ERROR: Did not set UnityHelper.Destroy!";

        /// <summary>
        /// Destroys a Unity object.
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        public static void Destroy(Object obj)
        {
            destroyFunc(obj);
        }
    }
}
