using System;
using Helion.Resource;
using Helion.Resource.Decorate.Definitions.Properties;
using Helion.Util;
using Helion.Util.Extensions;
using Helion.Util.Geometry;
using UnityEngine;
using static Helion.Util.Unity.UnityHelper;

namespace Helion.Worlds.Entities.Players
{
    public class Player
    {
        public static readonly UpperString DefinitionName = "DOOMPLAYER";

        public readonly int PlayerNumber;
        public readonly Entity Entity;
        public readonly Camera Camera;
        public readonly GameObject GameObject;
        private float pitchDegrees;
        private float yawDegrees;

        public Player(int playerNumber, Entity entity)
        {
            PlayerNumber = playerNumber;
            Entity = entity;
            GameObject = new GameObject($"Player {playerNumber} camera");
            entity.GameObject.SetChild(GameObject, false);
            Camera = CreateCamera();
            yawDegrees = DoomToUnityAngle(entity.Angle.Degrees);

            SetGameObjectTransform();
        }

        public void ApplyPlayerCameraInput()
        {
            float x;
            float y;
            if (Data.Config.Mouse.UseRawInput)
            {
                x = Input.GetAxisRaw("Mouse X");
                y = Input.GetAxisRaw("Mouse Y");
            }
            else
            {
                x = Input.GetAxis("Mouse X");
                y = Input.GetAxis("Mouse Y");
            }

            yawDegrees += x * Data.Config.Mouse.Yaw * Data.Config.Mouse.Sensitivity;
            pitchDegrees += y * Data.Config.Mouse.Pitch * Data.Config.Mouse.Sensitivity;

            yawDegrees %= 360.0f;
            pitchDegrees = pitchDegrees.Clamp(-90, 90);

            GameObject.transform.rotation = Quaternion.Euler(-pitchDegrees, yawDegrees, 0);

            Entity.Angle = BitAngle.FromDegrees(DoomToUnityAngle(yawDegrees));
        }

        public void Update(float tickFraction)
        {
            // Nothing yet.
        }

        private Camera CreateCamera()
        {
            Camera camera = GameObject.AddComponent<Camera>();

            ActorProperties props = Entity.Definition.Properties;
            camera.nearClipPlane = Math.Min(props.Radius, props.Height / 2).MapUnit();
            camera.farClipPlane = 512.0f;

            return camera;
        }

        private void SetGameObjectTransform()
        {
            Vector3 pos = GameObject.transform.position;
            pos.y += Entity.Definition.Properties.PlayerViewHeight.MapUnit();

            Quaternion rotate = Entity.Angle.Quaternion;

            GameObject.transform.SetPositionAndRotation(pos, rotate);
        }
    }
}
