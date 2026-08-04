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

        private bool hasEnded;
        private float maxInfectionReached01;
        private int employeesKilled;

        public float GetTimer => (float)missionTimer.TimeRemaining;
        public bool IsEndDay => missionTimer.IsEndOfDay;
        public int CurrentNight { get; }

        // Fires exactly once per mission, either when the timer runs out (victory — the night was
        // survived) or a defeat condition is hit first (infection maxed out / whole team dead).
        // Subscribe to this to drive the results screen's Show(isVictory, maxInfectionReached01,
        // employeesKilled).
        public event Action<LevelEndResult> LevelEnded;

        public MissionManager(HouseConfig houseConfig, HouseModel houseModel, EmployeeRegistry employeeRegistry)
        {
            this.houseModel = houseModel;
            this.employeeRegistry = employeeRegistry;

            missionTimer = new MissionTimer(houseConfig.DayDurationInSecond);
            CurrentNight = houseConfig.NightNumber;
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

            if (IsEndDay) EndLevel(isVictory: true);
        }

        // Defeat, per the GDD (Docs/agents/gdd/defeat.md): whole team dead, or infection maxed
        // out. The third candidate there — infection corridor floor breached twice — isn't checked
        // here because no floor/ceiling values are wired up yet (see HousePresenter's
        // SetHouseInfectionRange TODO).
        private bool IsDefeated()
        {
            if (houseModel.GetHouseInfectionLevel01() >= 1f) return true;

            IReadOnlyList<IEmployee> employees = employeeRegistry.Employees;
            if (employees.Count == 0) return false;

            foreach (IEmployee employee in employees)
                if (employee.IsAlive) return false;

            return true;
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
