using System.Collections.Generic;
using UnityEngine;

namespace Game.House
{
    [CreateAssetMenu(menuName = "House/Zone Config", fileName = "ZoneConfig")]
    public sealed class ZoneConfig : ScriptableObject
    {
        [Header("Infection growth (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Скорость роста заражения зоны в секунду (в процентных пунктах), пока в зоне активна вспышка заражения.")]
        public float BaseGrowthPerSecond = 0.05f;
        [Tooltip("Дополнительная скорость роста заражения в секунду, прибавляется к базовой, если в зоне выключен свет.")]
        public float DarknessGrowthPerSecond = 0.1f;

        [Header("Treatment activity (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Сколько секунд занимает обработка зоны (устранение заражения) одним сотрудником.")]
        public float TreatmentDurationSeconds = 5f;
        [Tooltip("На сколько процентных пунктов снижается заражение зоны после завершения обработки.")]
        public float TreatmentInfectionReduction = 20f;
        [Tooltip("Сколько йода тратится на одну обработку зоны.")]
        public int TreatmentIodineCost = 1;

        [Header("Lightbulb change activity (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Сколько секунд занимает замена лампочки в зоне.")]
        public float LightbulbChangeDurationSeconds = 5f;
        [Tooltip("Сколько лампочек тратится на одну замену.")]
        public int LightbulbChangeCost = 1;

        [Header("Resident event activity (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Сколько секунд занимает решение события с жильцом (resident event) в этой зоне.")]
        public float ResidentEventDurationSeconds = 5f;

        [Header("Events (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Список случайных событий, которые могут возникать в этой зоне (вспышка заражения, выключение света и т.д.). Настройки каждого события — см. тултипы полей ZoneEventDefinition.")]
        public List<ZoneEventDefinition> Events = new();

        [Header("Multi-worker speedup (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Кривая множителя скорости в зависимости от количества сотрудников, одновременно работающих над одной активностью в зоне. По оси X — число сотрудников, по оси Y — итоговый множитель скорости выполнения активности.")]
        public AnimationCurve WorkerCountSpeedMultiplier = AnimationCurve.Linear(1f, 1f, 4f, 4f);

        public float EvaluateSpeedMultiplier(int activeParticipantCount)
        {
            if (activeParticipantCount <= 0) return 0f;
            return Mathf.Max(0f, WorkerCountSpeedMultiplier.Evaluate(activeParticipantCount));
        }
    }
}
