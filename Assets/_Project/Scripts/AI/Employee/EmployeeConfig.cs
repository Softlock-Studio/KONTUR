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
    }
}
