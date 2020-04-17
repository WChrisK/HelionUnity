using Helion.Core.Resource.Decorate.Definitions.Flags;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions
{
    /// <summary>
    /// The definition of an actor for use in a game world.
    /// </summary>
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

            if (parent)
            {
                Flags = new ActorFlags(parent.Value.Flags);
                Properties = new ActorProperties(parent.Value.Properties);
                States = new ActorStates(parent.Value.States, parent.Value.Name);
            }
        }
    }
}
