using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.States
{
    public class ActorActionFunction
    {
        public readonly UpperString FunctionName;

        public ActorActionFunction(UpperString functionName)
        {
            FunctionName = functionName;
        }

        public override string ToString() => FunctionName.String;
    }
}
