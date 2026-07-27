using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    // The only input contract other systems should depend on — wraps the new Input System's
    // Mouse/Keyboard device polling so call sites never touch Mouse.current/Keyboard.current
    // directly. Polled once per frame by InputService.Tick(), so every property here is already
    // this-frame-accurate by the time gameplay code reads it in its own Update.
    public interface IInputService
    {
        Vector2 MousePosition { get; }
        Vector2 MouseDelta { get; }

        bool IsLeftMouseButtonHeld { get; }
        bool IsRightMouseButtonHeld { get; }
        bool WasLeftMouseButtonPressedThisFrame { get; }
        bool WasLeftMouseButtonReleasedThisFrame { get; }
        bool WasRightMouseButtonPressedThisFrame { get; }
        bool WasRightMouseButtonReleasedThisFrame { get; }

        bool WasEscapePressedThisFrame { get; }

        // Generic keyboard access for call sites that need a specific/rebindable key (debug
        // hotkeys, WASD movement) beyond the LMB/RMB/Esc convenience members above.
        bool IsKeyHeld(Key key);
        bool WasKeyPressedThisFrame(Key key);
        bool WasKeyReleasedThisFrame(Key key);
    }
}
