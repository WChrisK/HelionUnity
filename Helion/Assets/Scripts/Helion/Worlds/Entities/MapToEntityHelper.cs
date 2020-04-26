using System;
using Helion.Resource.Maps;
using Helion.Resource.Maps.Components;
using Helion.Util.Geometry;
using Helion.Worlds.Info;
using UnityEngine;

namespace Helion.Worlds.Entities
{
    /// <summary>
    /// A helper class for spawning entities.
    /// </summary>
    public static class MapToEntityHelper
    {
        /// <summary>
        /// Spawns all the entities from the map data into the map with the
        /// entity manager provided.
        /// </summary>
        /// <param name="entityManager">The manager to spawn them from.</param>
        /// <param name="map">The map data to read.</param>
        public static void SpawnEntities(EntityManager entityManager, MapData map)
        {
            WorldInfo info = entityManager.world.Info;

            foreach (MapThing thing in map.Things)
            {
                if (DoNotSpawnFromInfo(thing, info))
                    continue;

                BitAngle angle = BitAngle.FromDegrees(thing.AngleDegrees);
                entityManager.Spawn(thing.EditorID, thing.Position, angle);
            }
        }

        private static bool DoNotSpawnFromInfo(MapThing thing, WorldInfo info)
        {
            if (thing.InSinglePlayer && !info.SinglePlayer)
                return true;

            if (info.MultiPlayer && info.Mode == GameMode.Cooperative && !thing.InCooperative)
                return true;

            if (info.Mode == GameMode.Deathmatch && !thing.InDeathmatch)
                return true;

            switch (info.Skill)
            {
            case Skill.VeryEasy:
                if (!thing.Skill1)
                    return true;
                break;
            case Skill.Easy:
                if (!thing.Skill2)
                    return true;
                break;
            case Skill.Medium:
                if (!thing.Skill3)
                    return true;
                break;
            case Skill.Hard:
                if (!thing.Skill4)
                    return true;
                break;
            case Skill.Nightmare:
                if (!thing.Skill5)
                    return true;
                break;
            default:
                throw new Exception($"Unexpected skill type when spawning thing: {info.Skill}");
            }

            return false;
        }
    }
}
