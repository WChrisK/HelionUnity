using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Archives;
using Helion.Resource.Decorate.Definitions;
using Helion.Resource.Decorate.Definitions.States;
using Helion.Resource.Decorate.Parser;
using Helion.Resource.Textures.Sprites;
using Helion.Util;
using Helion.Util.Extensions;

namespace Helion.Resource.Decorate
{
    /// <summary>
    /// Manages all of the decorate actors. All decorate text files should pass
    /// through this object so it can collect and link all the definitions.
    /// </summary>
    public static class DecorateManager
    {
        private static readonly List<ActorDefinition> actors = new List<ActorDefinition>();
        private static readonly Dictionary<int, ActorDefinition> editorIDToDefinition = new Dictionary<int, ActorDefinition>();
        private static readonly Dictionary<UpperString, ActorDefinition> nameToDefinition = new Dictionary<UpperString, ActorDefinition>();
        private static readonly UpperString missingActorName = "UNKNOWN";
        private static ActorDefinition missingActorDefinition;

        internal static ActorDefinition BaseActorDefinition => actors.First();

        static DecorateManager()
        {
            ResetCoreDefinitions();
        }

        public static void Clear()
        {
            actors.Clear();
            editorIDToDefinition.Clear();
            nameToDefinition.Clear();

            ResetCoreDefinitions();
        }

        /// <summary>
        /// Handles a decorate entry, or throws if it cannot parse it.
        /// </summary>
        /// <param name="entry">The decorate base entry. This should have the
        /// name 'decorate' (case insensitive).</param>
        /// <param name="archive">The archive to look up definitions from.
        /// </param>
        /// <exception cref="Exception">If parsing fails.</exception>
        public static void HandleDefinitionsOrThrow(IEntry entry, IArchive archive)
        {
            DecorateParser parser = new DecorateParser(archive);

            bool success = parser.Parse(entry);
            if (!success)
                throw new Exception("Unable to parse decorate entries");

            AddDefinitions(parser.Definitions);
        }

        /// <summary>
        /// Looks up a definition by name.
        /// </summary>
        /// <param name="name">The name to look up.</param>
        /// <returns>The definition if it exists, or the 'missing' definition.
        /// </returns>
        public static ActorDefinition Find(UpperString name)
        {
            if (nameToDefinition.TryGetValue(name, out ActorDefinition definition))
                return definition;
            return missingActorDefinition;
        }

        /// <summary>
        /// Looks up a definition by editor ID.
        /// </summary>
        /// <param name="editorID">The ID to look up.</param>
        /// <returns>The definition if it exists, or the 'missing' definition.
        /// </returns>
        public static ActorDefinition Find(int editorID)
        {
            if (editorIDToDefinition.TryGetValue(editorID, out ActorDefinition definition))
                return definition;
            return missingActorDefinition;
        }

        internal static void AttachSpriteRotationsToFrames()
        {
            foreach (ActorDefinition definition in actors)
                foreach (ActorFrame frame in definition.States.Frames)
                    frame.SpriteRotations = SpriteManager.Rotations(frame.Sprite);
        }

        private static void ResetCoreDefinitions()
        {
            AddDefinitions(DefaultDefinitionFactory.CreateAllDefaultDefinitions());

            missingActorDefinition = nameToDefinition.Find(missingActorName).Value;
            if (missingActorDefinition == null)
                throw new NullReferenceException($"Core definition for unknown '{missingActorName}' missing");
        }

        private static void AddDefinitions(IEnumerable<ActorDefinition> definitions)
        {
            foreach (ActorDefinition definition in definitions)
            {
                actors.Add(definition);
                nameToDefinition[definition.Name] = definition;

                if (definition.EditorNumber != null)
                    editorIDToDefinition[definition.EditorNumber.Value] = definition;
            }
        }
    }
}
