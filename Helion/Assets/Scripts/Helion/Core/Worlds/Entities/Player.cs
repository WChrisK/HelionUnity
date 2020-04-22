using Helion.Core.Resource;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using UnityEngine;

namespace Helion.Core.Worlds.Entities
{
    public class Player : MonoBehaviour
    {
        private const float MOVE_FACTOR = 10 * Constants.MapUnit;

        public int PlayerNumber;
        public float Pitch;
        public float Yaw;
        private Entity entity;
        private float deltaTime;

        void Update()
        {
            UpdateCamera();
            HandleSpriteIfConsolePlayer();
        }

        private void HandleSpriteIfConsolePlayer()
        {
            // MeshRenderer MeshRenderer = GetComponent<MeshRenderer>();
            // if (MeshRenderer)
            //     MeshRenderer.enabled = (entity.world.ConsolePlayerNumber != PlayerNumber);
        }

        void FixedUpdate()
        {
            if (!entity)
                return;

            MovePlayer();
        }

        internal void Attach(Entity targetEntity)
        {
            entity = targetEntity;

            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

            CharacterController controller = gameObject.AddComponent<CharacterController>();
            controller.height = entity.Definition.Properties.Height.MapUnit();
            controller.radius = entity.Definition.Properties.Radius.MapUnit();
            controller.center = boxCollider.center;
            controller.stepOffset = 24.MapUnit();
        }

        private void UpdateCamera()
        {
            if (Camera.main == null)
                return;

            Transform cameraTransform = Camera.main.transform;

            // TODO: Multiply by Time.deltaTime? Use non raw for buffering?
            Yaw += Input.GetAxisRaw("Mouse X") * Data.Config.Mouse.Yaw;
            Pitch += Input.GetAxisRaw("Mouse Y") * Data.Config.Mouse.Pitch;

            Pitch = Mathf.Clamp(Pitch, -89.9f, 89.9f);
            while (Yaw < 0)
                Yaw += 360;
            while (Yaw >= 360)
                Yaw -= 360;

            cameraTransform.eulerAngles = new Vector3(-Pitch, Yaw, 0f);
        }

        private void MovePlayer()
        {
            CharacterController controller = gameObject.GetComponent<CharacterController>();
            if (!controller)
                return;

            Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * Yaw), 0, Mathf.Cos(Mathf.Deg2Rad * Yaw));
            Vector3 rightDirection = new Vector3(direction.z, 0, -direction.x);

            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                move += direction;
            if (Input.GetKey(KeyCode.A))
                move -= rightDirection;
            if (Input.GetKey(KeyCode.S))
                move -= direction;
            if (Input.GetKey(KeyCode.D))
                move += rightDirection;
            if (Input.GetKey(KeyCode.Space))
                move += Vector3.up;
            if (Input.GetKey(KeyCode.C))
                move += Vector3.down;

            // Always apply gravity (for now).
            move += Vector3.down;

            if (move != Vector3.zero)
            {
                controller.Move(move * MOVE_FACTOR);
                entity.SetPosition(transform.position / Constants.MapUnit);
            }
        }
    }
}
