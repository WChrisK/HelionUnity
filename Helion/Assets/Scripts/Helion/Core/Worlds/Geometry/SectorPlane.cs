using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    /// <summary>
    /// A plane for a sector. This is referenced by <see cref="Subsector"/>
    /// instances.
    /// </summary>
    public class SectorPlane
    {
        /// <summary>
        /// The index of this plane in the list of all planes.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// The texture name for this plane.
        /// </summary>
        public UpperString TextureName;

        /// <summary>
        /// The height that the sector is at. This is independent of the plane
        /// and is used to tell how to slope the plane if present. Otherwise,
        /// it is the height of the plane.
        /// </summary>
        public float Height;

        /// <summary>
        /// The light level in the 0 - 256 range. It may go outside this range
        /// for compatibility reasons.
        /// </summary>
        public int LightLevel;

        /// <summary>
        /// The plane for sloping. It is null if no slope is applied.
        /// </summary>
        public Plane? Plane;

        /// <summary>
        /// The light level in a normalized range of 0.0f to 1.0f.
        /// </summary>
        public float LightLevelNormalized => LightLevel * Constants.InverseLightLevel;

        /// <summary>
        /// Creates a new sector plane.
        /// </summary>
        /// <param name="index">The index of the plane.</param>
        /// <param name="textureName">The texture name.</param>
        /// <param name="height">The vertical height in the world.</param>
        /// <param name="lightLevel">The light level in integer form.</param>
        public SectorPlane(int index, UpperString textureName, float height, int lightLevel)
        {
            Index = index;
            TextureName = textureName;
            Height = height;
            LightLevel = lightLevel;
        }
    }
}
