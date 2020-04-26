using Helion.Util;

namespace Helion.Worlds.Info
{
    /// <summary>
    /// Information about a world.
    /// </summary>
    public class WorldInfo
    {
        public readonly UpperString LevelName;
        public readonly int LevelNumber;
        public Skill Skill = Skill.VeryEasy;
        public GameMode Mode = GameMode.Cooperative;
        public bool MultiPlayer = false;

        public bool SinglePlayer => !MultiPlayer;

        public WorldInfo(UpperString name, int number)
        {
            LevelName = name;
            LevelNumber = number;
        }
    }
}
