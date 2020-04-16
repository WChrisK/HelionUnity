using System.Collections.Generic;
using Helion.Core.Resource.Decorate.Definitions.States;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions
{
    public class ActorStates
    {
        public readonly Dictionary<UpperString, int> Labels = new Dictionary<UpperString, int>();
        public readonly Dictionary<UpperString, ActorFlowOverride> FlowOverrides = new Dictionary<UpperString, ActorFlowOverride>();
        public readonly List<ActorFrame> Frames = new List<ActorFrame>();
    }
}
