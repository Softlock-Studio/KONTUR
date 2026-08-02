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

        [Header("Attacked reaction")]
        [Tooltip("How long the employee stays frozen in the Attacked state (reaction animation) before being allowed to flee, regardless of when Babooshka's fight already decided the outcome. Tune to match the length of whatever hit-reaction animation/sequence is authored, so fleeing doesn't cut it short.")]
        public float AttackedHoldDurationSeconds = 1.5f;

        [Header("Death")]
        public bool CorpseDespawnEnabled = false;
        public float CorpseDespawnDelaySeconds = 20f;

        [Tooltip("Freezes the ragdoll in place and stops it colliding with anything, while staying visible.")]
        public bool CorpseCollisionDisableEnabled = false;
        public float CorpseCollisionDisableDelaySeconds = 5f;

        [Header("Audio")]
        public SfxCue DeathCue;
        public SfxCue FleeCue;
        public SfxCue AttackedCue;

        [Header("Sounds (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Per-type re-trigger cadence — see SoundDefinition.MinIntervalSeconds/MaxIntervalSeconds.")]
        public List<SoundDefinition<EmployeeSoundType>> Sounds = new();
    }
}
