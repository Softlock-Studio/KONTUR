using UnityEngine;

namespace Game.AI.Employee
{
    [CreateAssetMenu(menuName = "AI/Employee Config", fileName = "EmployeeConfig")]
    public sealed class EmployeeConfig : ScriptableObject
    {
        [Header("Movement")]
        public float MoveSpeed = 3.5f;
        public float ReturnSpeed = 3.5f;
        public float FleeSpeed = 5f;
        public float ArrivalThreshold = 0.15f;

        [Header("Death")]
        public bool CorpseDespawnEnabled = false;
        public float CorpseDespawnDelaySeconds = 20f;

        [Tooltip("Freezes the ragdoll in place and stops it colliding with anything, while staying visible.")]
        public bool CorpseCollisionDisableEnabled = false;
        public float CorpseCollisionDisableDelaySeconds = 5f;
    }
}
