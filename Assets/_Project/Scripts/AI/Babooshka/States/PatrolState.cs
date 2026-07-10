using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace Game.AI.Babooshka
{
    public sealed class PatrolState : StateBase
    {
        private const float WaitAtPointDuration = 2f;
        private const float ArrivalThreshold = 0.15f;

        private readonly NavMeshAgent agent;
        private readonly BabooshkaConfig config;
        private readonly Transform[] patrolPoints;

        private int currentPointIndex;
        private float waitTimer;

        public PatrolState(NavMeshAgent agent, BabooshkaConfig config, Transform[] patrolPoints)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.patrolPoints = patrolPoints;
        }

        public override void OnEnter()
        {
            agent.speed = config.PatrolSpeed;
            agent.isStopped = false;
            waitTimer = 0f;

            if (patrolPoints == null || patrolPoints.Length == 0) return;
            currentPointIndex = ClosestPointIndex();
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }

        public override void OnLogic()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;
            if (agent.pathPending || agent.remainingDistance > ArrivalThreshold) return;

            waitTimer += Time.deltaTime;
            if (waitTimer < WaitAtPointDuration) return;

            waitTimer = 0f;
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }

        private int ClosestPointIndex()
        {
            int closest = 0;
            float closestSqrDistance = (patrolPoints[0].position - agent.transform.position).sqrMagnitude;

            for (int i = 1; i < patrolPoints.Length; i++)
            {
                float sqrDistance = (patrolPoints[i].position - agent.transform.position).sqrMagnitude;
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closest = i;
                }
            }

            return closest;
        }
    }
}
