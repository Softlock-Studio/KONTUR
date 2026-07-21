using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;
using Game.House;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.AI.Babooshka
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class BabooshkaController : MonoBehaviour, IBabooshka
    {
        [SerializeField] private BabooshkaConfig config;
        [SerializeField] private SightSensor sightSensor;
        [SerializeField] private HearingSensor hearingSensor;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField, Tooltip("Must implement IInfectionDirector; optionally IZoneDirectory too (e.g. ZoneRegistry) for apartment wandering.")]
        private MonoBehaviour infectionDirectorSource;

        private NavMeshAgent agent;
        private StateMachine fsm;
        private BabooshkaBlackboard blackboard;
        private IInfectionDirector infectionDirector;
        private IZoneDirectory zoneDirectory;

        public Vector3 Position => transform.position;
        public string CurrentStateName => fsm?.GetActiveHierarchyPath() ?? string.Empty;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            infectionDirector = infectionDirectorSource as IInfectionDirector;
            zoneDirectory = infectionDirectorSource as IZoneDirectory;
            blackboard = new BabooshkaBlackboard();

            if (sightSensor != null) sightSensor.Bind(blackboard, config);
            if (hearingSensor != null) hearingSensor.Bind(blackboard, config);
        }

        private void Start()
        {
            // Deferred to Start: BuildStateMachine synchronously enters "Wander", which may
            // query infectionDirectorSource (e.g. ZoneRegistry.GetZones()) — that object's own
            // Awake isn't guaranteed to have run yet if it happened first in Awake here.
            // Unity guarantees every Awake in the scene completes before any Start does.
            BuildStateMachine();
        }

        private void BuildStateMachine()
        {
            fsm = new StateMachine();

            var fightState = new FightState(agent, config, blackboard, () => infectionDirector?.GetInfectionLevel() ?? 0f);

            fsm.AddState("Wander", new WanderState(agent, config, patrolPoints, zoneDirectory));
            fsm.AddState("Chase", new ChaseState(agent, config, blackboard));
            fsm.AddState("Search", new SearchState(agent, config, blackboard));
            fsm.AddState("Fight", fightState);

            fsm.SetStartState("Wander");

            fsm.AddTransition("Wander", "Chase", t => blackboard.Target != null);
            fsm.AddTransition("Wander", "Search",
                t => blackboard.Target == null && Time.time - blackboard.LastHeardTime <= config.HearingReactionWindow);

            fsm.AddTransition("Chase", "Fight",
                t => blackboard.Target != null
                    && Vector3.Distance(transform.position, blackboard.Target.Position) <= config.AttackRange);
            fsm.AddTransition("Chase", "Search", t => blackboard.Target == null);

            fsm.AddTransition("Search", "Chase", t => blackboard.Target != null);
            fsm.AddTransition(new TransitionAfter("Search", "Wander", config.InvestigateTimeout));

            fsm.AddTransition("Fight", "Wander", t => fightState.IsResolved);

#if UNITY_EDITOR
            fsm.StateChanged += state =>
            {
                if (config.EnableDebugVisuals) Debug.Log($"[{name}] FSM: {state.name}", this);
            };
#endif

            fsm.Init();
        }

        private void Update()
        {
            if (sightSensor != null) sightSensor.Tick();
            fsm.OnLogic();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (fsm == null || config == null || !config.EnableDebugVisuals) return;
            Handles.Label(transform.position + Vector3.up * 2.2f, CurrentStateName);
        }
#endif
    }
}
