namespace Helion.Core.Resource.Decorate.Parser
{
    public partial class DecorateParser
    {
        private void ConsumeActorStateElement()
        {
            MakeException("TODO");
        }

        private void ConsumeActorStates()
        {
            Consume('{');
            InvokeUntilAndConsume('}', ConsumeActorStateElement);
        }
    }
}
