using System.Collections.Generic;
using Game.Audio;
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

        [Header("Locomotion Animation")]
        [Tooltip("Seconds of continuous movement needed to accelerate from a walk to a full run.")]
        public float AccelerationTime = 3f;
        [Tooltip("Remaining path distance below which the employee decelerates back to a walk before arriving.")]
        public float BrakingDistance = 4f;

        [Header("Death")]
        public bool CorpseDespawnEnabled = false;
        public float CorpseDespawnDelaySeconds = 20f;

        [Tooltip("Freezes the ragdoll in place and stops it colliding with anything, while staying visible.")]
        public bool CorpseCollisionDisableEnabled = false;
        public float CorpseCollisionDisableDelaySeconds = 5f;

        [Header("Audio")]
        public SfxCue DeathCue;
        public SfxCue FleeCue;

        [Header("Sounds (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("How often (seconds) a continuous action (walking, cleaning, ...) re-triggers its configured sound while active.")]
        public float SoundEmitIntervalSeconds = 1f;
        public List<EmployeeSoundDefinition> Sounds = new();

        public bool TryGetSound(EmployeeSoundType type, out EmployeeSoundDefinition sound)
        {
            foreach (EmployeeSoundDefinition candidate in Sounds)
            {
                if (candidate.Type != type) continue;
                sound = candidate;
                return true;
            }

            sound = null;
            return false;
        }
    }
}
