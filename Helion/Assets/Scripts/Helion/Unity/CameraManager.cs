using Helion.Util;
using Helion.Util.Unity;
using UnityEngine;

namespace Helion.Unity
{
    public static class CameraManager
    {
        public static Camera Camera;
        public static Vector3 Position;
        internal static EntryPoint entryPoint;

        public static void Initialize(EntryPoint gameEntryPoint)
        {
            entryPoint = gameEntryPoint;
            Camera = Camera.main;
        }

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
