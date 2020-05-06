using System;
using System.Collections.Generic;
using Helion.Resource.Decorate.Definitions;
using Helion.Resource.Decorate.Definitions.Flags;
using Helion.Resource.Decorate.Definitions.Properties;
using Helion.Resource.Decorate.Definitions.States;
using Helion.Unity;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Boxes;
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
        public Vec3Interpolation Position { get; private set; }

        /// <summary>
        /// The angle the entity is facing.
        /// </summary>
        public BitAngle Angle;

        /// <summary>
        /// The velocity of the entity.
        /// </summary>
        public Vec3F Velocity;

        /// <summary>
        /// The bounding box around the entity from a birds eye view. This box
        /// will only be at the current position, there is no interpolation.
        /// </summary>
        public Box3F Box { get; private set; }

        /// <summary>
        /// The sector the center of the entity is in.
        /// </summary>
        public Sector Sector { get; internal set; }

        internal LinkedListNode<Entity> node;
        private readonly EntityManager entityManager;
        private readonly FrameTracker frameTracker;
        private readonly EntityMeshComponents meshComponents;
        private readonly BoxCollider collider;
        private readonly CollisionInfo collisionInfo;

        public World World => entityManager.world;
        public ActorFrame Frame => frameTracker.Frame;
        public ActorFlags Flags => Definition.Flags;
        public ActorProperties Properties => Definition.Properties;
        public float Radius => Definition.Properties.Radius;
        public float Height => Definition.Properties.Height;
        public float HalfHeight => Height * 0.5f;

        public Entity(int id, ActorDefinition definition, Vec3F position, BitAngle angle,
            Sector sector, EntityManager manager)
        {
            ID = id;
            Definition = definition;
            Position = new Vec3Interpolation(position);
            Angle = angle;
            Box = CreateBoxAt(position);
            Sector = sector;
            entityManager = manager;
            frameTracker = new FrameTracker(this);
            GameObject = CreateGameObject();
            meshComponents = new EntityMeshComponents(this, GameObject);
            collider = CreateCollider();
            collisionInfo = CollisionInfo.CreateOn(GameObject, this);
        }

        public void Update(float tickFraction)
        {
            GameObject.transform.position = Position.Value(tickFraction).MapUnit();

            meshComponents.Update(tickFraction);
        }

        /// <summary>
        /// Sets the world position. This should be the world unit position
        /// (not the scaled map unit position).
        /// </summary>
        /// <param name="position">The world position.</param>
        public void SetPosition(Vec3F position)
        {
            Position = Position.At(position);
            Box = CreateBoxAt(position);
            GameObject.transform.position = position.MapUnit();

            Sector = World.Geometry.BspTree.Sector(position);
        }

        /// <summary>
        /// Checks if the provided entity blocks this one. This is primarily
        /// intended to see if a collision between entities should be blocking
        /// or if the mover can walk into it (and possibly even pick it up).
        /// </summary>
        /// <param name="other">The other entity to check against.</param>
        /// <returns>True if the other blocks it, false if not.</returns>
        public bool IsBlockedBy(Entity other)
        {
            return other.Flags.Solid;
        }

        public void Tick()
        {
            Position = Position.Tick();
            Sector = World.Geometry.BspTree.Sector(Position.Current);

            frameTracker.Tick();

            World.Physics.TryMove(this);
        }

        public void Dispose()
        {
            meshComponents.Dispose();
            entityManager.Entities.Remove(node);
            GameObjectHelper.Destroy(collisionInfo);
            GameObjectHelper.Destroy(collider);
            GameObjectHelper.Destroy(GameObject);
        }

        private Box3F CreateBoxAt(in Vec3F center)
        {
            float radius = Radius;
            Vec3F radiusVector = new Vec3F(radius, 0, radius);
            Vec3F radiusHeightVector = radiusVector.WithY(Height);
            return new Box3F(center - radiusVector, center + radiusHeightVector);
        }

        private GameObject CreateGameObject()
        {
            GameObject gameObj = new GameObject($"Entity {Definition.Name} ({ID})");
            gameObj.transform.position = Position.Current.MapUnit();
            return gameObj;
        }

        private BoxCollider CreateCollider()
        {
            float diameter = Radius * 2;

            BoxCollider boxCollider = GameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, Height / 2, 0).MapUnit();
            boxCollider.size = new Vector3(diameter, Height, diameter).MapUnit();

            return boxCollider;
        }
    }
}
