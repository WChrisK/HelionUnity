using System;
using System.Collections.Generic;
using Helion.Resource.Decorate.Definitions;
using Helion.Resource.Decorate.Definitions.States;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;
using Helion.Util.Interpolation;
using Helion.Util.Unity;
using Helion.Worlds.Geometry;
using UnityEngine;

namespace Helion.Worlds.Entities
{
    /// <summary>
    /// An actor in a world.
    /// </summary>
    public class Entity : ITickable, IDisposable
    {
        /// <summary>
        /// The unique ID of this entity.
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// The definition (or actor class) for this entity.
        /// </summary>
        public readonly ActorDefinition Definition;

        /// <summary>
        /// The game object that unity uses. The position of the transform will
        /// be the interpolated position at the bottom center of the body.
        /// </summary>
        public readonly GameObject GameObject;

        /// <summary>
        /// The bottom center position of the entity. Note that this uses the
        /// Unity coordinate system (so Y is up/down).
        /// </summary>
        public Vec3Interpolation Position;

        /// <summary>
        /// The angle the entity is facing.
        /// </summary>
        public BitAngle Angle;

        /// <summary>
        /// The velocity of the entity.
        /// </summary>
        public Vec3F Velocity;

        /// <summary>
        /// The sector the center of the entity is in.
        /// </summary>
        public Sector Sector { get; internal set; }

        internal LinkedListNode<Entity> node;
        private readonly EntityManager entityManager;
        private readonly FrameTracker frameTracker;
        private readonly EntityMeshComponents meshComponents;
        private readonly BoxCollider collider;

        public World World => entityManager.world;
        public ActorFrame Frame => frameTracker.Frame;

        public Entity(int id, ActorDefinition definition, Vec3F position, BitAngle angle,
            Sector sector, EntityManager manager)
        {
            ID = id;
            Definition = definition;
            Position = new Vec3Interpolation(position);
            Angle = angle;
            Sector = sector;
            entityManager = manager;
            frameTracker = new FrameTracker(this);
            GameObject = CreateGameObject();
            meshComponents = new EntityMeshComponents(this, GameObject);
            collider = CreateCollider();
        }

        public void Update(float tickFraction)
        {
            GameObject.transform.position = Position.Value(tickFraction).MapUnit();

            meshComponents.Update(tickFraction);
        }

        public void Tick()
        {
            Position.Tick();
            Sector = World.Geometry.BspTree.Sector(Position.Current);

            frameTracker.Tick();

            World.Physics.TryMove(this);
        }

        public void Dispose()
        {
            meshComponents.Dispose();
            entityManager.Entities.Remove(node);
            GameObjectHelper.Destroy(collider);
            GameObjectHelper.Destroy(GameObject);
        }

        private GameObject CreateGameObject()
        {
            GameObject gameObj = new GameObject($"Entity {Definition.Name} ({ID})");
            gameObj.transform.position = Position.Current.MapUnit();
            return gameObj;
        }

        private BoxCollider CreateCollider()
        {
            float radius = Definition.Properties.Radius;
            float diameter = radius * 2;
            float height = Definition.Properties.Height;

            BoxCollider boxCollider = GameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, height / 2, 0).MapUnit();
            boxCollider.size = new Vector3(diameter, height, diameter).MapUnit();

            return boxCollider;
        }
    }
}
