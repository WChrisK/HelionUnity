using Helion.Core.Resource.Colors;
using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Core.Util;
using Helion.Core.Util.Geometry;
using UnityEngine;
using static Helion.Core.Util.OptionalHelper;

namespace Helion.Core.Resource.Textures.Definitions
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
