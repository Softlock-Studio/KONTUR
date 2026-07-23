using UnityEngine;

namespace Game.House
{
    [CreateAssetMenu(menuName = "House/House Config", fileName = "HouseConfig")]
    public class HouseConfig : ScriptableObject
    {
        [Header("Mission Timer")]
        public double DayDurationInSecond;
    }
}