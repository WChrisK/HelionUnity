using Helion.Resource.Decorate.Definitions.Flags;
using Helion.Resource.Decorate.Definitions.Properties;
using Helion.Resource.Decorate.Definitions.States;
using Helion.Resource.Decorate.Definitions.Types;
using Helion.Util;

namespace Helion.Resource.Decorate.Definitions
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
        public readonly ActorStates States;
        public readonly ActorTypes ActorType = new ActorTypes();

        public ActorDefinition(UpperString name, Optional<ActorDefinition> parent, int? editorNumber = null)
        {
            Name = name;
            Parent = parent;
            EditorNumber = editorNumber;
            States = new ActorStates(this);

            if (parent)
            {
                Flags = new ActorFlags(parent.Value.Flags);
                Properties = new ActorProperties(parent.Value.Properties);
                States = new ActorStates(this, parent.Value.States, parent.Value.Name);
                ActorType = new ActorTypes(parent.Value.ActorType);
            }
        }

        public override string ToString()
        {
            return EditorNumber != null ? $"Actor [{EditorNumber}]: {Name}" : $"Actor: {Name}";
        }
    }
}
