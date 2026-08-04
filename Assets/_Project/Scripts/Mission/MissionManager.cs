using System;
using System.Collections.Generic;
using Game.AI.Employee;
using Game.House;
using Game.House.Model;
using UnityEngine;
using VContainer.Unity;

namespace Game.Mission
{
    public class MissionManager : IStartable, ITickable, IDisposable
    {
        private readonly HouseModel houseModel;
        private readonly EmployeeRegistry employeeRegistry;
        private readonly MissionTimer missionTimer;
        private readonly float infectionFloor01;
        private readonly float infectionCeiling01;

        private bool hasEnded;
        private float maxInfectionReached01;
        private int employeesKilled;

        public float GetTimer => (float)missionTimer.TimeRemaining;
        public bool IsEndDay => missionTimer.IsEndOfDay;
        public int CurrentNight { get; }

        // Fires exactly once per mission: when the timer runs out, victory requires infection to
        // be within [infectionFloor01; infectionCeiling01] at that moment (otherwise it's a
        // defeat, same report screen, just isVictory: false) — or a hard defeat is hit first
        // (infection maxed out at 100% / whole team dead), which ends the mission immediately
        // instead of waiting for day-end. Subscribe to this to drive the results screen's
        // Show(isVictory, maxInfectionReached01, employeesKilled).
        public event Action<LevelEndResult> LevelEnded;

        public MissionManager(HouseConfig houseConfig, HouseModel houseModel, EmployeeRegistry employeeRegistry)
        {
            this.houseModel = houseModel;
            this.employeeRegistry = employeeRegistry;

            missionTimer = new MissionTimer(houseConfig.DayDurationInSecond);
            CurrentNight = houseConfig.NightNumber;
            infectionFloor01 = houseConfig.InfectionFloor01;
            infectionCeiling01 = houseConfig.InfectionCeiling01;
        }

        // Deferred from the constructor because EmployeeRegistry.Employees is only populated in
        // its own Awake() — by Start() time (this class's, not EmployeeRegistry's) every Awake in
        // the scene has already run, same reasoning as HouseModel.Initialize() being called from
        // HousePresenter.Start() rather than done eagerly.
        public void Start()
        {
            foreach (IEmployee employee in employeeRegistry.Employees)
                employee.Died += OnEmployeeDied;
        }

        public void Tick()
        {
            if (hasEnded) return;

            if (!IsEndDay) missionTimer.Update(Time.deltaTime);

            maxInfectionReached01 = Mathf.Max(maxInfectionReached01, houseModel.GetHouseInfectionLevel01());

            if (IsDefeated())
            {
                EndLevel(isVictory: false);
                return;
            }

            if (IsEndDay) EndLevel(isVictory: IsInfectionWithinCorridor());
        }

        // Hard, instant defeat per the GDD (Docs/agents/gdd/defeat.md): whole team dead, or
        // infection maxed out at 100%. Ends the mission the moment it happens instead of waiting
        // for day-end — unlike the corridor check below, which only matters at day-end.
        private bool IsDefeated()
        {
            if (houseModel.GetHouseInfectionLevel01() >= 1f) return true;

            IReadOnlyList<IEmployee> employees = employeeRegistry.Employees;
            if (employees.Count == 0) return false;

            foreach (IEmployee employee in employees)
                if (employee.IsAlive) return false;

            return true;
        }

        // The GDD's "infection corridor" condition (Docs/agents/gdd/defeat.md): the night must
        // end with infection inside [floor; ceiling], not just under the 100% hard cap above.
        private bool IsInfectionWithinCorridor()
        {
            float infection = houseModel.GetHouseInfectionLevel01();
            return infection >= infectionFloor01 && infection <= infectionCeiling01;
        }

        private void EndLevel(bool isVictory)
        {
            hasEnded = true;
            LevelEnded?.Invoke(new LevelEndResult(isVictory, maxInfectionReached01, employeesKilled));
        }

        private void OnEmployeeDied() => employeesKilled++;

        public void Dispose()
        {
            foreach (IEmployee employee in employeeRegistry.Employees)
                employee.Died -= OnEmployeeDied;
        }
    }
}
