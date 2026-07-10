using System;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Babooshka
{
    public sealed class FightState : StateBase
    {
        private readonly NavMeshAgent agent;
        private readonly BabooshkaConfig config;
        private readonly BabooshkaBlackboard blackboard;
        private readonly Func<float> getInfectionLevel;

        private float elapsed;

        public bool IsResolved { get; private set; }

        public FightState(NavMeshAgent agent, BabooshkaConfig config, BabooshkaBlackboard blackboard, Func<float> getInfectionLevel)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.blackboard = blackboard;
            this.getInfectionLevel = getInfectionLevel;
        }

        public override void OnEnter()
        {
            elapsed = 0f;
            IsResolved = false;
            agent.isStopped = true;

            float infection = getInfectionLevel?.Invoke() ?? 0f;
            float deathChance = config.ResolveDeathChance(infection);
            bool employeeDies = UnityEngine.Random.value < deathChance;

            // TODO: use death roll on employee when it'll be made
            Debug.Log($"[Babooshka] Fight resolved: infection={infection:F2}, deathChance={deathChance:F2}, employeeDies={employeeDies}");
        }

        public override void OnLogic()
        {
            elapsed += Time.deltaTime;
            if (elapsed < config.FightResolutionDuration) return;

            IsResolved = true;
            blackboard.Target = null;
            agent.isStopped = false;
        }
    }
}
