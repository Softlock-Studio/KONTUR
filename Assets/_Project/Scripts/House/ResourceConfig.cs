using UnityEngine;

namespace Game.House
{
    [CreateAssetMenu(menuName = "House/Resource Config", fileName = "ResourceConfig")]
    public sealed class ResourceConfig : ScriptableObject
    {
        [Header("Starting amounts (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Стартовое количество йода у игрока в начале смены.")]
        public int StartingIodine = 10;
        [Tooltip("Стартовое количество лампочек у игрока в начале смены.")]
        public int StartingLightbulbs = 10;
    }
}
