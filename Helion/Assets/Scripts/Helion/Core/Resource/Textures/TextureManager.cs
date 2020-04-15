using System;
using Helion.Core.Archives;
using Helion.Core.Graphics;
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
        private static readonly Shader defaultShader = Shader.Find("Doom/Default");
        private static readonly Material NullMaterial = Resources.Load<Material>("Materials/null");

        public Palette Palette { get; private set; } = Palette.CreateDefault();
        private readonly ResourceTracker<Material> materials = new ResourceTracker<Material>();
        private readonly ResourceTracker<RgbaImage> loadedImages = new ResourceTracker<RgbaImage>();
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
            if (materials.TryGetValue(name, resourceNamespace, out Material material))
                return material;

            if (TryCreateFromExistingImage(name, resourceNamespace, out material))
                return material;

            return NullMaterial;
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
            // TODO: Clean up all the materials.
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

        internal void FinishPostProcessingOrThrow()
        {
            // TODO: Refactor me please...
            foreach ((PNames pnames, TextureX textureX) in vanillaTextureTracker)
            {
                foreach (TextureXImage textureXImage in textureX)
                {
                    int w = textureXImage.Dimension.Width;
                    int h = textureXImage.Dimension.Height;
                    Optional<RgbaImage> imageOpt = RgbaImage.From(w, h).Value;
                    if (!imageOpt)
                        throw new Exception($"Bad dimensions for TextureX image: {textureXImage.Name}");

                    RgbaImage image = imageOpt.Value;

                    foreach (TextureXPatch patch in textureXImage.Patches)
                    {
                        UpperString name = pnames[patch.PatchIndex];

                        if (!loadedImages.TryGetValue(name, ResourceNamespace.Textures, out RgbaImage loadedImage))
                        {
                            // TODO: Search by namespace priority!
                            Optional<IEntry> entry = Data.Find(name);
                            if (!entry)
                                throw new Exception($"Unable to find entry for texture resource {name}");

                            Optional<PaletteImage> paletteImage = PaletteImage.FromColumn(entry.Value.Data, ResourceNamespace.Textures);
                            if (!paletteImage)
                                throw new Exception($"Corrupt palette image from texture resource {name}");

                            RgbaImage convertedImage = paletteImage.Value.ToColor(Palette);
                            loadedImages.Add(name, ResourceNamespace.Textures, convertedImage);
                            loadedImage = convertedImage;
                        }

                        // OPTIMIZE: If the definition is the exact same image at offset <0, 0>, don't bother.
                        bool success = image.DrawOntoThis(loadedImage, patch.Offset);
                        if (!success)
                            Log.Error($"Unable to draw patch {name} onto composite texture {textureXImage.Name}");
                    }

                    loadedImages.Add(textureXImage.Name, ResourceNamespace.Textures, image);
                }
            }
        }

        private bool TryCreateFromExistingImage(UpperString name, ResourceNamespace resourceNamespace,
            out Material material)
        {
            if (loadedImages.TryGetValue(name, resourceNamespace, out RgbaImage image))
            {
                material = ImageToMaterial(name, ResourceNamespace.Flats, image);
                return true;
            }

            if (resourceNamespace == ResourceNamespace.Flats)
            {
                // TODO: Find by flat namespace only!
                Optional<PaletteImage> flat = Data.Find(name).Map(entry => PaletteImage.FromFlat(entry.Data, resourceNamespace).Value);
                if (flat)
                {
                    RgbaImage convertedImage = flat.Value.ToColor(Palette);
                    loadedImages.Add(name, ResourceNamespace.Flats, convertedImage);

                    material = ImageToMaterial(name, ResourceNamespace.Flats, convertedImage);
                    return true;
                }
            }

            material = null;
            return false;
        }

        private Material ImageToMaterial(UpperString name, ResourceNamespace resourceNamespace, RgbaImage image)
        {
            Texture2D texture = image.ToTexture();
            Material material = new Material(defaultShader) { mainTexture = texture };
            Material existingMaterial = materials.Add(name, resourceNamespace, material);

            if (existingMaterial != null)
            {
                // TODO: Destroy
                Log.Error($"CRITICAL: Leaking material reference! Need to destroy {name}!");
            }

            return material;
        }
    }
}
