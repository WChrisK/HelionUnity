using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Resource.Decorate
{
    public class DecorateManager
    {
        private readonly Dictionary<UpperString, ActorDefinition> nameToDefinition = new Dictionary<UpperString, ActorDefinition>();

        public void HandleDefinitionsOrThrow(IEntry entry, IArchive archive)
        {
            // TODO
        }

        internal Optional<ActorDefinition> LookupActor(UpperString name) => nameToDefinition.Find(name);
    }
}
