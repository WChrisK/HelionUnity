using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.States
{
    /// <summary>
    /// A wrapper around the case where a label is immediately followed by some
    /// flow command. This comes up with inheritance and redirecting states or
    /// removing them.
    /// </summary>
    public class ActorFlowOverride
    {
        public ActorStateBranch BranchType;
        public Optional<UpperString> Parent;
        public Optional<UpperString> Label;
        public int? Offset;
    }
}
