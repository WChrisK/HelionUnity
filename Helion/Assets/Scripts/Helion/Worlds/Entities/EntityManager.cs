using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helion.Resource.Decorate;
using Helion.Resource.Decorate.Definitions;
using Helion.Resource.Maps;
using Helion.Util;
using Helion.Util.Geometry;
using Helion.Worlds.Entities.Players;
using MoreLinq;
using UnityEngine;

namespace Helion.Worlds.Entities
{
    public class EntityManager : IEnumerable<Entity>, ITickable, IDisposable
    {
        private readonly World world;
        private readonly LinkedList<Entity> Entities = new LinkedList<Entity>();
        private readonly Dictionary<int, Player> Players = new Dictionary<int, Player>();
        private int nextEntityID;

        public EntityManager(World world, MapData map)
        {
            this.world = world;
        }

        public Entity Spawn(UpperString type, Vector2 position, BitAngle angle = default)
        {
            float height = world.Geometry.BspTree.Sector(position).Floor.Height;
            return Spawn(type, new Vector3(position.x, height, position.y), angle);
        }

        public Entity Spawn(UpperString type, Vector3 position, BitAngle angle = default)
        {
            ActorDefinition definition = DecorateManager.Find(type);
            Entity entity = new Entity(nextEntityID++, definition, position, angle);
            Entities.AddLast(entity);

            return entity;
        }

        public bool TryGetPlayer(int playerNumber, out Player player)
        {
            return Players.TryGetValue(playerNumber, out player);
        }

        public void Tick()
        {
            // TODO
        }

        public void Dispose()
        {
            // Since logic may unhook it from the list we're iterating over, we
            // have to clone the entire list.
            Entities.ToList().ForEach(entity => entity.Dispose());
        }

        public IEnumerator<Entity> GetEnumerator() => Entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
