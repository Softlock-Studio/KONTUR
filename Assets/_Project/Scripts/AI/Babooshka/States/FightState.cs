using System;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Babooshka
{
    public sealed class FightState : StateBase
    {
        private enum Phase { Reacting, Resolved }

        private readonly NavMeshAgent agent;
        private readonly BabooshkaConfig config;
        private readonly BabooshkaBlackboard blackboard;
        private readonly Func<float> getInfectionLevel;
        private readonly BabooshkaAnimatorDriver animatorDriver;

        private Phase phase;
        private float elapsed;

        // Captured on enter rather than re-read from blackboard.Target at resolve time: SightSensor
        // ticks every frame regardless of FSM state, so if a second, closer employee wanders into
        // view during AttackReactionDuration it would otherwise silently reassign blackboard.Target
        // mid-fight and the outcome would land on the wrong employee.
        private Employee.IEmployee fightTarget;

        public bool IsResolved { get; private set; }

        public FightState(NavMeshAgent agent, BabooshkaConfig config, BabooshkaBlackboard blackboard,
            Func<float> getInfectionLevel, BabooshkaAnimatorDriver animatorDriver = null)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.blackboard = blackboard;
            this.getInfectionLevel = getInfectionLevel;
            this.animatorDriver = animatorDriver;
        }

        public override void OnEnter()
        {
            elapsed = 0f;
            phase = Phase.Reacting;
            IsResolved = false;
            agent.isStopped = true;

            fightTarget = blackboard.Target;
            animatorDriver?.PlayAttack();
            fightTarget?.ReactToAttack();
        }

        public override void OnLogic()
        {
            elapsed += Time.deltaTime;

            switch (phase)
            {
                case Phase.Reacting:
                    if (elapsed < config.AttackReactionDuration) return;
                    ResolveOutcome();
                    phase = Phase.Resolved;
                    elapsed = 0f;
                    return;

                case Phase.Resolved:
                    if (elapsed < config.FightResolutionDuration) return;
                    IsResolved = true;
                    if (blackboard.Target == fightTarget) blackboard.Target = null;
                    agent.isStopped = false;
                    return;
            }
        }

        private void ResolveOutcome()
        {
            float infection = getInfectionLevel?.Invoke() ?? 0f;
            float deathChance = config.ResolveDeathChance(infection);
            bool employeeDies = UnityEngine.Random.value < deathChance;

            if (!employeeDies && fightTarget != null)
            {
                blackboard.SparedTarget = fightTarget;
                blackboard.SparedUntilTime = Time.time + config.PostFightMercyDuration;
            }

            fightTarget?.ApplyAttackOutcome(!employeeDies);

#if UNITY_EDITOR
            if (config.EnableDebugVisuals)
                Debug.Log($"[Babooshka] Fight resolved: infection={infection:F2}, deathChance={deathChance:F2}, employeeDies={employeeDies}");
#endif
        }
    }
}
