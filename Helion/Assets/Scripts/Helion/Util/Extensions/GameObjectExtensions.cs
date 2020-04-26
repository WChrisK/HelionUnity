using UnityEngine;

namespace Helion.Util.Extensions
{
    /// <summary>
    /// Helper functions for the game object.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Sets the child in a simpler and easier to read fashion.
        /// </summary>
        /// <param name="gameObject">The current object.</param>
        /// <param name="child">The child to set.</param>
        /// <param name="worldPositionStays">True if the world position should
        /// be unchanged, false if not.</param>
        public static void SetChild(this GameObject gameObject, GameObject child, bool worldPositionStays = true)
        {
            child.transform.SetParent(gameObject.transform, worldPositionStays);
        }
    }
}
