using Helion.Core.Util;

namespace Helion.Core.Resource.Maps.Doom
{
    /// <summary>
    /// A sector in a Doom map.
    /// </summary>
    public class DoomSector
    {
        /// <summary>
        /// The height of the floor.
        /// </summary>
        public short FloorHeight { get; }

        /// <summary>
        /// The height of the ceiling.
        /// </summary>
        public short CeilingHeight { get; }

        /// <summary>
        /// The floor texture.
        /// </summary>
        public UpperString FloorTexture { get; }

        /// <summary>
        /// The ceiling texture.
        /// </summary>
        public UpperString CeilingTexture { get; }

        /// <summary>
        /// The light level.
        /// </summary>
        public short LightLevel { get; }

        /// <summary>
        /// The sector tag.
        /// </summary>
        public ushort Tag { get; }

        /// <summary>
        /// Creates a new Doom sector.
        /// </summary>
        /// <param name="floorHeight">The floor height.</param>
        /// <param name="ceilingHeight">The ceiling height.</param>
        /// <param name="floorTexture">The floor texture.</param>
        /// <param name="ceilingTexture">The ceiling texture.</param>
        /// <param name="lightLevel">The light level.</param>
        /// <param name="specialBits">The bits for the sector special.</param>
        /// <param name="tag">The sector tag.</param>
        public DoomSector(short floorHeight, short ceilingHeight, UpperString floorTexture,
                          UpperString ceilingTexture, short lightLevel, ushort specialBits,
                          ushort tag)
        {
            FloorHeight = floorHeight;
            CeilingHeight = ceilingHeight;
            FloorTexture = floorTexture;
            CeilingTexture = ceilingTexture;
            LightLevel = lightLevel;
            Tag = tag;
        }
    }
}
