using System.Collections.Generic;
using Game.Bootstrap;
using Game.House;
using Game.Save;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.AI.Employee
{
    public sealed class EmployeeRegistry : MonoBehaviour
    {
        // Wired by hand to the same asset as this scene's MissionScope.houseConfig — not
        // DI-injected because activation must happen in Awake (see below), and Awake ordering
        // between EmployeeRegistry and MissionScope isn't guaranteed, so MissionScope's own
        // container might not be built yet at this point.
        [SerializeField] private HouseConfig houseConfig;

        private IReadOnlyList<IEmployee> activeEmployees;

        public IReadOnlyList<IEmployee> Employees => activeEmployees;

        // Everything here — including the roster-size decision — has to happen in Awake rather
        // than a VContainer IStartable: Unity always finishes every object's Awake before calling
        // Start on any of them, native Unity or VContainer's alike, but native MonoBehaviour.Start
        // (e.g. EmployeeListView, which reads Employees in its own Start) runs BEFORE VContainer's
        // IStartable.Start in the same frame. An IStartable-based activator would race that and
        // sometimes lose, leaving the Employee List UI bound to an empty roster.
        //
        // GameLifetimeScope is safe to resolve from here (it's a different, already-built root
        // scope, persisting since MainMenu) — MissionScope's own container is not, hence the
        // SerializeField above instead of injecting HouseConfig the normal way.
        private void Awake()
        {
            // Scene-filtered: during a level transition the previous level is still loaded
            // additively at this point (see SceneController.LevelLoad), so an unfiltered
            // FindObjectsByType would also pick up the outgoing scene's employees.
            var placed = new List<EmployeeController>();
            foreach (EmployeeController candidate in FindObjectsByType<EmployeeController>(FindObjectsSortMode.None))
                if (candidate.gameObject.scene == gameObject.scene) placed.Add(candidate);

            ISaveService saveService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<ISaveService>();
            int previousAliveCount = saveService.TryLoad(out SaveData data) ? data.AliveEmployeeCount : 0;
            int targetCount = Mathf.Clamp(previousAliveCount + houseConfig.EmployeeReinforcements, 0, placed.Count);

            var active = new List<IEmployee>(targetCount);
            for (int i = 0; i < placed.Count; i++)
            {
                bool shouldBeActive = i < targetCount;
                placed[i].gameObject.SetActive(shouldBeActive);
                if (shouldBeActive) active.Add(placed[i]);
            }

            activeEmployees = active;
        }
    }
}
