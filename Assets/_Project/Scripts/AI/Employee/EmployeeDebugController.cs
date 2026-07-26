using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Game.Bootstrap;
using Game.House;
using Game.Input;
using VContainer;
using VContainer.Unity;

namespace Game.AI.Employee
{
    public sealed class EmployeeDebugController : MonoBehaviour
    {
        [SerializeField] private bool debugEnabled = false;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private LayerMask clickMask = ~0;
        [SerializeField] private float navMeshSampleRadius = 2f;
        [SerializeField] private float debugTaskDuration = 5f;
        [SerializeField] private ActivityType debugActivityType = ActivityType.Treatment;
        [SerializeField] private ZoneEventType debugTargetEventType;

        [Header("Keys")]
        [SerializeField] private Key stopKey = Key.X;
        [SerializeField] private Key returnToBaseKey = Key.B;
        [SerializeField] private Key simulateSurviveKey = Key.F;
        [SerializeField] private Key simulateDeathKey = Key.G;

        private IInputService input;
        private IEmployee selectedEmployee;
        private string lastZoneMessage = string.Empty;

        private void Awake()
        {
            if (targetCamera == null) targetCamera = Camera.main;
        }

        private void Start()
        {
            input = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IInputService>();
        }

        private void Update()
        {
            if (!debugEnabled) return;

            HandleClick();
            HandleKeys();
        }

        private void HandleClick()
        {
            if (input == null || targetCamera == null) return;

            bool assignTask = input.WasRightMouseButtonPressedThisFrame;
            bool leftClick = input.WasLeftMouseButtonPressedThisFrame;
            if (!assignTask && !leftClick) return;

            Ray ray = targetCamera.ScreenPointToRay(input.MousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, clickMask)) return;

            if (leftClick)
            {
                IEmployee clickedEmployee = hit.collider.GetComponentInParent<IEmployee>();
                if (clickedEmployee != null)
                {
                    selectedEmployee = clickedEmployee;
                    lastZoneMessage = string.Empty;
                    return;
                }
            }

            if (selectedEmployee == null) return;

            if (assignTask)
            {
                Zone zone = hit.collider.GetComponentInParent<Zone>();
                if (zone != null)
                {
                    ZoneEventType? targetEvent = debugActivityType == ActivityType.ResidentEvent
                        ? debugTargetEventType
                        : null;
                    bool assigned = zone.TryAssign(selectedEmployee, debugActivityType, targetEvent, out string reason);
                    lastZoneMessage = assigned ? $"Assigned to {zone.DisplayName}" : $"Assign failed: {reason}";
                    return;
                }
            }

            if (!NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, navMeshSampleRadius, NavMesh.AllAreas)) return;

            if (assignTask) selectedEmployee.AssignTask(new DebugEmployeeTask(navHit.position, debugTaskDuration));
            else selectedEmployee.Move(navHit.position);
        }

        private void HandleKeys()
        {
            if (selectedEmployee == null || input == null) return;

            if (input.WasKeyPressedThisFrame(stopKey)) selectedEmployee.Stop();
            if (input.WasKeyPressedThisFrame(returnToBaseKey)) selectedEmployee.ReturnToBase();
            if (input.WasKeyPressedThisFrame(simulateSurviveKey)) selectedEmployee.ApplyAttackOutcome(survived: true);
            if (input.WasKeyPressedThisFrame(simulateDeathKey)) selectedEmployee.ApplyAttackOutcome(survived: false);
        }

        private void OnGUI()
        {
            if (!debugEnabled) return;

            if (selectedEmployee == null)
            {
                GUI.Label(new Rect(10, 10, 400, 20), "No employee selected — left-click one to select");
                return;
            }

            Component selectedComponent = selectedEmployee as Component;
            string label = selectedComponent != null ? selectedComponent.name : "employee";
            GUI.Label(new Rect(10, 10, 400, 20), $"Selected: {label}   State: {selectedEmployee.CurrentStateName}   Alive: {selectedEmployee.IsAlive}");
            GUI.Label(new Rect(10, 30, 500, 20), "LMB: select/move RMB: assign debug task/room X/B/F/G: Stop/Return/Survive/Die");
            if (!string.IsNullOrEmpty(lastZoneMessage)) GUI.Label(new Rect(10, 105, 500, 20), lastZoneMessage);

            GUI.enabled = selectedEmployee.IsAlive;
            if (GUI.Button(new Rect(10, 55, 90, 25), "Stop")) selectedEmployee.Stop();
            if (GUI.Button(new Rect(105, 55, 120, 25), "Return to base")) selectedEmployee.ReturnToBase();
            if (GUI.Button(new Rect(230, 55, 140, 25), "Simulate survive")) selectedEmployee.ApplyAttackOutcome(survived: true);
            if (GUI.Button(new Rect(375, 55, 120, 25), "Simulate death")) selectedEmployee.ApplyAttackOutcome(survived: false);
            GUI.enabled = true;
        }
    }
}
