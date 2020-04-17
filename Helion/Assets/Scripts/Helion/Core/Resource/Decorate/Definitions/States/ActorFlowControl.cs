using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Resource.Decorate.Definitions.States
{
    /// <summary>
    /// Represents a state change that occurs after some frame.
    /// </summary>
    public class ActorFlowControl
    {
        public readonly ActorStateBranch FlowType;
        public readonly Optional<UpperString> Parent = Optional<UpperString>.Empty();
        public readonly Optional<UpperString> Label = Optional<UpperString>.Empty();
        public readonly int Offset;

        public ActorFlowControl() : this(ActorStateBranch.None)
        {
        }

        public ActorFlowControl(ActorStateBranch branchType)
        {
            Debug.Assert(branchType != ActorStateBranch.Goto && branchType != ActorStateBranch.Loop, $"Using wrong branch type constructor ({branchType} should be not be Goto or Loop)");

            FlowType = branchType;
        }

        public ActorFlowControl(ActorStateBranch branchType, UpperString label)
        {
            Debug.Assert(branchType == ActorStateBranch.Loop, $"Using wrong branch type constructor ({branchType} should be only Loop)");

            FlowType = branchType;
            Label = label;
        }

        public ActorFlowControl(ActorStateBranch branchType, Optional<UpperString> parent,
            Optional<UpperString> label, int offset)
        {
            FlowType = branchType;
            Label = label;
            Parent = parent;
            Offset = offset;
        }

        public ActorFlowControl(ActorFlowControl other)
        {
            FlowType = other.FlowType;
            Label = new Optional<UpperString>(other.Label.Value);
            Parent = new Optional<UpperString>(other.Parent.Value);
            Offset = other.Offset;
        }

        public override string ToString() => $"{FlowType} (parent={Parent} label={Label} offset={Offset})";
    }
}
