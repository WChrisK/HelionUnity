using Helion.Util;
using Helion.Util.Geometry;
using Helion.Util.Unity;
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
        public static Vector3 Position { get; private set; }
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

            Position = Camera.transform.position / Constants.MapUnit;
            Angle = BitAngle.FromDegrees(DoomUnityAngleConverter(Camera.transform.eulerAngles.y));
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
