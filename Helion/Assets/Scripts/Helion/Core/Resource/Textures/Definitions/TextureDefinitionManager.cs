using Helion.Core.Archives;
using Helion.Core.Resource.Textures.Definitions.Vanilla;
using Helion.Core.Util;
using Helion.Core.Util.Logging;

namespace Helion.Core.Resource.Textures.Definitions
{
    public static class TextureDefinitionManager
    {
        private static readonly Log Log = LogManager.Instance();

        private static readonly ResourceTracker<TextureDefinition> textureDefinitions = new ResourceTracker<TextureDefinition>();
        private static VanillaTextureTracker vanillaTextureTracker = new VanillaTextureTracker();

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
            vanillaTextureTracker = new VanillaTextureTracker();
        }

        internal static void TrackPNames(IEntry entry, IArchive archive)
        {
            Optional<PNames> pnames = PNames.From(entry.Data);
            if (pnames)
                vanillaTextureTracker.Track(pnames.Value, archive);
            else
                Log.Error($"Unable to read PNames entry: {entry.Path}");
        }

        internal static void TrackTextureX(IEntry entry, IArchive archive)
        {
            Optional<TextureX> textureX = TextureX.From(entry);
            if (textureX)
                vanillaTextureTracker.Track(textureX.Value, archive);
            else
                Log.Error($"Unable to read TextureX entry: {entry.Path}");
        }

        // TODO: We should do this after each archive iteration.
        internal static void CompileAnyNewVanillaDefinitions()
        {
        }
    }
}
