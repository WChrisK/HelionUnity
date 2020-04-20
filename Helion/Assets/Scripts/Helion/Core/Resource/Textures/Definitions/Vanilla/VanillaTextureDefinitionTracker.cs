using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Util;
using Helion.Core.Util.Logging;
using UnityEngine;

namespace Helion.Core.Resource.Textures.Definitions.Vanilla
{
    /// <summary>
    /// Tracks pnames/textureX definitions to handle the weird
    /// </summary>
    public class VanillaTextureDefinitionTracker
    {
        private static readonly Log Log = LogManager.Instance();

        private Optional<PNames> lastPnames = Optional<PNames>.Empty();
        private Optional<TextureX> lastTexture1 = Optional<TextureX>.Empty();
        private Optional<TextureX> lastTexture2 = Optional<TextureX>.Empty();

        /// <summary>
        /// Tracks a pnames/textureX entry. Does nothing if it is not one of
        /// the vanilla entries.
        /// </summary>
        /// <param name="entry">The entry to track.</param>
        public void Track(IEntry entry)
        {
            switch (entry.Name.String)
            {
            case "PNAMES":
                Optional<PNames> pnames = PNames.From(entry.Data);
                if (pnames)
                    lastPnames = pnames.Value;
                else
                    Log.Error($"Error reading pnames at: {entry.Path}");
                break;
            case "TEXTURE1":
            case "TEXTURE2":
                Optional<TextureX> textureX = TextureX.From(entry);
                if (textureX)
                {
                    if (textureX.Value.TextureXNumber == 1)
                        lastTexture1 = textureX.Value;
                    else
                        lastTexture2 = textureX.Value;
                }
                else
                    Log.Error($"Error reading pnames at: {entry.Path}");
                break;
            default:
                Debug.Assert(false, $"Should not be processing entry {entry.Path} in the vanilla texture definitions tracker");
                break;
            }
        }

        /// <summary>
        /// Compiles definitions from the existing definition entries if they
        /// are available. Resets the state for the next archive to add new
        /// definitions via <see cref="Track"/>
        /// </summary>
        /// <returns>A list of compiled texture definitions, or an empty
        /// enumerable if there are no definitions to parse (or the parsed
        /// definitions were empty).</returns>
        public IEnumerable<TextureDefinition> CompileDefinitions()
        {
            List<TextureDefinition> definitions = new List<TextureDefinition>();

            if (lastPnames && (lastTexture1 || lastTexture2))
            {
                // These functions handle null option values fine.
                ParseAndAddDefinitions(lastPnames.Value, lastTexture1.Value, definitions);
                ParseAndAddDefinitions(lastPnames.Value, lastTexture2.Value, definitions);
            }

            lastTexture1 = Optional<TextureX>.Empty();
            lastTexture2 = Optional<TextureX>.Empty();
            return definitions;
        }

        private static void ParseAndAddDefinitions(PNames pnames, TextureX textureX, List<TextureDefinition> definitions)
        {
            if (pnames == null || textureX == null)
                return;

            foreach (TextureXImage image in textureX)
            {
                TextureDefinition definition = new TextureDefinition(image.Name, image.Dimension, ResourceNamespace.Textures);

                foreach (TextureXPatch patch in image.Patches)
                {
                    UpperString name = pnames[patch.PatchIndex];
                    TextureDefinitionPatch defPatch = new TextureDefinitionPatch(name, patch.Offset, ResourceNamespace.Textures);
                    definition.Patches.Add(defPatch);
                }

                definitions.Add(definition);
            }
        }
    }
}
