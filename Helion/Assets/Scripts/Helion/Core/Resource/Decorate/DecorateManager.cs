using System;
using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Resource.Decorate.Parser;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Resource.Decorate
{
    public class DecorateManager
    {
        private readonly List<ActorDefinition> actors = new List<ActorDefinition>();
        private readonly Dictionary<UpperString, ActorDefinition> nameToDefinition = new Dictionary<UpperString, ActorDefinition>();

        public void HandleDefinitionsOrThrow(IEntry entry, IArchive archive)
        {
            DecorateParser parser = new DecorateParser(this, archive);

            bool success = parser.Parse(entry);
            if (!success)
                throw new Exception("Unable to parse decorate entries");

            actors.AddRange(parser.Definitions);
            parser.Definitions.ForEach(def => nameToDefinition[def.Name] = def);
        }

        internal Optional<ActorDefinition> LookupActor(UpperString name) => nameToDefinition.Find(name);
    }
}
