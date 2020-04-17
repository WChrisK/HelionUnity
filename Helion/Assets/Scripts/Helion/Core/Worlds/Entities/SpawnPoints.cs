using System;
using System.Collections.Generic;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Core.Resource.Decorate.Definitions.Properties.Types;
using Helion.Core.Util;
using Helion.Core.Util.Logging;
using UnityEngine;

namespace Helion.Core.Worlds.Entities
{
    /// <summary>
    /// Maintains all of the spawn points for easy usage.
    /// </summary>
    public class SpawnPoints
    {
        private static readonly Log Log = LogManager.Instance();

        private readonly Dictionary<int, Vector2> coopSpawns = new Dictionary<int, Vector2>();
        private readonly List<Vector2> deathmatchSpawns = new List<Vector2>();
        private readonly Dictionary<UpperString, List<Vector2>> teamSpawns = new Dictionary<UpperString, List<Vector2>>();

        public void Add(Vector2 position, ActorDefinition definition)
        {
            // Ignore the default spawn point definition, it's just a template.
            if (definition.Name == "SPAWNPOINT")
                return;

            if (!definition.Properties.SpawnInfo)
            {
                Log.Error("Should not be missing spawn information on a spawnpoint definition (contact a developer)");
                return;
            }

            SpawnInfo spawnInfo = definition.Properties.SpawnInfo.Value;
            switch (spawnInfo.Mode)
            {
            case SpawnGameMode.Cooperative:
                coopSpawns[spawnInfo.Number] = position;
                break;
            case SpawnGameMode.Deathmatch:
                deathmatchSpawns.Add(position);
                break;
            case SpawnGameMode.Team:
                if (spawnInfo.Team)
                    GetOrCreateTeam(spawnInfo.Team.Value).Add(position);
                else
                    Log.Error("Should not be missing spawn information on a spawnpoint definition (contact a developer)");
                break;
            default:
                throw new Exception($"Unexpected spawn information type: {definition.Properties.SpawnInfo.Value.Mode}");
            }
        }

        private List<Vector2> GetOrCreateTeam(UpperString teamName)
        {
            if (teamSpawns.TryGetValue(teamName, out List<Vector2> teamList))
                return teamList;

            List<Vector2> newList = new List<Vector2>();
            teamSpawns[teamName] = newList;
            return newList;
        }

        /// <summary>
        /// Gets the cooperative spawn location for the player.
        /// </summary>
        /// <param name="playerNumber">The player number.</param>
        /// <returns>The location, or null if no spawn for the player number
        /// exists.</returns>
        public Vector2? Coop(int playerNumber)
        {
            if (coopSpawns.TryGetValue(playerNumber, out Vector2 pos))
                return pos;
            return null;
        }
    }
}
