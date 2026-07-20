using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;
using Game.House;

namespace Game.AI.Babooshka
{
    public sealed class WanderState : StateBase
    {
        private const float ArrivalThreshold = 0.15f;

        private enum Phase { Moving, Standing }

        private readonly NavMeshAgent agent;
        private readonly BabooshkaConfig config;
        private readonly Transform[] corridorPoints;
        private readonly IZoneDirectory zoneDirectory;

        private Phase phase;
        private float standStillTimer;
        private float standStillDuration;
        private IWanderZone currentZone;

        public WanderState(NavMeshAgent agent, BabooshkaConfig config, Transform[] corridorPoints, IZoneDirectory zoneDirectory)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.corridorPoints = corridorPoints;
            this.zoneDirectory = zoneDirectory;
        }

        public override void OnEnter()
        {
            agent.speed = config.PatrolSpeed;
            agent.isStopped = false;
            PickNextDestination();
        }

        public override void OnLogic()
        {
            switch (phase)
            {
                case Phase.Moving:
                    if (agent.pathPending || agent.remainingDistance > ArrivalThreshold) return;
                    BeginStandingStill();
                    return;

                case Phase.Standing:
                    standStillTimer += Time.deltaTime;
                    if (standStillTimer < standStillDuration) return;

                    if (currentZone != null && Random.value < config.WallLickChance)
                        currentZone.TriggerInfectionOutbreak();

                    agent.isStopped = false;
                    PickNextDestination();
                    return;
            }
        }

        private void BeginStandingStill()
        {
            phase = Phase.Standing;
            standStillTimer = 0f;
            standStillDuration = Random.Range(config.WanderStandStillMinSeconds, config.WanderStandStillMaxSeconds);
            agent.isStopped = true;
        }

        private void PickNextDestination()
        {
            phase = Phase.Moving;

            bool visitApartment = zoneDirectory != null && Random.value < config.ApartmentVisitChance;
            IWanderZone apartment = visitApartment ? PickRandomApartment() : null;

            if (apartment != null)
            {
                currentZone = apartment;
                agent.SetDestination(apartment.GetWanderPoint());
                return;
            }

            currentZone = null;

            if (corridorPoints == null || corridorPoints.Length == 0)
            {
                // Nothing to wander to at all (empty scene) — just stay put.
                phase = Phase.Standing;
                standStillTimer = 0f;
                standStillDuration = config.WanderStandStillMaxSeconds;
                agent.isStopped = true;
                return;
            }

            Transform point = corridorPoints[Random.Range(0, corridorPoints.Length)];
            agent.SetDestination(point.position);
        }

        // Reservoir sampling: picks one random apartment zone without allocating a list.
        private IWanderZone PickRandomApartment()
        {
            var zones = zoneDirectory.GetZones();
            IWanderZone chosen = null;
            int count = 0;

            for (int i = 0; i < zones.Count; i++)
            {
                if (!zones[i].IsApartment) continue;
                count++;
                if (Random.Range(0, count) == 0) chosen = zones[i];
            }

            return chosen;
        }
    }
}
