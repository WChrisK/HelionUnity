namespace Helion.Core.Resource.Decorate.Definitions.States
{
    public class ActorFlowControl
    {
        public readonly ActorStateBranch FlowType;
        public readonly string Label;
        public readonly string Parent;
        public readonly int Offset;

        public override string ToString() => $"{FlowType} (parent={Parent} label={Label} offset={Offset})";
    }
}
