using System;
using System.Collections.Generic;
using Helion.Resource.Decorate.Definitions;
using Helion.Util.Geometry;
using Helion.Util.Interpolation;
using Helion.Util.Unity;
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
        /// The bottom center position of the entity.
        /// </summary>
        public Vector3Interpolation Position;

        /// <summary>
        /// The angle the entity is facing.
        /// </summary>
        public BitAngle Angle;

        /// <summary>
        /// The velocity of the entity.
        /// </summary>
        public Vector3 Velocity;

        internal LinkedListNode<Entity> node;
        private readonly EntityManager entityManager;
        private readonly FrameTracker frameTracker;

        public World World => entityManager.world;

        public Entity(int id, ActorDefinition definition, Vector3 position, BitAngle angle,
            EntityManager manager)
        {
            ID = id;
            Definition = definition;
            Position = new Vector3Interpolation(position);
            Angle = angle;
            entityManager = manager;
            frameTracker = new FrameTracker(this);
            GameObject = CreateGameObject();
        }

        public void Update(float tickFraction)
        {
            GameObject.transform.position = Position.Value(tickFraction).MapUnit();
        }

        public void Tick()
        {
            Position.Tick();
            frameTracker.Tick();
        }

        public void Dispose()
        {
            entityManager.Entities.Remove(node);
        }

        private GameObject CreateGameObject()
        {
            GameObject gameObj = new GameObject($"Entity {Definition.Name} ({ID})");
            gameObj.transform.position = Position.Current.MapUnit();
            return gameObj;
        }
    }
}
