using UnityEngine;

namespace Game.AI.Babooshka
{
    public sealed class SightSensor : MonoBehaviour
    {
        [SerializeField] private Transform eye;

        private BabooshkaBlackboard blackboard;
        private BabooshkaConfig config;

        public void Bind(BabooshkaBlackboard board, BabooshkaConfig cfg)
        {
            blackboard = board;
            config = cfg;
        }

        public void Tick()
        {
            Transform origin = eye != null ? eye : transform;
            Collider[] hits = Physics.OverlapSphere(origin.position, config.SightRadius, config.EmployeeLayer);

            Employee.IEmployee visible = null;
            float closestSqrDistance = float.MaxValue;

            foreach (Collider hit in hits)
            {
                Employee.IEmployee employee = hit.GetComponentInParent<Employee.IEmployee>();
                if (employee == null || !employee.IsAlive) continue;
                if (employee == blackboard.SparedTarget && Time.time < blackboard.SparedUntilTime) continue;

                Vector3 toTarget = employee.Position - origin.position;
                float sqrDistance = toTarget.sqrMagnitude;
                if (sqrDistance > closestSqrDistance) continue;

                float angle = Vector3.Angle(origin.forward, toTarget);
                if (angle > config.SightAngle * 0.5f) continue;

                float distance = Mathf.Sqrt(sqrDistance);
                bool obstructed = Physics.Raycast(origin.position, toTarget.normalized, out RaycastHit rayHit, distance, config.ObstacleMask)
                    && rayHit.collider.gameObject != hit.gameObject;
                if (obstructed) continue;

                visible = employee;
                closestSqrDistance = sqrDistance;
            }

            visible = ApplyAggressionGate(visible);

            blackboard.Target = visible;
            if (visible != null)
            {
                blackboard.LastKnownTargetPosition = visible.Position;
                blackboard.LastSeenTime = Time.time;
            }
        }

        // Gates a raw sighting through config.AggressionChance01 — see the field's tooltip for the
        // never/sometimes/always semantics and the "sticks until sight is lost" reasoning.
        private Employee.IEmployee ApplyAggressionGate(Employee.IEmployee visible)
        {
            if (visible == null)
            {
                blackboard.IgnoredSightTarget = null;
                return null;
            }

            if (visible == blackboard.Target) return visible;
            if (visible == blackboard.IgnoredSightTarget) return null;

            if (ShouldEngage(config.AggressionChance01)) return visible;

            blackboard.IgnoredSightTarget = visible;
            return null;
        }

        private static bool ShouldEngage(float chance01)
        {
            if (chance01 <= 0f) return false;
            if (chance01 >= 1f) return true;
            return Random.value < chance01;
        }

        private void OnDrawGizmos()
        {
            if (config == null || !config.EnableDebugVisuals) return;

            Transform origin = eye != null ? eye : transform;
            bool seesTarget = blackboard != null && blackboard.Target != null;
            Gizmos.color = seesTarget ? new Color(1f, 0.15f, 0.15f, 0.9f) : new Color(0.2f, 1f, 0.2f, 0.6f);

            Gizmos.DrawWireSphere(origin.position, config.SightRadius);

            Quaternion leftRot = Quaternion.AngleAxis(-config.SightAngle * 0.5f, Vector3.up);
            Quaternion rightRot = Quaternion.AngleAxis(config.SightAngle * 0.5f, Vector3.up);
            Gizmos.DrawLine(origin.position, origin.position + leftRot * origin.forward * config.SightRadius);
            Gizmos.DrawLine(origin.position, origin.position + rightRot * origin.forward * config.SightRadius);
        }
    }
}
