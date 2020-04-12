using System;
using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Graphics;
using Helion.Core.Resource.Textures.Definitions.Vanilla;
using Helion.Core.Util;
using Helion.Core.Util.Geometry;
using UnityEngine;

namespace Helion.Core.Resource.Textures
{
    public class TextureManager
    {
        private Dictionary<UpperString, Material> Materials = new Dictionary<UpperString, Material>();
        private Dictionary<UpperString, Texture2D> Textures = new Dictionary<UpperString, Texture2D>();

        public Material FindMaterial(UpperString name)
        {
            if (Materials.TryGetValue(name, out Material material))
                return material;

            Debug.Log($"Cannot find material: {name}");
            return Resources.Load<Material>("Materials/null");
        }

        public void Update(ResourceManager resources)
        {
            Shader shader = Shader.Find("Sprites/Default");

            try
            {
                // 1) Handle textures.
                PNames pnames = resources.PNamesDefinitions[0];
                foreach (TextureXImage textureXImage in resources.TextureXDefinitions[0])
                {
                    UpperString textureName = textureXImage.Name;

                    if (textureXImage.Patches.Count != 1)
                        continue;

                    TextureXPatch patch = textureXImage.Patches[0];
                    if (patch.Offset != Vec2I.Zero)
                        continue;

                    UpperString patchName = pnames[patch.PatchIndex];
                    IEntry entry = GameData.Find(patchName).Value;

                    Optional<PaletteImage> paletteImageOpt = PaletteImage.FromColumn(entry.Data, ResourceNamespace.Textures);
                    if (!paletteImageOpt)
                        continue;

                    PaletteImage paletteImage = paletteImageOpt.Value;
                    if (paletteImage.Dimension != textureXImage.Dimension)
                        continue;

                    RgbaImage rgbaImage = paletteImage.ToColor(resources.Palette);
                    Texture2D texture = rgbaImage.ToTexture();
                    Textures[textureName] = texture;

                    Material material = new Material(shader);
                    material.mainTexture = texture;
                    Materials[textureName] = material;
                }

                // 2) Handle floors: TODO: Holy crap this is hacky garbage...
                foreach (IArchive archive in GameData.Archives)
                {
                    foreach (IEntry entry in archive)
                    {
                        if (entry.Namespace != ResourceNamespace.Flats)
                            continue;

                        Optional<PaletteImage> paletteImageOpt = PaletteImage.FromFlat(entry.Data, ResourceNamespace.Textures);
                        if (!paletteImageOpt)
                            continue;

                        UpperString flatName = entry.Name;
                        PaletteImage paletteImage = paletteImageOpt.Value;
                        RgbaImage rgbaImage = paletteImage.ToColor(resources.Palette);
                        Texture2D texture = rgbaImage.ToTexture();
                        Textures[flatName] = texture;

                        Material material = new Material(shader);
                        material.mainTexture = texture;
                        Materials[flatName] = material;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Unexpected texture processing error: {e.Message}");
            }
        }
    }
}
