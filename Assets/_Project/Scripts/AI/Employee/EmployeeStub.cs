using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.AI.Employee
{
    public sealed class EmployeeStub : MonoBehaviour, IEmployee
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private Key makeNoiseKey = Key.Space;
        [SerializeField] private Babooshka.HearingSensor[] hearingSensorsToNotify;

        public Vector3 Position => transform.position;
        public bool IsAlive { get; private set; } = true;
        public string CurrentStateName => "ManualControl";

        private void Update()
        {
            if (!IsAlive) return;

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            Vector2 axis = Vector2.zero;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) axis.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) axis.x += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) axis.y -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) axis.y += 1f;

            Vector3 input = new Vector3(axis.x, 0f, axis.y);
            if (input.sqrMagnitude > 1f) input.Normalize();
            transform.position += input * (moveSpeed * Time.deltaTime);

            if (keyboard[makeNoiseKey].wasPressedThisFrame && hearingSensorsToNotify != null)
            {
                foreach (Babooshka.HearingSensor sensor in hearingSensorsToNotify)
                    sensor.NotifySound(transform.position);
            }
        }

        public bool AssignTask(IEmployeeTask task) => false;
        public void Move(Vector3 point) { }
        public void Stop() { }
        public void ReturnToBase() { }

        public void ApplyAttackOutcome(bool survived)
        {
            if (!survived) IsAlive = false;
        }
    }
}
