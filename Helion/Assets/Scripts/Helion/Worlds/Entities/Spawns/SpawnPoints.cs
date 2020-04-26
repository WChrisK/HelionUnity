using System;
using System.Collections.Generic;
using Helion.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Resource.Decorate.Definitions.Properties.Types;
using Helion.Util;
using Helion.Util.Logging;
using Helion.Worlds.Info;
using UnityEngine;

namespace Helion.Worlds.Entities.Spawns
{
/// <summary>
    /// Maintains all of the spawn points for easy usage.
    /// </summary>
    public class SpawnPoints
    {
        private static readonly Log Log = LogManager.Instance();

        private readonly Dictionary<int, Entity> coopSpawns = new Dictionary<int, Entity>();
        private readonly List<Entity> deathmatchSpawns = new List<Entity>();
        private readonly Dictionary<UpperString, List<Entity>> teamSpawns = new Dictionary<UpperString, List<Entity>>();

        public void Add(Entity entity)
        {
            Debug.Assert(entity.Definition.ActorType.SpawnPoint, "Trying to add a non-spawn point to the spawn points");

            SpawnInfo spawnInfo = entity.Definition.Properties.SpawnInfo.Value;
            switch (spawnInfo.Mode)
            {
            case GameMode.Cooperative:
                coopSpawns[spawnInfo.Number] = entity;
                break;
            case GameMode.Deathmatch:
                deathmatchSpawns.Add(entity);
                break;
            case GameMode.Team:
                if (spawnInfo.Team)
                    GetOrCreateTeam(spawnInfo.Team.Value).Add(entity);
                else
                    Log.Error("Should not be missing spawn information on a spawnpoint definition (contact a developer)");
                break;
            default:
                throw new Exception($"Unexpected spawn information type: {entity.Definition.Properties.SpawnInfo.Value.Mode}");
            }
        }

        private List<Entity> GetOrCreateTeam(UpperString teamName)
        {
            if (teamSpawns.TryGetValue(teamName, out List<Entity> teamList))
                return teamList;

            List<Entity> newList = new List<Entity>();
            teamSpawns[teamName] = newList;
            return newList;
        }

        /// <summary>
        /// Tries to get the cooperative player spawn.
        /// </summary>
        /// <param name="playerNumber">The player number.</param>
        /// <param name="entity">The spawn if found.</param>
        /// <returns>True if the coop spawn was found, false if not.</returns>
        public bool TryGetCoopSpawn(int playerNumber, out Entity entity)
        {
            return coopSpawns.TryGetValue(playerNumber, out entity);
        }
    }
}
