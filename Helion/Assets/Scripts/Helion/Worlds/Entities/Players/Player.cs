using System;
using Helion.Resource;
using Helion.Resource.Decorate.Definitions.Properties;
using Helion.Util;
using Helion.Util.Extensions;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;
using Helion.Util.Unity;
using UnityEngine;
using static Helion.Util.Unity.UnityHelper;

namespace Helion.Worlds.Entities.Players
{
    public class Player : ITickable, IDisposable
    {
        public const float MaxMovement = 30.0f;
        private const float ForwardMovementSpeed = 1.5625f;
        private const float SideMovementSpeed = 1.25f;
        private const float FarClipPlaneDistance = 512.0f;
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
            yawDegrees %= 360.0f;

            pitchDegrees += y * Data.Config.Mouse.Pitch * Data.Config.Mouse.Sensitivity;
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
            Vec3F forward = Vec3F.CircleUnitDeg(DoomUnityAngleConverter(yawDegrees));
            Vec3F right = new Vec3F(forward.Z, 0, -forward.X);

            // TODO: This is obviously bad if we're not the console player...
            // TODO: Should use the config!
            Vec3F velocity = Vec3F.Zero;
            if (Input.GetKey(KeyCode.W) || Input.GetMouseButton(1))
                velocity += forward * ForwardMovementSpeed;
            if (Input.GetKey(KeyCode.A))
                velocity += -right * SideMovementSpeed;
            if (Input.GetKey(KeyCode.S))
                velocity += -forward * ForwardMovementSpeed;
            if (Input.GetKey(KeyCode.D))
                velocity += right * SideMovementSpeed;

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
            camera.nearClipPlane = Math.Min(Entity.Radius, Entity.HalfHeight).MapUnit() / 2;
            camera.farClipPlane = FarClipPlaneDistance;

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
