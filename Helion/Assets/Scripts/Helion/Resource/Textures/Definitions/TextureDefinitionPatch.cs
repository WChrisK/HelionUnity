using Helion.Resource.Colors;
using Helion.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Util;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;
using UnityEngine;
using static Helion.Util.OptionalHelper;

namespace Helion.Resource.Textures.Definitions
{
    public class TextureDefinitionPatch
    {
        public readonly UpperString Name;
        public readonly Vec2I Origin;
        public readonly ResourceNamespace Namespace;
        public bool FlipX;
        public bool FlipY;
        public bool UseOffsets;
        public TextureRotation Rotation = TextureRotation.None;
        public Optional<Translation> Translation = Empty;
        public Color? Blend;
        public float Alpha = 1.0f;
        public RenderStyle RenderStyle = RenderStyle.Normal;

        public TextureDefinitionPatch(UpperString name, Vec2I origin, ResourceNamespace resourceNamespace)
        {
            Name = name;
            Origin = origin;
            Namespace = resourceNamespace;
        }
    }
}
