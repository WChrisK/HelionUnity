using Helion.Unity.MonoBehaviours;
using Helion.Util;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;
using Helion.Util.Unity;
using Helion.Worlds.Entities;
using UnityEngine;
using static Helion.Util.Unity.UnityHelper;

namespace Helion.Unity
{
    /// <summary>
    /// A global manager of the camera.
    /// </summary>
    public static class CameraManager
    {
        /// <summary>
        /// The current camera we are looking through.
        /// </summary>
        public static Camera Camera { get; private set; }
        public static BitAngle Angle { get; private set; }
        public static Vec3F Position { get; private set; }
        internal static EntryPoint entryPoint;

        /// <summary>
        /// To be called right at initialization.
        /// </summary>
        /// <param name="gameEntryPoint">The entry point module.</param>
        public static void Initialize(EntryPoint gameEntryPoint)
        {
            entryPoint = gameEntryPoint;
            Camera = Camera.main;
        }

        /// <summary>
        /// To be called each frame. Should be done before we do any other
        /// update calls.
        /// </summary>
        public static void Update()
        {
            // This is a safeguard in case we have not initialized yet.
            if (entryPoint == null)
                return;

            // Because Unity requires us to enable and disable cameras, we will
            // have to detect when any changes happen and do it ourselves.
            Camera camera = FindCamera();
            UpdateToNewCameraIfNeeded(camera);

            Position = Camera.transform.position.AsVec() / Constants.MapUnit;
            Angle = BitAngle.FromDegrees(DoomUnityAngleConverter(Camera.transform.eulerAngles.y));
        }

        /// <summary>
        /// Gets the rotational index between 0 - 7 for which sprite rotation
        /// to use.
        /// </summary>
        /// <param name="entity">The entity we're looking at.</param>
        /// <param name="tickFraction">The fraction for interpolation.</param>
        /// <returns>The index to use as an offset in a sprite rotation object.
        /// </returns>
        public static int CalculateRotationIndex(Entity entity, float tickFraction)
        {
            // TODO: We keep doing pointless XYZ -> XZ conversions when we don't need to.
            // TODO: Does not take interpolation of the camera position into account!
            Vec2F eye = Position.XZ;
            Vec2F other = entity.Position.Value(tickFraction).XZ;
            uint bits = BitAngle.ToDiamondAngle(eye, other);
            return (int)BitAngle.CalculateSpriteRotation(bits, entity.Angle.Bits);
        }

        private static Camera FindCamera()
        {
            if (entryPoint.player != null)
                return entryPoint.player.Camera;

            Camera camera = Camera.main;
            Debug.Assert(camera != null, "Should never have a scene with no cameras");

            return camera;
        }

        private static void UpdateToNewCameraIfNeeded(Camera newCamera)
        {
            if (ReferenceEquals(Camera, newCamera))
                return;

            Camera.enabled = false;
            GameObjectHelper.Destroy(Camera.GetComponent<AudioListener>());

            newCamera.enabled = true;
            newCamera.gameObject.AddComponent<AudioListener>();

            Camera = newCamera;
        }
    }
}
