using UnityEngine;

namespace Helion.Util.Unity
{
    /// <summary>
    /// Helper functions for a 3D box collider.
    /// </summary>
    public static class BoxColliderExtensions
    {
        /// <summary>
        /// Gets the point for the 8 corners of the collider. See the class
        /// comments for what the indices look like.
        /// </summary>
        /// <param name="boxCollider">The box collider.</param>
        /// <returns>The eight corners.</returns>
        public static Vector3[] GetCorners(this BoxCollider boxCollider)
        {
            Vector3 center = boxCollider.center;
            Vector3 extents = boxCollider.bounds.extents;
            Vector3 min = center - extents;
            Vector3 max = center + extents;

            // This has the following indices:
            //
            //    7--------6
            //   /.       /|
            //  / .      / |
            // 4--------5  |
            // |  .     |  |
            // |  3.....|..2      Y    Z
            // | .      | /       ^  .`
            // |.       |/        |.`
            // 0--------1         O--> X
            return new[]
            {
                new Vector3(min.x, min.y, min.z),
                new Vector3(max.x, min.y, min.z),
                new Vector3(max.x, min.y, max.z),
                new Vector3(min.x, min.y, max.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(max.x, max.y, min.z),
                new Vector3(max.x, max.y, max.z),
                new Vector3(min.x, max.y, max.z)
            };
        }

        /// <summary>
        /// Gets all the edges we can pass to a line renderer to draw the box
        /// collider.
        /// </summary>
        /// <param name="boxCollider">The box collider to draw.</param>
        /// <returns>The edge points to use in a line renderer.</returns>
        public static Vector3[] GetEdgeLinePoints(this BoxCollider boxCollider)
        {
            Vector3[] c = boxCollider.GetCorners();

            return new[]
            {
                // Draw bottom box
                c[0], c[1], c[2], c[3], c[0],
                // Draw top box
                c[4], c[5], c[6], c[7], c[4],
                // Draw the last 3 vertical edges we missed
                c[5], c[1], c[2], c[6], c[7], c[3]
            };
        }
    }
}
