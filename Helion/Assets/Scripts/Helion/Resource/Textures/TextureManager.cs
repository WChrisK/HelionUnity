using System;
using System.Collections.Generic;
using Helion.Archives;
using Helion.Graphics;
using Helion.Resource.Colors.Palettes;
using Helion.Resource.Textures.Definitions;
using Helion.Util;
using Helion.Util.Geometry;
using Helion.Util.Logging;
using MoreLinq;
using UnityEngine;

namespace Helion.Resource.Textures
{
    using PaletteReaderFunc = Func<byte[], ResourceNamespace, Optional<PaletteImage>>;

    /// <summary>
    /// Manages all of the texture resources in the archive.
    /// </summary>
    public static class TextureManager
    {
        private static readonly Log Log = LogManager.Instance();

        // These do not need to be recreated on clearing and should persist.
        public static readonly Texture NullTexture = CreateNullTexture();
        private static readonly Shader defaultShader = Shader.Find("Doom/Default");

        // These need to be recreated for each clearing.
        public static Palette Palette { get; private set; } = Palette.CreateDefault();
        private static ResourceTracker<Texture> textures = new ResourceTracker<Texture>();
        private static ResourceTracker<RgbaImage> loadedImages = new ResourceTracker<RgbaImage>();
        private static HashSet<UpperString> missingTextureNames = new HashSet<UpperString>();

        /// <summary>
        /// Gets a material for the name. If the namespace is provided, it will
        /// give priority to that namespace. If the material cannot be found,
        /// the null material is returned.
        /// </summary>
        /// <param name="name">The material name.</param>
        /// <param name="priorityNamespace">The namespace to search for first
        /// in.</param>
        /// <returns>The material that was found, or the 'null material' for
        /// rendering with.</returns>
        public static Texture Texture(UpperString name, ResourceNamespace priorityNamespace = ResourceNamespace.Global)
        {
            return TryGetTexture(name, priorityNamespace, out Texture texture) ? texture : NullTexture;
        }

        public static bool TryGetTexture(UpperString name, ResourceNamespace priorityNamespace,
            out Texture texture)
        {
            if (textures.TryGetValue(name, priorityNamespace, out texture))
                return true;

            // This is a heuristic to make failed lookups less expensive, as
            // there is a lot of work done to create the texture if it does
            // not exist after this.
            if (missingTextureNames.Contains(name))
            {
                texture = NullTexture;
                return false;
            }

            if (TryCreateExactNamespaceTexture(name, priorityNamespace, out texture))
            {
                textures.Add(name, priorityNamespace, texture);
                return true;
            }

            if (textures.TryGetAnyValue(name, out texture, out _))
                return true;

            if (TryCreateAnyNamespaceTexture(name, out texture, out ResourceNamespace newNamespace))
            {
                textures.Add(name, newNamespace, texture);
                return true;
            }

            missingTextureNames.Add(name);

            texture = NullTexture;
            return false;
        }

        public static void Clear()
        {
            textures.ForEach(tex => tex.Dispose());

            Palette = Palette.CreateDefault();
            textures = new ResourceTracker<Texture>();
            loadedImages = new ResourceTracker<RgbaImage>();
            missingTextureNames = new HashSet<UpperString>();
        }

        internal static void TrackPalette(IEntry entry)
        {
            Optional<Palette> palette = Palette.From(entry.Data);
            if (palette)
                Palette = palette.Value;
            else
                Log.Error("Unable to read palette from entry: ", entry.Path);
        }

        private static Texture CreateNullTexture()
        {
            Material material = Resources.Load<Material>("Materials/null");
            Dimension dimension = new Dimension(material.mainTexture.width, material.mainTexture.height);
            return new Texture("NULL", material, dimension);
        }

        private static bool TryCreateExactNamespaceTexture(UpperString name, ResourceNamespace resourceNamespace,
            out Texture texture)
        {
            if (loadedImages.TryGetValue(name, resourceNamespace, out RgbaImage loadedImage))
            {
                texture = CreateAndTrackTexture(name, resourceNamespace, loadedImage);
                return true;
            }

            if (TextureDefinitionManager.TryGetExact(name, resourceNamespace, out TextureDefinition definition))
            {
                RgbaImage compiledImage = TextureDefinitionToImage(definition);
                loadedImages.Add(name, resourceNamespace, compiledImage);
                texture = CreateAndTrackTexture(name, resourceNamespace, compiledImage);
                return true;
            }

            if (Data.TryFindExact(name, resourceNamespace, out IEntry entry))
            {
                if (TryReadImageEntry(entry, resourceNamespace, out RgbaImage newImage))
                {
                    loadedImages.Add(name, resourceNamespace, newImage);
                    texture = CreateAndTrackTexture(name, resourceNamespace, newImage);
                    return true;
                }
            }

            texture = null;
            return false;
        }

        private static bool TryCreateAnyNamespaceTexture(UpperString name, out Texture texture,
            out ResourceNamespace newNamespace)
        {
            if (loadedImages.TryGetAnyValue(name, out RgbaImage loadedImage, out newNamespace))
            {
                texture = CreateAndTrackTexture(name, newNamespace, loadedImage);
                return true;
            }

            if (TextureDefinitionManager.TryGetAny(name, out TextureDefinition definition, out ResourceNamespace definitionNamespace))
            {
                RgbaImage compiledImage = TextureDefinitionToImage(definition);
                loadedImages.Add(name, definitionNamespace, compiledImage);
                texture = CreateAndTrackTexture(name, definitionNamespace, compiledImage);
                return true;
            }

            if (Data.TryFindAny(name, out IEntry entry))
            {
                if (TryReadImageEntry(entry, entry.Namespace, out RgbaImage newImage))
                {
                    loadedImages.Add(name, entry.Namespace, newImage);
                    texture = CreateAndTrackTexture(name, entry.Namespace, newImage);
                    return true;
                }
            }

            texture = null;
            newNamespace = ResourceNamespace.Global;
            return false;
        }

        private static bool TryGetOrReadImage(UpperString name, ResourceNamespace resourceNamespace,
            out RgbaImage rgbaImage)
        {
            if (loadedImages.TryGetValue(name, resourceNamespace, out rgbaImage))
                return true;

            if (Data.TryFindExact(name, resourceNamespace, out IEntry entry))
            {
                if (TryReadImageEntry(entry, entry.Namespace, out rgbaImage))
                {
                    loadedImages.Add(name, entry.Namespace, rgbaImage);
                    return true;
                }
            }

            rgbaImage = null;
            return false;
        }

        private static bool TryReadImageEntry(IEntry entry, ResourceNamespace resourceNamespace,
            out RgbaImage rgbaImage)
        {
            byte[] data = entry.Data;

            // TODO: Handle PNG/JPG/...etc, here!

            // We want to give priority to reading flats if it's coming from a
            // flat namespace. This way we reduce false positive hits... which
            // should be very rare. Usually column palette images are in the
            // global namespace anyways and flats are generally in the flat
            // namespace.
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

                rgbaImage = paletteImage.Value.ToColor(Palette);
                return true;
            }

            rgbaImage = null;
            return false;
        }

        private static RgbaImage TextureDefinitionToImage(TextureDefinition definition)
        {
            RgbaImage newImage = new RgbaImage(definition.Dimension)
            {
                Namespace = definition.Namespace,
                Offset = definition.Offset
            };

            foreach (TextureDefinitionPatch patch in definition.Patches)
            {
                if (TryGetOrReadImage(patch.Name, patch.Namespace, out RgbaImage image))
                {
                    Vec2I position = patch.Origin;
                    if (patch.UseOffsets)
                        position += image.Offset;

                    // TODO: Handle the other fields on the patch (ex: flipping, rotation).

                    newImage.DrawOntoThis(image, position);
                }
                else
                    throw new Exception($"Cannot find image patch {patch.Name} in texture definition {definition.Name}");
            }

            return newImage;
        }

        private static Texture CreateAndTrackTexture(UpperString name, ResourceNamespace resourceNamespace,
            RgbaImage image)
        {
            Texture texture = new Texture(name, resourceNamespace, defaultShader, image);
            textures.Add(name, resourceNamespace, texture);

            // To make things look a bit more reasonable with sprites, we'll
            // do GL_NEAREST since filtering makes them blurry and ugly.
            if (resourceNamespace == ResourceNamespace.Sprites)
                texture.Material.mainTexture.filterMode = FilterMode.Point;

            return texture;
        }
    }
}
