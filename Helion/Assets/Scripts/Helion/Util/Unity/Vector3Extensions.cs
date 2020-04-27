using UnityEngine;

namespace Helion.Util.Unity
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
        /// <param name="vec">The vector to scale.</param>
        /// <returns>The map unit scaled vector.</returns>
        public static Vector3 MapUnit(this Vector3 vec) => vec * Constants.MapUnit;

        /// <summary>
        /// Gets the bird's eye projection of this vector on the X/Z plane.
        /// This is equal to dropping the Y coordinate.
        /// </summary>
        /// <param name="vec">The vector.</param>
        /// <returns>The 2D vector with the X/Z components placed in the X/Y
        /// of the returned vector.</returns>
        public static Vector2 XZ(this Vector3 vec) => new Vector2(vec.x, vec.z);
    }
}
