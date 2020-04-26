using Helion.Util;

namespace Helion.Resource.Decorate.Definitions.States
{
    public class ActorActionFunction
    {
        public readonly UpperString FunctionName;

        public ActorActionFunction(UpperString functionName)
        {
            FunctionName = functionName;
        }

        public ActorActionFunction(ActorActionFunction other)
        {
            FunctionName = other.FunctionName;
        }

        /// <summary>
        /// Calls the action function.
        /// </summary>
        public void Execute()
        {
            // TODO
        }

        public override string ToString() => FunctionName.String;
    }
}
