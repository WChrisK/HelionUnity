using System;
using Helion.Resource.Decorate.Definitions.Properties;
using Helion.Util;
using Helion.Util.Extensions;
using UnityEngine;

namespace Helion.Worlds.Entities.Players
{
    public class Player
    {
        public static readonly UpperString DefinitionName = "DOOMPLAYER";

        public readonly int PlayerNumber;
        public readonly Entity Entity;
        public readonly Camera Camera;
        private readonly GameObject gameObject;

        public Player(int playerNumber, Entity entity)
        {
            PlayerNumber = playerNumber;
            Entity = entity;
            gameObject = new GameObject($"Player {playerNumber} camera");
            entity.GameObject.SetChild(gameObject, false);
            Camera = CreateCamera();

            SetGameObjectTransform();
        }

        public void Update(float tickFraction)
        {
            // Nothing yet.
        }

        private Camera CreateCamera()
        {
            Camera camera = gameObject.AddComponent<Camera>();

            ActorProperties props = Entity.Definition.Properties;
            camera.nearClipPlane = Math.Min(props.Radius, props.Height / 2).MapUnit();
            camera.farClipPlane = 512.0f;

            return camera;
        }

        private void SetGameObjectTransform()
        {
            Vector3 pos = gameObject.transform.position;
            pos.y += Entity.Definition.Properties.PlayerViewHeight.MapUnit();

            Quaternion rotate = Entity.Angle.Quaternion;

            gameObject.transform.SetPositionAndRotation(pos, rotate);
        }
    }
}
