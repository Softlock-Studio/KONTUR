using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Debugging
{
    public sealed class SpectatorFlyCamera : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float fastMoveMultiplier = 3f;
        [SerializeField] private float lookSensitivity = 0.1f;

        private float yaw;
        private float pitch;

        private void Awake()
        {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            if (keyboard == null || mouse == null) return;

            if (mouse.rightButton.isPressed)
            {
                Vector2 look = mouse.delta.ReadValue() * lookSensitivity;
                yaw += look.x;
                pitch -= look.y;
                pitch = Mathf.Clamp(pitch, -89f, 89f);
                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            Vector3 move = Vector3.zero;
            if (keyboard.wKey.isPressed) move += transform.forward;
            if (keyboard.sKey.isPressed) move -= transform.forward;
            if (keyboard.aKey.isPressed) move -= transform.right;
            if (keyboard.dKey.isPressed) move += transform.right;
            if (keyboard.eKey.isPressed) move += transform.up;
            if (keyboard.qKey.isPressed) move -= transform.up;

            float speed = moveSpeed * (keyboard.leftShiftKey.isPressed ? fastMoveMultiplier : 1f);
            transform.position += move.normalized * speed * Time.deltaTime;
        }
    }
}
