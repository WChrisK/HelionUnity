using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Graphics;
using Helion.Core.Resource.Decorate;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Resource.Decorate.Definitions.States;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Logging;
using Helion.Core.Util.Unity;
using Helion.Core.Worlds.Geometry;
using UnityEngine;
using static Helion.Core.Util.OptionalHelper;

namespace Helion.Core.Worlds.Entities
{
    /// <summary>
    /// Manages all of the entities in a world.
    /// </summary>
    public class EntityManager : IEnumerable<Entity>, IDisposable
    {
        private static readonly Log Log = LogManager.Instance();

        public readonly SpawnPoints SpawnPoints = new SpawnPoints();
        private readonly GameObject entityCollectorGameObject;
        private readonly World world;
        private readonly MapGeometry geometry;
        private readonly LinkedList<Entity> entities = new LinkedList<Entity>();
        private readonly Dictionary<int, Player> players = new Dictionary<int, Player>();
        private int nextAvailableID;

        public EntityManager(World owningWorld, GameObject parentObject, MapGeometry mapGeometry, IMap map)
        {
            world = owningWorld;
            geometry = mapGeometry;

            entityCollectorGameObject = new GameObject("Entities");
            parentObject.SetChild(entityCollectorGameObject);

            CreateEntities(((DoomMap)map).Things);
        }

        public Optional<Entity> SpawnPlayer(int playerNumber)
        {
            if (players.ContainsKey(playerNumber))
            {
                Log.Warn("Trying to create multiple players for player ", playerNumber);
                return Empty;
            }

            Optional<ActorDefinition> actorDefinition = DecorateManager.Find("DOOMPLAYER");
            if (!actorDefinition)
                return Empty;

            Vector2? position = SpawnPoints.Coop(playerNumber);
            if (position == null)
                return Empty;

            Entity entity = CreateEntity(actorDefinition.Value, position.Value);

            Player player = entity.gameObject.AddComponent<Player>();
            player.PlayerNumber = playerNumber;
            player.Attach(entity);
            players[playerNumber] = player;

            return entity;
        }

        public void Dispose()
        {
            // Have to clone the list because the entity will unlink itself,
            // and doing that during iteration is probably really bad.
            entities.ToList().ForEach(entity => entity.Dispose());
        }

        public IEnumerator<Entity> GetEnumerator() => entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void CreateEntities(IEnumerable<DoomThing> things)
        {
            foreach (DoomThing thing in things)
            {
                Optional<ActorDefinition> actorDefinition = DecorateManager.Find(thing.EditorNumber);
                if (!actorDefinition)
                {
                    Log.Warn("Unknown entity type: ", thing.EditorNumber);
                    continue;
                }

                ActorDefinition definition = actorDefinition.Value;
                if (definition.ActorType.SpawnPoint)
                {
                    SpawnPoints.Add(thing.Position, definition);
                    continue;
                }

                CreateEntity(definition, thing.Position);
            }
        }

        private Entity CreateEntity(ActorDefinition definition, Vector2 position)
        {
            GameObject entityObject = new GameObject($"{definition.Name} ({nextAvailableID})");
            entityCollectorGameObject.SetChild(entityObject);

            // Note: The definition should be set immediately on the entity
            // as other code relies on it. We should consider restructuring
            // this so the dependency is not required.
            Entity entity = entityObject.AddComponent<Entity>();
            entity.ID = nextAvailableID++;
            entity.Definition = definition;
            entity.world = world;
            entity.frameTracker = new FrameTracker(entity);
            entity.entityNode = entities.AddLast(entity);

            float height = entity.Definition.Properties.Height;
            Sector sector = geometry.BspTree.Sector(position);
            float y = sector.FloorPlane.Height;
            Vector3 worldPos = new Vector3(position.x, y, position.y);
            entity.transform.position = worldPos.MapUnit();
            entity.Position = worldPos;
            entity.PrevPosition = worldPos;

            if (entity.Definition.Flags.Solid)
            {
                float diameter = entity.Definition.Properties.Radius * 2;
                BoxCollider collider = entityObject.AddComponent<BoxCollider>();
                collider.center = new Vector3(0, height / 2, 0).MapUnit();
                collider.size = new Vector3(diameter, height, diameter).MapUnit();
            }

            CreateSpriteBillboardMesh(entity, sector);

            return entity;
        }

        private void CreateSpriteBillboardMesh(Entity entity, Sector sector)
        {
            ActorFrame frame = entity.frameTracker.Frame;
            Material material = frame.SpriteRotations[0];
            Texture texture = material.mainTexture;

            MeshRenderer meshRenderer = entity.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;

            MeshFilter meshFilter = entity.gameObject.AddComponent<MeshFilter>();

            // 2--3
            // |  |
            // 0--1
            float radius = texture.width / 2.0f;
            Color brightness = ColorHelper.FromRGB(sector.LightLevel, sector.LightLevel, sector.LightLevel);
            Vector3[] vertices =
            {
                new Vector3(-radius, 0, 0).MapUnit(),
                new Vector3(radius, 0, 0).MapUnit(),
                new Vector3(-radius, texture.height, 0).MapUnit(),
                new Vector3(radius, texture.height, 0).MapUnit()
            };
            Vector2[] uvCoords =
            {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 0)
            };
            Vector3[] normals = { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
            Color[] colors = { brightness, brightness, brightness, brightness };
            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = new[] { 0, 2, 1, 1, 2, 3 },
                normals = normals,
                uv = uvCoords,
                colors = colors
            };
            meshFilter.sharedMesh = mesh;
        }
    }
}
