using Helion.Archives;
using Helion.Resource.Textures.Definitions.Vanilla;
using Helion.Util;

namespace Helion.Resource.Textures.Definitions
{
    public static class TextureDefinitionManager
    {
        private static readonly ResourceTracker<TextureDefinition> textureDefinitions = new ResourceTracker<TextureDefinition>();
        private static VanillaTextureDefinitionTracker vanillaTextureTracker = new VanillaTextureDefinitionTracker();

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
            vanillaTextureTracker = new VanillaTextureDefinitionTracker();
        }

        internal static void TrackVanillaDefinition(IEntry entry)
        {
            vanillaTextureTracker.Track(entry);
        }

        internal static void CompileAnyNewVanillaDefinitions()
        {
            foreach (TextureDefinition definition in vanillaTextureTracker.CompileDefinitions())
                textureDefinitions.Add(definition.Name, definition.Namespace, definition);
        }
    }
}
