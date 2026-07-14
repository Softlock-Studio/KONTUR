using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Game.AI.Employee
{
    public sealed class EmployeeDebugController : MonoBehaviour
    {
        [SerializeField] private bool debugEnabled = false;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private MonoBehaviour employeeSource;
        [SerializeField] private LayerMask clickMask = ~0;
        [SerializeField] private float navMeshSampleRadius = 2f;
        [SerializeField] private float debugTaskDuration = 5f;

        [Header("Keys")]
        [SerializeField] private Key stopKey = Key.X;
        [SerializeField] private Key returnToBaseKey = Key.B;
        [SerializeField] private Key simulateSurviveKey = Key.F;
        [SerializeField] private Key simulateDeathKey = Key.G;

        private IEmployee employee;

        private bool IsActive => debugEnabled && employee != null;

        private void Awake()
        {
            employee = employeeSource as IEmployee;
            if (targetCamera == null) targetCamera = Camera.main;
        }

        private void Update()
        {
            if (!IsActive) return;

            HandleClick();
            HandleKeys();
        }

        private void HandleClick()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || targetCamera == null) return;

            bool assignTask = mouse.rightButton.wasPressedThisFrame;
            bool move = mouse.leftButton.wasPressedThisFrame;
            if (!assignTask && !move) return;

            Ray ray = targetCamera.ScreenPointToRay(mouse.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, clickMask)) return;
            if (!NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, navMeshSampleRadius, NavMesh.AllAreas)) return;

            if (assignTask) employee.AssignTask(new DebugEmployeeTask(navHit.position, debugTaskDuration));
            else employee.Move(navHit.position);
        }

        private void HandleKeys()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard[stopKey].wasPressedThisFrame) employee.Stop();
            if (keyboard[returnToBaseKey].wasPressedThisFrame) employee.ReturnToBase();
            if (keyboard[simulateSurviveKey].wasPressedThisFrame) employee.ApplyAttackOutcome(survived: true);
            if (keyboard[simulateDeathKey].wasPressedThisFrame) employee.ApplyAttackOutcome(survived: false);
        }

        private void OnGUI()
        {
            if (!debugEnabled) return;

            if (employee == null)
            {
                GUI.Label(new Rect(10, 10, 400, 20), "EmployeeDebugController: no IEmployee assigned");
                return;
            }

            GUI.Label(new Rect(10, 10, 400, 20), $"State: {employee.CurrentStateName}   Alive: {employee.IsAlive}");
            GUI.Label(new Rect(10, 30, 500, 20), "LMB: Move   RMB: Assign debug task   X/B/F/G: Stop / Return / Survive / Die");

            GUI.enabled = employee.IsAlive;
            if (GUI.Button(new Rect(10, 55, 90, 25), "Stop")) employee.Stop();
            if (GUI.Button(new Rect(105, 55, 120, 25), "Return to base")) employee.ReturnToBase();
            if (GUI.Button(new Rect(230, 55, 140, 25), "Simulate survive")) employee.ApplyAttackOutcome(survived: true);
            if (GUI.Button(new Rect(375, 55, 120, 25), "Simulate death")) employee.ApplyAttackOutcome(survived: false);
            GUI.enabled = true;
        }
    }
}
