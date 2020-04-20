using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Archives;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Resource.Decorate.Definitions.States;
using Helion.Core.Resource.Decorate.Parser;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Resource.Decorate
{
    /// <summary>
    /// Manages all of the decorate actors. All decorate text files should pass
    /// through this object so it can collect and link all the definitions.
    /// </summary>
    public class DecorateManager
    {
        private readonly List<ActorDefinition> actors = new List<ActorDefinition>();
        private readonly Dictionary<int, ActorDefinition> editorIDToDefinition = new Dictionary<int, ActorDefinition>();
        private readonly Dictionary<UpperString, ActorDefinition> nameToDefinition = new Dictionary<UpperString, ActorDefinition>();

        internal ActorDefinition BaseActorDefinition => actors.First();

        public DecorateManager()
        {
            AddDefinitions(DefaultDefinitionFactory.CreateAllDefaultDefinitions());
        }

        /// <summary>
        /// Handles a decorate entry, or throws if it cannot parse it.
        /// </summary>
        /// <param name="entry">The decorate base entry. This should have the
        /// name 'decorate' (case insensitive).</param>
        /// <param name="archive">The archive to look up definitions from.
        /// </param>
        /// <exception cref="Exception">If parsing fails.</exception>
        public void HandleDefinitionsOrThrow(IEntry entry, IArchive archive)
        {
            DecorateParser parser = new DecorateParser(this, archive);

            bool success = parser.Parse(entry);
            if (!success)
                throw new Exception("Unable to parse decorate entries");

            AddDefinitions(parser.Definitions);
        }

        /// <summary>
        /// Looks up a definition by name.
        /// </summary>
        /// <param name="name">The name to look up.</param>
        /// <returns>The definition if it exists, or an empty value.</returns>
        public Optional<ActorDefinition> Find(UpperString name) => nameToDefinition.Find(name);

        /// <summary>
        /// Looks up a definition by editor ID.
        /// </summary>
        /// <param name="editorID">The ID to look up.</param>
        /// <returns>The definition if it exists, or an empty value.</returns>
        public Optional<ActorDefinition> Find(int editorID) => editorIDToDefinition.Find(editorID);

        internal void AttachSpriteRotationsToFrames()
        {
            foreach (ActorDefinition definition in actors)
                foreach (ActorFrame frame in definition.States.Frames)
                    frame.SpriteRotations = Data.Sprites.Rotations(frame.Sprite);
        }

        private void AddDefinitions(IEnumerable<ActorDefinition> definitions)
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
