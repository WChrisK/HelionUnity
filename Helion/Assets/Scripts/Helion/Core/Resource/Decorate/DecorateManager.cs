using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Archives;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Resource.Decorate.Parser;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Resource.Decorate
{
    public class DecorateManager
    {
        private readonly List<ActorDefinition> actors;
        private readonly Dictionary<int, ActorDefinition> editorIDToDefinition = new Dictionary<int, ActorDefinition>();
        private readonly Dictionary<UpperString, ActorDefinition> nameToDefinition = new Dictionary<UpperString, ActorDefinition>();

        internal ActorDefinition BaseActorDefinition => actors.First();

        public DecorateManager()
        {
            actors = new List<ActorDefinition> { DefaultDefinitionFactory.CreateDefinition() };
        }

        public void HandleDefinitionsOrThrow(IEntry entry, IArchive archive)
        {
            DecorateParser parser = new DecorateParser(this, archive);

            bool success = parser.Parse(entry);
            if (!success)
                throw new Exception("Unable to parse decorate entries");

            foreach (ActorDefinition definition in parser.Definitions)
            {
                actors.Add(definition);
                nameToDefinition[definition.Name] = definition;

                if (definition.EditorNumber != null)
                    editorIDToDefinition[definition.EditorNumber.Value] = definition;
            }
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
    }
}
