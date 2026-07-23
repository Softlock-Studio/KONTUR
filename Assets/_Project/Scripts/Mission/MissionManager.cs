using Game.House;
using Game.House.Model;
using UnityEngine;
using VContainer.Unity;

namespace Game.Mission
{
    public class MissionManager : ITickable
    {
        private HouseModel _houseModel;
        private MissionTimer _missionTimer;

        private bool _isWorking;
        private bool _isWin = false;

        public float GetTimer => (float)_missionTimer.TimeRemaining;
        public bool IsEndDay => _missionTimer.IsEndOfDay;
        public bool GetResultMission => _isWin;

        public MissionManager(HouseConfig houseConfig, HouseModel houseModel)
        {
            _houseModel = houseModel;

            _missionTimer = new MissionTimer(houseConfig.DayDurationInSecond);

            _isWorking = true;
        }

        public void Tick()
        {
            Update();
            CheckCurrentStatus();
        }

        private void Update()
        {
            if (!IsEndDay)
            {
                _missionTimer.Update(Time.deltaTime);
                // Debug.Log("Time left: " + GetTimer + " Status system: " + _isWorking);
            }
        }

        private void CheckCurrentStatus()
        {
            if(_houseModel.GetHouseInfectionLevel01() == 1)
            {
                _isWorking = false;
            }
        }
    }
}