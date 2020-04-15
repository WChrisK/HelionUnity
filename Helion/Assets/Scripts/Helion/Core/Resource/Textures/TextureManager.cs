using System;
using Helion.Core.Archives;
using Helion.Core.Resource.Colors.Palettes;
using Helion.Core.Resource.Textures.Definitions.Vanilla;
using Helion.Core.Util;
using Helion.Core.Util.Logging;
using UnityEngine;

namespace Helion.Core.Resource.Textures
{
    /// <summary>
    /// Manages all of the texture resources in the archive.
    /// </summary>
    public class TextureManager : IDisposable
    {
        private static readonly Log Log = LogManager.Instance();
        private static readonly Material NullMaterial = Resources.Load<Material>("Materials/null");

        public Palette Palette { get; private set; } = Palette.CreateDefault();
        private readonly ResourceTracker<Material> materials = new ResourceTracker<Material>();
        private readonly VanillaTextureTracker vanillaTextureTracker = new VanillaTextureTracker();

        /// <summary>
        /// Gets the material for the name/namespace provided.
        /// </summary>
        /// <param name="name">The material name.</param>
        /// <param name="resourceNamespace">The namespace of the material.
        /// </param>
        /// <returns>The material, or a "null" texture material if it does not
        /// exist but still can be rendered with.</returns>
        public Material Material(UpperString name, ResourceNamespace resourceNamespace)
        {
            return materials.TryGetValue(name, resourceNamespace, out Material material) ? material : NullMaterial;
        }

        /// <summary>
        /// Gets the texture for the name/namespace provided. This should not
        /// be called with <see cref="Material"/> since it will do two lookups
        /// for the same data. You should instead access `mainTexture` from the
        /// material.
        /// </summary>
        /// <param name="name">The texture name.</param>
        /// <param name="resourceNamespace">The namespace of the texture.
        /// </param>
        /// <returns>The texture, or a "null" texture texture if it does not
        /// exist but still can be rendered with.</returns>
        public Texture Texture(UpperString name, ResourceNamespace resourceNamespace)
        {
            return Material(name, resourceNamespace).mainTexture;
        }

        public void Dispose()
        {
            // TODO
        }

        internal void HandlePaletteOrThrow(IEntry entry)
        {
            Optional<Palette> palette = Palette.From(entry.Data);
            if (!palette)
                throw new Exception("Palette is corrupt");

            Palette = palette.Value;
        }

        internal void TrackPNamesOrThrow(IEntry entry, IArchive archive)
        {
            Optional<PNames> pnames = PNames.From(entry.Data);
            if (!pnames)
                throw new Exception($"PNAMES is corrupt (at {entry.Path})");

            vanillaTextureTracker.Track(pnames.Value, archive);
        }

        internal void TrackTextureXOrThrow(IEntry entry, IArchive archive)
        {
            Optional<TextureX> textureX = TextureX.From(entry);
            if (!textureX)
                throw new Exception($"TEXTUREx is corrupt (at {entry.Path})");

            vanillaTextureTracker.Track(textureX.Value, archive);
        }

        internal void FinishPostProcessing()
        {
            // TODO
        }
    }
}
