using System;
using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Resource.Colors.Palettes;
using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Resource.Textures
{
    /// <summary>
    /// Manages all of the texture resources in the archive.
    /// </summary>
    public class TextureManager : IDisposable
    {
        private static readonly Material NullMaterial = Resources.Load<Material>("Materials/null");

        public Palette Palette { get; private set; }
        private readonly ResourceTracker<Material> materials = new ResourceTracker<Material>();
        private readonly List<IEntry> textureDefinitionEntries = new List<IEntry>();

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

        internal void TrackPNames(IEntry entry)
        {
            textureDefinitionEntries.Add(entry);
        }

        internal void TrackTextureX(IEntry entry)
        {
            textureDefinitionEntries.Add(entry);
        }

        internal void FinishPostProcessing()
        {
            // Note: This is not how Pnames/TextureX are resolved...
            // TODO
        }
    }
}
