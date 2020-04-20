using System;
using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Graphics;
using Helion.Core.Resource.Colors.Palettes;
using Helion.Core.Resource.Textures.Definitions;
using Helion.Core.Util;
using Helion.Core.Util.Geometry;
using Helion.Core.Util.Unity;
using MoreLinq;
using UnityEngine;

namespace Helion.Core.Resource.Textures
{
    using PaletteReaderFunc = Func<byte[], ResourceNamespace, Optional<PaletteImage>>;

    /// <summary>
    /// Manages all of the texture resources in the archive.
    /// </summary>
    public static class TextureManagerNew
    {
        private static readonly Shader defaultShader = Shader.Find("Doom/Default");
        private static readonly Material nullMaterial = Resources.Load<Material>("Materials/null");

        // The following must be recreated on every Clear() call.
        public static Palette Palette { get; private set; } = Palette.CreateDefault();
        private static ResourceTracker<Material> materials = new ResourceTracker<Material>();
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
        public static Material Material(UpperString name, ResourceNamespace priorityNamespace = ResourceNamespace.Global)
        {
            return TryGetMaterial(name, priorityNamespace, out _);
        }

        public static Material TryGetMaterial(UpperString name, ResourceNamespace priorityNamespace,
            out bool isNullMaterial)
        {
            if (materials.TryGetValue(name, priorityNamespace, out Material material))
            {
                isNullMaterial = false;
                return material;
            }

            // This is a heuristic to make failed lookups less expensive, as
            // there is a lot of work done to create the texture if it does
            // not exist after this.
            if (missingTextureNames.Contains(name))
            {
                isNullMaterial = true;
                return nullMaterial;
            }

            if (TryCreateExactNamespaceMaterial(name, priorityNamespace, out Material newPriorityMaterial))
            {
                materials.Add(name, priorityNamespace, newPriorityMaterial);
                isNullMaterial = false;
                return newPriorityMaterial;
            }

            if (materials.TryGetAnyValue(name, out material, out _))
            {
                isNullMaterial = false;
                return material;
            }

            if (TryCreateAnyNamespaceMaterial(name, out Material newMaterial, out ResourceNamespace newNamespace))
            {
                materials.Add(name, newNamespace, newMaterial);
                isNullMaterial = false;
                return newMaterial;
            }

            missingTextureNames.Add(name);

            isNullMaterial = true;
            return nullMaterial;
        }

        public static void Clear()
        {
            materials.ForEach(DestroyMaterialAndTexture);

            Palette = Palette.CreateDefault();
            materials = new ResourceTracker<Material>();
            loadedImages = new ResourceTracker<RgbaImage>();
            missingTextureNames = new HashSet<UpperString>();
        }

        private static bool TryCreateExactNamespaceMaterial(UpperString name, ResourceNamespace resourceNamespace,
            out Material material)
        {
            if (loadedImages.TryGetValue(name, resourceNamespace, out RgbaImage loadedImage))
            {
                material = CreateAndTrackMaterial(name, resourceNamespace, loadedImage);
                return true;
            }

            if (TextureDefinitionManager.TryGetExact(name, resourceNamespace, out TextureDefinition definition))
            {
                RgbaImage compiledImage = TextureDefinitionToImage(definition);
                loadedImages.Add(name, resourceNamespace, compiledImage);
                material = CreateAndTrackMaterial(name, resourceNamespace, compiledImage);
                return true;
            }

            if (DataNew.TryFind(name, resourceNamespace, out IEntry entry))
            {
                if (TryReadImageEntry(entry, resourceNamespace, out RgbaImage newImage))
                {
                    loadedImages.Add(name, resourceNamespace, newImage);
                    material = CreateAndTrackMaterial(name, resourceNamespace, loadedImage);
                    return true;
                }
            }

            material = null;
            return false;
        }

        private static bool TryCreateAnyNamespaceMaterial(UpperString name, out Material material,
            out ResourceNamespace newNamespace)
        {
            if (loadedImages.TryGetAnyValue(name, out RgbaImage loadedImage, out newNamespace))
            {
                material = CreateAndTrackMaterial(name, newNamespace, loadedImage);
                return true;
            }

            if (TextureDefinitionManager.TryGetAny(name, out TextureDefinition definition, out ResourceNamespace definitionNamespace))
            {
                RgbaImage compiledImage = TextureDefinitionToImage(definition);
                loadedImages.Add(name, definitionNamespace, compiledImage);
                material = CreateAndTrackMaterial(name, definitionNamespace, compiledImage);
                return true;
            }

            if (DataNew.TryFind(name, out IEntry entry))
            {
                if (TryReadImageEntry(entry, entry.Namespace, out RgbaImage newImage))
                {
                    loadedImages.Add(name, entry.Namespace, newImage);
                    material = CreateAndTrackMaterial(name, entry.Namespace, loadedImage);
                    return true;
                }
            }

            material = null;
            newNamespace = ResourceNamespace.Global;
            return false;
        }

        private static bool TryGetOrReadImage(UpperString name, ResourceNamespace resourceNamespace,
            out RgbaImage rgbaImage)
        {
            if (loadedImages.TryGetValue(name, resourceNamespace, out rgbaImage))
                return true;

            if (DataNew.TryFind(name, out IEntry entry))
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

        private static Material CreateAndTrackMaterial(UpperString name, ResourceNamespace resourceNamespace,
            RgbaImage image)
        {
            Material material = new Material(defaultShader)
            {
                mainTexture = image.ToTexture(),
                name = $"{name.String} ({resourceNamespace})"
            };
            materials.Add(name, resourceNamespace, material);

            // To make things look a bit more reasonable with sprites, we'll
            // do GL_NEAREST since filtering makes them blurry and ugly.
            if (resourceNamespace == ResourceNamespace.Sprites)
                material.mainTexture.filterMode = FilterMode.Point;

            return material;
        }

        private static void DestroyMaterialAndTexture(Material material)
        {
            if (material.mainTexture != null)
                GameObjectHelper.Destroy(material.mainTexture);
            GameObjectHelper.Destroy(material);
        }
    }
}
