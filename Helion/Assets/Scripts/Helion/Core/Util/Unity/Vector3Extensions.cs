using UnityEngine;

namespace Helion.Core.Util.Unity
{
    /// <summary>
    /// Helper methods for Unity's Vector3 class.
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Scales the vector to Unity's map unit scaling. This should be
        /// called on any world position vector that is to be placed in a
        /// Unity scene.
        /// </summary>
        /// <param name="vector">The vector to scale.</param>
        /// <returns>The map unit scaled vector.</returns>
        public static Vector3 MapUnit(this Vector3 vector) => vector * Constants.MapUnit;
    }
}
