using System;
using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Graphics;
using Helion.Core.Resource.Colors.Palettes;
using Helion.Core.Resource.Textures.Definitions;
using Helion.Core.Util;
using Helion.Core.Util.Logging;
using Helion.Core.Util.Unity;
using MoreLinq;
using UnityEngine;

namespace Helion.Core.Resource.Textures
{
    using PaletteReaderFunc = Func<byte[], ResourceNamespace, Optional<PaletteImage>>;

    public static class TextureManagerNew
    {
        private static readonly Log Log = LogManager.Instance();
        private static readonly Shader defaultShader = Shader.Find("Doom/Default");
        private static readonly Material nullMaterial = Resources.Load<Material>("Materials/null");

        // The following must be recreated on every Clear() call.
        public static Palette Palette { get; private set; } = Palette.CreateDefault();
        private static ResourceTracker<Material> materials = new ResourceTracker<Material>();
        private static ResourceTracker<RgbaImage> loadedImages = new ResourceTracker<RgbaImage>();
        private static HashSet<UpperString> missingTextureNames = new HashSet<UpperString>();

        public static Material Material(UpperString name, ResourceNamespace priorityNamespace = ResourceNamespace.Global)
        {
            if (materials.TryGetValue(name, priorityNamespace, out Material priorityMaterial))
                return priorityMaterial;

            // This is a heuristic to make failed lookups less expensive, as
            // there is a lot of work done to create the texture if it does
            // not exist after this.
            if (missingTextureNames.Contains(name))
                return nullMaterial;

            if (TryCreateExactNamespaceMaterial(name, priorityNamespace, out Material newPriorityMaterial))
            {
                materials.Add(name, priorityNamespace, newPriorityMaterial);
                return newPriorityMaterial;
            }

            if (materials.TryGetAnyValue(name, out Material nonPriorityMaterial, out _))
                return nonPriorityMaterial;

            if (TryCreateAnyNamespaceMaterial(name, out Material newMaterial, out ResourceNamespace newNamespace))
            {
                materials.Add(name, newNamespace, newMaterial);
                return newMaterial;
            }

            missingTextureNames.Add(name);

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
                RgbaImage compiledImage = TextureDefinitionToImage(definition, resourceNamespace);
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
                RgbaImage compiledImage = TextureDefinitionToImage(definition, definitionNamespace);
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
                if (paletteImage)
                {
                    rgbaImage = paletteImage.Value.ToColor(Palette);
                    return true;
                }
            }

            rgbaImage = null;
            return false;
        }

        private static RgbaImage TextureDefinitionToImage(TextureDefinition definition, ResourceNamespace resourceNamespace)
        {
            if (definition.Width <= 0 || definition.Height <= 0)
            {
                Log.Error("Bad texture dimensions for texture definition: ", definition.Name);
                return RgbaImage.From(4, 4).Value;
            }

            RgbaImage image = RgbaImage.From(definition.Width, definition.Height).Value;

            // TODO: Fill in image here.

            return image;
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
