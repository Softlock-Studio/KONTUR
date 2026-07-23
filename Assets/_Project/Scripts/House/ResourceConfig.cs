using UnityEngine;

namespace Game.House
{
    [CreateAssetMenu(menuName = "House/Resource Config", fileName = "ResourceConfig")]
    public sealed class ResourceConfig : ScriptableObject
    {
        [Header("Starting amounts (TBD placeholder values, not GDD-sourced)")]
        public int StartingIodine = 10;
        public int StartingLightbulbs = 10;
    }
}
