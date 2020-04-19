using System;
using System.Collections;

namespace Helion.Core.Resource.Decorate.Definitions.Types
{
    /// <summary>
    /// A lightweight collection of heuristic information to avoid recursive or
    /// expensive lookups of the inheritance type for very common definitions.
    /// </summary>
    public class ActorTypes
    {
        public static readonly int ActorTypeCount = Enum.GetNames(typeof(ActorType)).Length;

        private readonly BitArray bits;

        public bool Ammo => bits[(int)ActorType.Ammo];
        public bool Inventory => bits[(int)ActorType.Inventory];
        public bool Key => bits[(int)ActorType.Key];
        public bool Player => bits[(int)ActorType.Player];
        public bool Powerup => bits[(int)ActorType.Powerup];
        public bool SpawnPoint => bits[(int)ActorType.SpawnPoint];
        public bool TeleportDestination => bits[(int)ActorType.TeleportDestination];
        public bool Weapon => bits[(int)ActorType.Weapon];

        public ActorTypes()
        {
            bits = new BitArray(ActorTypeCount);
        }

        public ActorTypes(ActorTypes actorTypes)
        {
            bits = new BitArray(actorTypes.bits);
        }

        public void Set(ActorType actorType)
        {
            bits.Set((int)actorType, true);
        }
    }
}
