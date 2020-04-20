using Helion.Core.Util;

namespace Helion.Core.Resource.Textures.Definitions
{
    public static class TextureDefinitionManager
    {
        private static readonly ResourceTracker<TextureDefinition> textureDefinitions = new ResourceTracker<TextureDefinition>();
        //private static VanillaTextureTracker vanillaTextureTracker = new VanillaTextureTracker();

        public static bool TryGetExact(UpperString name, ResourceNamespace resourceNamespace,
            out TextureDefinition definition)
        {
            return textureDefinitions.TryGetValue(name, resourceNamespace, out definition);
        }

        public static bool TryGetAny(UpperString name, out TextureDefinition definition,
            out ResourceNamespace resourceNamespace)
        {
            return textureDefinitions.TryGetAnyValue(name, out definition, out resourceNamespace);
        }

        public static void Clear()
        {
            textureDefinitions.Clear();
        }
    }
}
