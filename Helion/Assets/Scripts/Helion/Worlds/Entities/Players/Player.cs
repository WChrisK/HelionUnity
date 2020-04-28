using System;
using Helion.Resource;
using Helion.Resource.Decorate.Definitions.Properties;
using Helion.Util;
using Helion.Util.Extensions;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;
using Helion.Util.Unity;
using Helion.Worlds.Geometry;
using UnityEngine;
using static Helion.Util.Unity.UnityHelper;

namespace Helion.Worlds.Entities.Players
{
    public class Player : ITickable, IDisposable
    {
        private const float ForwardMovementSpeed = 1.5625f;
        private const float SideMovementSpeed = 1.25f;
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
            yawDegrees = DoomUnityAngleConverter(entity.Angle.Degrees);

            SetGameObjectTransform();
        }

        public void ApplyPlayerCameraInput()
        {
            Func<string, float> mouseReaderFunc = GetMouseReaderFunction();
            float x = mouseReaderFunc("Mouse X");
            float y = mouseReaderFunc("Mouse Y");

            yawDegrees += x * Data.Config.Mouse.Yaw * Data.Config.Mouse.Sensitivity;
            pitchDegrees += y * Data.Config.Mouse.Pitch * Data.Config.Mouse.Sensitivity;

            yawDegrees %= 360.0f;
            pitchDegrees = pitchDegrees.Clamp(-90, 90);

            GameObject.transform.rotation = Quaternion.Euler(-pitchDegrees, yawDegrees, 0);

            Entity.Angle = BitAngle.FromDegrees(DoomUnityAngleConverter(yawDegrees));
        }

        public void Update(float tickFraction)
        {
            // Nothing yet.
        }

        public void Tick()
        {
            Vector3 forward = Vec3F.UnitFromDegrees(DoomUnityAngleConverter(yawDegrees));
            Vector3 side = new Vector3(forward.z, 0, -forward.x);

            // TODO: This is obviously bad if we're not the console player...
            Vector3 velocity = Vector3.zero;
            if (Input.GetKey(KeyCode.W) || Input.GetMouseButton(1))
                velocity += forward * ForwardMovementSpeed;
            if (Input.GetKey(KeyCode.A))
                velocity += side * -SideMovementSpeed;
            if (Input.GetKey(KeyCode.S))
                velocity += forward * -ForwardMovementSpeed;
            if (Input.GetKey(KeyCode.D))
                velocity += side * SideMovementSpeed;

            Entity.Velocity += velocity;
        }

        public void Dispose()
        {
            GameObjectHelper.Destroy(GameObject);
        }

        private static Func<string, float> GetMouseReaderFunction()
        {
            if (Data.Config.Mouse.UseRawInput)
                return Input.GetAxisRaw;
            return Input.GetAxis;
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
