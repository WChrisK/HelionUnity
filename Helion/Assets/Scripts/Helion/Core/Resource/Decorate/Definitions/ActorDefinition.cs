using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions
{
    public class ActorDefinition
    {
        public readonly UpperString Name;
        public readonly Optional<UpperString> Parent;
        public readonly Optional<UpperString> Replaces;
        public readonly int? EditorNumber;
        public readonly ActorFlags Flags = new ActorFlags();
        public readonly ActorProperties Properties = new ActorProperties();
        public readonly ActorStates States = new ActorStates();

        public ActorDefinition(UpperString name, UpperString parent = null, UpperString replaces = null,
            int? editorNumber = null)
        {
            Name = name;
            Parent = new Optional<UpperString>(parent);
            Replaces = new Optional<UpperString>(replaces);
            EditorNumber = editorNumber;
        }
    }
}
