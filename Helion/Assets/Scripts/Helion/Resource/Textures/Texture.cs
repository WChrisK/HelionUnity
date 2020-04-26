using System;
using Helion.Graphics;
using Helion.Util;
using Helion.Util.Geometry;
using Helion.Util.Unity;
using UnityEngine;
using static Helion.Util.OptionalHelper;

namespace Helion.Resource.Textures
{
    /// <summary>
    /// A texture with extra information used in rendering.
    /// </summary>
    public class Texture : IDisposable
    {
        public readonly UpperString Name;
        public readonly ResourceNamespace Namespace;
        public readonly Material Material;
        public readonly Dimension Dimension;
        public readonly Vector2 InverseDimension;
        public readonly Optional<RgbaImage> Rgba = Empty;
        public readonly Optional<PaletteImage> Palette = Empty;

        public int Width => Dimension.Width;
        public int Height => Dimension.Height;
        public float InvWidth => InverseDimension.x;
        public float InvHeight => InverseDimension.y;

        public Texture(UpperString name, Material material, Dimension dimension)
        {
            Name = name;
            Namespace = ResourceNamespace.Global;
            Material = material;
            Dimension = dimension;
            InverseDimension = new Vector2(1.0f / Dimension.Width, 1.0f / Dimension.Height);

            material.name = name.String;
        }

        public Texture(UpperString name, ResourceNamespace resourceNamespace, Shader shader,
            RgbaImage rgbaImage, PaletteImage paletteImage = null)
        {
            Name = name;
            Namespace = resourceNamespace;
            Material = new Material(shader)
            {
                mainTexture = rgbaImage.ToTexture(),
                name = $"{name.String} ({resourceNamespace})"
            };
            Dimension = rgbaImage.Dimension;
            InverseDimension = new Vector2(1.0f / Dimension.Width, 1.0f / Dimension.Height);
            Rgba = new Optional<RgbaImage>(rgbaImage);
            Palette = new Optional<PaletteImage>(paletteImage);
        }

        public void Dispose()
        {
            GameObjectHelper.Destroy(Material.mainTexture);
            GameObjectHelper.Destroy(Material);
        }
    }
}
