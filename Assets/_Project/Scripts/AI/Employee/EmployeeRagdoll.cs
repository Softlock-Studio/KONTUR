using UnityEngine;
using UnityEngine.AI;

namespace Game.AI.Employee
{
    // Turns the employee into a physical ragdoll on death. Expects the bone rig
    // (Rigidbody/Collider/CharacterJoint per bone) to already be built via Unity's
    // Ragdoll Wizard (GameObject > 3D Object > Ragdoll...) on the model's child
    // hierarchy — this component only toggles what the wizard creates, it doesn't
    // create it. Everything is discovered through the hierarchy, so no per-bone
    // wiring is needed once the wizard has run.
    public sealed class EmployeeRagdoll : MonoBehaviour
    {
        [SerializeField] private Collider mainCollider;

        private Animator animator;
        private NavMeshAgent agent;
        private Rigidbody[] ragdollBodies;
        private EmployeeConfig config;
        private bool isDead;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            ragdollBodies = GetComponentsInChildren<Rigidbody>(true);

            // Animation drives the bones while alive; physics only takes over on death.
            foreach (Rigidbody body in ragdollBodies)
                body.isKinematic = true;
        }

        public void Bind(EmployeeConfig employeeConfig)
        {
            config = employeeConfig;
        }

        public void TriggerDeath()
        {
            if (isDead) return;
            isDead = true;

            if (animator != null) animator.enabled = false;
            if (agent != null) agent.enabled = false;
            if (mainCollider != null) mainCollider.enabled = false;

            foreach (Rigidbody body in ragdollBodies)
                body.isKinematic = false;

            if (config != null && config.CorpseDespawnEnabled)
                Invoke(nameof(Despawn), config.CorpseDespawnDelaySeconds);

            if (config != null && config.CorpseCollisionDisableEnabled)
                Invoke(nameof(DisableCollisions), config.CorpseCollisionDisableDelaySeconds);
        }

        private void Despawn() => gameObject.SetActive(false);

        // Freezes the ragdoll exactly where it settled and stops it colliding with (or being
        // pushed by) anything, without touching visibility — the mesh stays as-is.
        private void DisableCollisions()
        {
            foreach (Rigidbody body in ragdollBodies)
            {
                body.isKinematic = true;
                body.detectCollisions = false;
            }
        }
    }
}
