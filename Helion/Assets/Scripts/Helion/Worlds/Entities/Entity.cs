using System;
using System.Collections.Generic;
using Helion.Resource.Decorate.Definitions;
using Helion.Util.Geometry;
using Helion.Util.Interpolation;
using Helion.Util.Unity;
using UnityEngine;

namespace Helion.Worlds.Entities
{
    public class Entity : ITickable, IDisposable
    {
        public readonly int ID;
        public readonly ActorDefinition Definition;
        public Vector3Interpolation Position;
        public BitAngle Angle;
        public Vector3 Velocity;
        internal LinkedListNode<Entity> node;
        private readonly EntityManager entityManager;
        private readonly FrameTracker frameTracker;
        private readonly GameObject gameObject;

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
            gameObject = CreateGameObject();
        }

        public void Tick()
        {
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
