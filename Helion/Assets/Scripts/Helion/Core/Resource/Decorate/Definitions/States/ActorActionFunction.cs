namespace Helion.Core.Resource.Decorate.Definitions.States
{
    public class ActorActionFunction
    {
        public readonly string FunctionName;

        public ActorActionFunction(string functionName)
        {
            FunctionName = functionName.ToUpper();
        }

        public override string ToString() => FunctionName;
    }
}
