using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Input
{
    public sealed class InputService : IInputService, ITickable
    {
        public Vector2 MousePosition { get; private set; }
        public Vector2 MouseDelta { get; private set; }

        public bool IsLeftMouseButtonHeld { get; private set; }
        public bool IsRightMouseButtonHeld { get; private set; }
        public bool WasLeftMouseButtonPressedThisFrame { get; private set; }
        public bool WasLeftMouseButtonReleasedThisFrame { get; private set; }
        public bool WasRightMouseButtonPressedThisFrame { get; private set; }
        public bool WasRightMouseButtonReleasedThisFrame { get; private set; }

        public bool WasEscapePressedThisFrame { get; private set; }

        public void Tick()
        {
            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                MousePosition = mouse.position.ReadValue();
                MouseDelta = mouse.delta.ReadValue();

                IsLeftMouseButtonHeld = mouse.leftButton.isPressed;
                IsRightMouseButtonHeld = mouse.rightButton.isPressed;
                WasLeftMouseButtonPressedThisFrame = mouse.leftButton.wasPressedThisFrame;
                WasLeftMouseButtonReleasedThisFrame = mouse.leftButton.wasReleasedThisFrame;
                WasRightMouseButtonPressedThisFrame = mouse.rightButton.wasPressedThisFrame;
                WasRightMouseButtonReleasedThisFrame = mouse.rightButton.wasReleasedThisFrame;
            }
            else
            {
                WasLeftMouseButtonPressedThisFrame = false;
                WasLeftMouseButtonReleasedThisFrame = false;
                WasRightMouseButtonPressedThisFrame = false;
                WasRightMouseButtonReleasedThisFrame = false;
            }

            Keyboard keyboard = Keyboard.current;
            WasEscapePressedThisFrame = keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
        }

        public bool IsKeyHeld(Key key) => Keyboard.current != null && Keyboard.current[key].isPressed;

        public bool WasKeyPressedThisFrame(Key key) => Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame;

        public bool WasKeyReleasedThisFrame(Key key) => Keyboard.current != null && Keyboard.current[key].wasReleasedThisFrame;
    }
}
