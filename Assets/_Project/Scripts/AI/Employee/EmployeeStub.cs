using Game.Audio;
using Game.Bootstrap;
using Game.House;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Game.AI.Employee
{
    public sealed class EmployeeStub : MonoBehaviour, IEmployee
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private Key makeNoiseKey = Key.Space;
        [SerializeField] private SoundLoudness noiseLoudness = SoundLoudness.Medium;
        [SerializeField] private Babooshka.HearingSensor[] hearingSensorsToNotify;

        private IInputService input;

        public Vector3 Position => transform.position;
        public bool IsAlive { get; private set; } = true;
        public string CurrentStateName => "ManualControl";
        public EmployeeStateId StateId => EmployeeStateId.Idle;
        public int CallsignNumber => 0;
        public string DestinationName => string.Empty;

        private void Start()
        {
            input = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IInputService>();
        }

        private void Update()
        {
            if (!IsAlive || input == null) return;

            Vector2 axis = Vector2.zero;
            if (input.IsKeyHeld(Key.A) || input.IsKeyHeld(Key.LeftArrow)) axis.x -= 1f;
            if (input.IsKeyHeld(Key.D) || input.IsKeyHeld(Key.RightArrow)) axis.x += 1f;
            if (input.IsKeyHeld(Key.S) || input.IsKeyHeld(Key.DownArrow)) axis.y -= 1f;
            if (input.IsKeyHeld(Key.W) || input.IsKeyHeld(Key.UpArrow)) axis.y += 1f;

            Vector3 move = new Vector3(axis.x, 0f, axis.y);
            if (move.sqrMagnitude > 1f) move.Normalize();
            transform.position += move * (moveSpeed * Time.deltaTime);

            if (input.WasKeyPressedThisFrame(makeNoiseKey) && hearingSensorsToNotify != null)
            {
                foreach (Babooshka.HearingSensor sensor in hearingSensorsToNotify)
                    sensor.NotifySound(transform.position, noiseLoudness);
            }
        }

        public bool AssignTask(IEmployeeTask task) => false;
        public void Move(Vector3 point, Zone targetZone = null) { }
        public void Stop() { }
        public void Continue() { }
        public void ReturnToBase() { }
        public void ReactToAttack() { }

        public void ApplyAttackOutcome(bool survived)
        {
            if (!survived) IsAlive = false;
        }
    }
}
