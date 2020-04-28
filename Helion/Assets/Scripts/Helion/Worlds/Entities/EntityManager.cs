using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helion.Resource.Decorate;
using Helion.Resource.Decorate.Definitions;
using Helion.Resource.Maps;
using Helion.Util;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;
using Helion.Worlds.Entities.Players;
using Helion.Worlds.Entities.Spawns;
using Helion.Worlds.Geometry;
using MoreLinq;

namespace Helion.Worlds.Entities
{
    public class EntityManager : IEnumerable<Entity>, ITickable, IDisposable
    {
        internal readonly World world;
        internal readonly LinkedList<Entity> Entities = new LinkedList<Entity>();
        private readonly Dictionary<int, Player> players = new Dictionary<int, Player>();
        private readonly SpawnPoints spawnPoints = new SpawnPoints();
        private int nextEntityID;

        public EntityManager(World world, MapData map)
        {
            this.world = world;

            MapToEntityHelper.SpawnEntities(this, map);
        }

        public Entity Spawn(UpperString type, Vec2F position, BitAngle angle = default)
        {
            float height = world.Geometry.BspTree.Sector(position).Floor.Height;
            return Spawn(type, new Vec3F(position.X, height, position.Y), angle);
        }

        public Entity Spawn(int editorID, Vec2F position, BitAngle angle = default)
        {
            float height = world.Geometry.BspTree.Sector(position).Floor.Height;
            return Spawn(editorID, new Vec3F(position.X, height, position.Y), angle);
        }

        public Entity Spawn(UpperString type, Vec3F position, BitAngle angle = default)
        {
            ActorDefinition definition = DecorateManager.Find(type);
            return Spawn(definition, position, angle);
        }

        public Entity Spawn(int editorID, Vec3F position, BitAngle angle = default)
        {
            ActorDefinition definition = DecorateManager.Find(editorID);
            return Spawn(definition, position, angle);
        }

        public Player SpawnPlayer(int playerNumber)
        {
            if (players.ContainsKey(playerNumber))
                throw new Exception($"Trying to spawn {playerNumber} twice (also: voodoo dolls not supported yet)");

            if (!spawnPoints.TryGetCoopSpawn(playerNumber, out Entity spawn))
                throw new Exception($"Cannot find coop spawn for player {playerNumber}");

            Entity entity = Spawn(Player.DefinitionName, spawn.Position.Current, spawn.Angle);
            Player player = new Player(playerNumber, entity);
            players[playerNumber] = player;

            return player;
        }

        public bool TryGetPlayer(int playerNumber, out Player player)
        {
            return players.TryGetValue(playerNumber, out player);
        }

        public void Tick()
        {
            // We want to
            players.Values.ForEach(player => player.Tick());

            Entities.ForEach(entity => entity.Tick());
        }

        public void Dispose()
        {
            players.Values.ForEach(player => player.Dispose());

            // Since logic may unhook it from the list we're iterating over, we
            // have to clone the entire list.
            Entities.ToList().ForEach(entity => entity.Dispose());
        }

        public IEnumerator<Entity> GetEnumerator() => Entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Entity Spawn(ActorDefinition definition, Vec3F position, BitAngle angle)
        {
            Sector sector = world.Geometry.BspTree.Sector(position);
            Entity entity = new Entity(nextEntityID++, definition, position, angle, sector, this);
            entity.node = Entities.AddLast(entity);

            if (entity.Definition.ActorType.SpawnPoint)
                spawnPoints.Add(entity);

            return entity;
        }

        public void Update(float tickFraction)
        {
            Entities.ForEach(entity => entity.Update(tickFraction));
            players.Values.ForEach(player => player.Update(tickFraction));
        }
    }
}
