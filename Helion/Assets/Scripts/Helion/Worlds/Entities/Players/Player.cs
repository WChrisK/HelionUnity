using Helion.Util;

namespace Helion.Worlds.Entities.Players
{
    public class Player
    {
        public static readonly UpperString DefinitionName = "DOOMPLAYER";

        public readonly int PlayerNumber;
        public readonly Entity Entity;

        public Player(int playerNumber, Entity entity)
        {
            PlayerNumber = playerNumber;
            Entity = entity;
        }
    }
}
