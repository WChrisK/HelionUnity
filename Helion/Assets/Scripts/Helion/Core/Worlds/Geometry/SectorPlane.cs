using System.Collections.Generic;
using Helion.Core.Resource.Textures;
using Helion.Core.Util;
using Helion.Core.Worlds.Geometry.Subsectors;
using Helion.Core.Worlds.Geometry.Walls;
using Texture = Helion.Core.Resource.Textures.Texture;

namespace Helion.Core.Worlds.Geometry
{
    /// <summary>
    /// A plane that belongs to a sector. This can either be a floor, ceiling,
    /// or something like a 3D floor/transfer heights plane.
    /// </summary>
    public class SectorPlane
    {
        /// <summary>
        /// The index in a list of sector planes.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// The sector this plane is part of.
        /// </summary>
        public Sector Sector { get; internal set; }

        /// <summary>
        /// True if this is a ceiling, false if it's a floor.
        /// </summary>
        public readonly bool IsCeiling;

        /// <summary>
        /// The vertical height on the level if this is not a sloped sector
        /// plane.
        /// </summary>
        public int Height
        {
            get => height;
            set
            {
                height = value;
                SubsectorPlanes.ForEach(subsectorPlane => subsectorPlane.UpdateMeshes());
                WallListeners.ForEach(wall => wall.UpdateWallMesh());
            }
        }

        /// <summary>
        /// The specific light level for this sector plane. Usually the parent
        /// light level is used, but if this is not null then this should be
        /// used in place of it.
        /// </summary>
        // TODO: Setting this should update mesh colors and update `normalized`. Use custom setter.
        public int? OverrideLightLevel { get; }

        public float? OverrideLightLevelNormalized { get; private set; }

        /// <summary>
        /// The material used for rendering the subsectors with.
        /// </summary>
        public Texture Texture { get; private set; }

        /// <summary>
        /// This is a list of walls that want to listen to any height changes
        /// because it means they may have to change their mesh or delete their
        /// mesh if they become a zero-height thing.
        /// </summary>
        public readonly List<Wall> WallListeners = new List<Wall>();

        /// <summary>
        /// All of the subsectors that use this sector plane and should listen
        /// for any height changes.
        /// </summary>
        public readonly List<SubsectorPlane> SubsectorPlanes = new List<SubsectorPlane>();

        private UpperString textureName;
        private int height;

        /// <summary>
        /// Gets the light level that should be used. This will select the
        /// appropriate light level from either the sector, or the light level
        /// specifically for this plane if overridden. This should not be used
        /// for rendering, use <see cref="LightLevelNormalized"/> instead.
        /// </summary>
        public int LightLevel => OverrideLightLevel ?? Sector.LightLevel;

        /// <summary>
        /// Gets the normalized light level.
        /// </summary>
        public float LightLevelNormalized => OverrideLightLevelNormalized ?? Sector.LightLevelNormalized;

        /// <summary>
        /// Checks if this is a floor or not.
        /// </summary>
        public bool IsFloor => !IsCeiling;

        /// <summary>
        /// Gets the texture name. The setter will update the texture, and will
        /// apply a new material to the subsectors in the world.
        /// </summary>
        public UpperString TextureName
        {
            get => textureName;
            set
            {
                if (value != null)
                {
                    textureName = value;
                    Texture = TextureManager.Texture(value);
                    // TODO: Update all subsectors!
                }
            }
        }

        public SectorPlane(int index, bool isCeiling, int verticalHeight, UpperString texture)
        {
            Index = index;
            IsCeiling = isCeiling;
            height = verticalHeight;
            TextureName = texture;
            Texture = TextureManager.Texture(texture);
        }
    }
}
