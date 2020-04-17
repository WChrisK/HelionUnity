using Helion.Core.Resource.Decorate.Definitions.Flags;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions
{
    public class ActorDefinition
    {
        public readonly UpperString Name;
        public readonly Optional<ActorDefinition> Parent;
        public readonly int? EditorNumber;
        public readonly ActorFlags Flags = new ActorFlags();
        public readonly ActorProperties Properties = new ActorProperties();
        public readonly ActorStates States = new ActorStates();

        public ActorDefinition(UpperString name, Optional<ActorDefinition> parent, int? editorNumber = null)
        {
            Name = name;
            Parent = parent;
            EditorNumber = editorNumber;
        }
    }
}
