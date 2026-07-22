using Game.House;
using Game.House.Model;
using UnityEngine;
using VContainer.Unity;

namespace Game.Mission
{
    public class MissionManager : ITickable
    {
        private HouseModel _houseModel;
        private DayCycle _dayCycle;

        private bool _isWorking;
        private bool _isWin = false;

        public float GetTimer => (float)_dayCycle.Timer;
        public TimeType GetTimeType => _dayCycle.TimeType;
        public bool IsEndDay => _dayCycle.IsEndDay;
        public bool GetResultMission => _isWin;

        public MissionManager(HouseConfig houseConfig, HouseModel houseModel)
        {
            _houseModel = houseModel;

            _dayCycle = new DayCycle(houseConfig.DayDurationInSecond, houseConfig.NightDurationInSecond);

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
                _dayCycle.Update(Time.deltaTime);
                Debug.Log("TimeType: " + GetTimeType + " Time: " + GetTimer + " Status system: " + _isWorking);
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