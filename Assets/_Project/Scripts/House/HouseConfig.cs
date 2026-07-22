using UnityEngine;

namespace Game.House
{
    [CreateAssetMenu(menuName = "House/House Config", fileName = "HouseConfig")]
    public class HouseConfig
    {
        [Header("Day/Night Cycle")]
        public double DayDurationInSecond;
        public double NightDurationInSecond;
    }
}