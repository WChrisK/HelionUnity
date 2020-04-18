using System;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Enums
{
    [Flags]
    public enum ThingSpecialActivationType
    {
        Default = 0x000,
        ThingActs = 0x001,
        TriggerActs = 0x002,
        ThingTargets = 0x004,
        TriggerTargets = 0x008,
        MonsterTrigger = 0x010,
        MissileTrigger = 0x020,
        ClearSpecial = 0x040,
        NoDeathSpecial = 0x080,
        Activate = 0x100,
        Deactivate = 0x200,
        Switch = 0x400
    }
}
