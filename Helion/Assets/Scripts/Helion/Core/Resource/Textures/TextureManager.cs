using System;
using Helion.Core.Archives;
using Helion.Core.Graphics;
using Helion.Core.Resource.Colors.Palettes;
using Helion.Core.Resource.Textures.Definitions.Vanilla;
using Helion.Core.Util;
using Helion.Core.Util.Logging;
using Helion.Core.Util.Unity;
using MoreLinq;
using UnityEngine;

// TODO: This needs a huge overhaul...

namespace Helion.Core.Resource.Textures
{
    using PaletteReaderFunc = Func<byte[], ResourceNamespace, Optional<PaletteImage>>;

    /// <summary>
    /// Manages all of the texture resources in the archive.
    /// </summary>
    public class TextureManager : IDisposable
    {
        private static readonly Log Log = LogManager.Instance();
        private static readonly Shader defaultShader = Shader.Find("Doom/Default");
        private static readonly Material nullMaterial = Resources.Load<Material>("Materials/null");

        public Palette Palette { get; private set; } = Palette.CreateDefault();
        private readonly ResourceTracker<Material> materials = new ResourceTracker<Material>();
        private readonly ResourceTracker<RgbaImage> loadedImages = new ResourceTracker<RgbaImage>();
        private readonly VanillaTextureTracker vanillaTextureTracker = new VanillaTextureTracker();

        /// <summary>
        /// Gets the null material that indicates a material is missing. This
        /// can be used to render with and will never be absent/null.
        /// </summary>
        public Material NullMaterial => nullMaterial;

        /// <summary>
        /// Gets the material for the name/namespace provided. Priority is
        /// given to the namespace provided, but will search other namespaces
        /// if the priority one cannot be found.
        /// </summary>
        /// <param name="name">The material name.</param>
        /// <param name="resourceNamespace">The namespace of the material. By
        /// default is the global namespace.</param>
        /// <returns>The material, or a "null" texture material if it does not
        /// exist but still can be rendered with.</returns>
        public Material Material(UpperString name, ResourceNamespace resourceNamespace = ResourceNamespace.Global)
        {
            if (TryGetMaterial(name, resourceNamespace, out Material material))
                return material;
            return nullMaterial;
        }

        /// <summary>
        /// Gets the material for the name/namespace provided if available.
        /// </summary>
        /// <param name="name">The material name.</param>
        /// <param name="priorityNamespace">The namespace of the material.
        /// </param>
        /// <param name="material">The material if found, null otherwise.
        /// </param>
        /// <returns>True if found, false if not.</returns>
        public bool TryGetMaterial(UpperString name, ResourceNamespace priorityNamespace, out Material material)
        {
            if (materials.TryGetValue(name, priorityNamespace, out material))
                return material;

            if (TryCreateFromExistingImage(name, priorityNamespace, out material))
                return true;

            // TODO: Call materials.TryGetAnyValue? The ordering above needs some thought...

            material = null;
            return false;
        }

        public void Dispose()
        {
            materials.ForEach(DestroyMaterialAndTexture);
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

            Optional<IEntry> entry = Data.FindPriority(name, resourceNamespace);
            if (!entry)
            {
                material = null;
                return false;
            }

            byte[] data = entry.Value.Data;

            // TODO: If extension is PNG/JPG/whatever, or has a header of that type, do that here!

            // We want to give priority to reading flats if it's coming from a
            // flat namespace. This way we reduce false positive hits... which
            // should be very rare anyways. Usually column palette images are
            // in the global namespace anyways and flats are generally in the
            // flat namespace.
            PaletteReaderFunc[] paletteReaders;
            if (resourceNamespace == ResourceNamespace.Flats)
                paletteReaders = new PaletteReaderFunc[] { PaletteImage.FromFlat, PaletteImage.FromColumn };
            else
                paletteReaders = new PaletteReaderFunc[] { PaletteImage.FromColumn, PaletteImage.FromFlat };

            foreach (PaletteReaderFunc readerFunc in paletteReaders)
            {
                Optional<PaletteImage> paletteImage = readerFunc(data, resourceNamespace);
                if (!paletteImage)
                    continue;

                RgbaImage convertedImage = paletteImage.Value.ToColor(Palette);
                loadedImages.Add(name, resourceNamespace, convertedImage);
                material = ImageToMaterial(name, resourceNamespace, convertedImage);
                return true;
            }

            material = null;
            return false;
        }

        private Material ImageToMaterial(UpperString name, ResourceNamespace resourceNamespace, RgbaImage image)
        {
            Texture2D texture = image.ToTexture();

            // We don't want interpolation on sprites... for now...
            if (resourceNamespace == ResourceNamespace.Sprites)
                texture.filterMode = FilterMode.Point;

            Material material = new Material(defaultShader)
            {
                mainTexture = texture,
                name = $"{name.String} ({resourceNamespace})"
            };

            Material existingMaterial = materials.Add(name, resourceNamespace, material);
            // ...

            return material;
        }

        private void DestroyMaterialAndTexture(Material material)
         {
             if (material.mainTexture != null)
                 GameObjectHelper.Destroy(material.mainTexture);
             GameObjectHelper.Destroy(material);
         }
    }
}
