using System.Collections.Generic;
using Helion.Util;
using Helion.Util.Geometry;

namespace Helion.Resource.Textures.Definitions
{
    /// <summary>
    /// A definition of a texture that can be compiled into an image from other
    /// textures.
    /// </summary>
    public class TextureDefinition
    {
        public readonly UpperString Name;
        public readonly Dimension Dimension;
        public readonly ResourceNamespace Namespace;
        public float XScale = 1.0f;
        public float YScale = 1.0f;
        public Vec2I Offset;
        public bool WorldPanning;
        public bool NoDecals;
        public readonly List<TextureDefinitionPatch> Patches = new List<TextureDefinitionPatch>();

        public int Width => Dimension.Width;
        public int Height => Dimension.Height;

        public TextureDefinition(UpperString name, Dimension dimension, ResourceNamespace resourceNamespace)
        {
            Name = name;
            Dimension = dimension;
            Namespace = resourceNamespace;
        }
    }
}
