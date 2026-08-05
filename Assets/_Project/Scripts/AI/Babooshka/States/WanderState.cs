using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;
using Game.AI;
using Game.Audio;
using Game.House;

namespace Game.AI.Babooshka
{
    public sealed class WanderState : StateBase
    {
        private const float ArrivalThreshold = 0.15f;

        private enum Phase { Moving, Standing, PerformingAction }

        private readonly NavMeshAgent agent;
        private readonly BabooshkaConfig config;
        private readonly Transform[] corridorPoints;
        private readonly IZoneDirectory zoneDirectory;
        private readonly AudioEmitter audioEmitter;
        private readonly BabooshkaAnimatorDriver animatorDriver;
        private readonly LoopingSoundEmitter<BabooshkaSoundType> soundEmitter;

        private readonly BabooshkaBlackboard blackboard;

        private Phase phase;
        private float standStillTimer;
        private float standStillDuration;
        private float actionTimer;
        private float actionDuration;
        private IWanderZone currentZone;

        public WanderState(NavMeshAgent agent, BabooshkaConfig config, Transform[] corridorPoints, IZoneDirectory zoneDirectory,
            AudioEmitter audioEmitter = null, BabooshkaAnimatorDriver animatorDriver = null,
            LoopingSoundEmitter<BabooshkaSoundType> soundEmitter = null, BabooshkaBlackboard blackboard = null)
            : base(needsExitTime: false)
        {
            this.agent = agent;
            this.config = config;
            this.corridorPoints = corridorPoints;
            this.zoneDirectory = zoneDirectory;
            this.audioEmitter = audioEmitter;
            this.animatorDriver = animatorDriver;
            this.soundEmitter = soundEmitter;
            this.blackboard = blackboard;
        }

        public override void OnEnter()
        {
            agent.speed = config.PatrolSpeed;
            agent.isStopped = false;

            // Fully disengaged from any hunt — forget who she's already barked Anger at, so the
            // next spotting (this employee again, or someone else) counts as a fresh encounter.
            if (blackboard != null) blackboard.LastAngeredTarget = null;

            PickNextDestination();
        }

        public override void OnLogic()
        {
            // Laughs whether moving or standing still ("пока просто ходит" — the whole time she's
            // idly wandering, not gated to mid-stride); footsteps only while actually moving.
            soundEmitter?.Tick(BabooshkaSoundType.Laugh, Time.deltaTime);

            switch (phase)
            {
                case Phase.Moving:
                    soundEmitter?.Tick(BabooshkaSoundType.Footstep, Time.deltaTime);

                    // No path (or only a partial one) to the current destination — give up on it
                    // immediately and pick a different one, instead of standing there forever
                    // "arriving" at a point she can never actually reach.
                    if (agent.HasUnreachableDestination())
                    {
                        PickNextDestination();
                        return;
                    }

                    if (agent.pathPending || agent.remainingDistance > ArrivalThreshold) return;
                    BeginStandingStill();
                    return;

                case Phase.Standing:
                    standStillTimer += Time.deltaTime;
                    if (standStillTimer < standStillDuration) return;

                    // else-if, not two independent rolls: at most one "creepy event" per visit,
                    // so we never fire two animator triggers in the same frame (see BabooshkaConfig.LightOffChance).
                    if (currentZone != null && Random.value < config.WallLickChance)
                    {
                        currentZone.TriggerInfectionOutbreak();
                        // mustFinish: false — she stops licking as soon as this visit's over, so the
                        // sound shouldn't keep playing after the next thing (e.g. Laugh) needs the
                        // General channel; unlike Laugh/Anger/Taunt/LightOff/Attack, which must finish.
                        audioEmitter?.Play(config.WallLickCue, mustFinish: false);
                        animatorDriver?.PlayWallLick();
                        BeginPerformingAction(config.WallLickDuration);
                        return;
                    }

                    if (currentZone != null && Random.value < config.LightOffChance && currentZone.TryTurnOffLight())
                    {
                        audioEmitter?.Play(config.LightOffCue);
                        animatorDriver?.PlayLightOff();
                        BeginPerformingAction(config.LightOffDuration);
                        return;
                    }

                    agent.isStopped = false;
                    PickNextDestination();
                    return;

                case Phase.PerformingAction:
                    // Stays stopped for the wall-lick/light-off animation's own duration — she only
                    // moves on once it's actually finished, not the instant the trigger fires.
                    actionTimer += Time.deltaTime;
                    if (actionTimer < actionDuration) return;

                    agent.isStopped = false;
                    PickNextDestination();
                    return;
            }
        }

        private void BeginPerformingAction(float duration)
        {
            phase = Phase.PerformingAction;
            actionTimer = 0f;
            actionDuration = duration;
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
