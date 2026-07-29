using Game.Bootstrap;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Game.Debugging
{
    public sealed class SpectatorFlyCamera : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float fastMoveMultiplier = 3f;
        [SerializeField] private float lookSensitivity = 0.1f;

        private IInputService input;
        private float yaw;
        private float pitch;

        private void Awake()
        {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }

        private void Start()
        {
            input = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IInputService>();
        }

        private void Update()
        {
            if (input == null) return;

            if (input.IsRightMouseButtonHeld)
            {
                Vector2 look = input.MouseDelta * lookSensitivity;
                yaw += look.x;
                pitch -= look.y;
                pitch = Mathf.Clamp(pitch, -89f, 89f);
                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            Vector3 move = Vector3.zero;
            if (input.IsKeyHeld(Key.W)) move += transform.forward;
            if (input.IsKeyHeld(Key.S)) move -= transform.forward;
            if (input.IsKeyHeld(Key.A)) move -= transform.right;
            if (input.IsKeyHeld(Key.D)) move += transform.right;
            if (input.IsKeyHeld(Key.E)) move += transform.up;
            if (input.IsKeyHeld(Key.Q)) move -= transform.up;

            float speed = moveSpeed * (input.IsKeyHeld(Key.LeftShift) ? fastMoveMultiplier : 1f);
            transform.position += move.normalized * speed * Time.deltaTime;
        }
    }
}
