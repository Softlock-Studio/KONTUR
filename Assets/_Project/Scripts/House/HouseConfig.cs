using UnityEngine;

namespace Game.House
{
    [CreateAssetMenu(menuName = "House/House Config", fileName = "HouseConfig")]
    public class HouseConfig : ScriptableObject
    {
        [Header("Mission Timer")]
        [Tooltip("Длительность игровой смены/ночи в секундах (таймер миссии).")]
        public double DayDurationInSecond;

        // No cross-night progression system exists yet (see Docs/agents/gdd/nights.md) — this is
        // just "which night is this level/scene", set per HouseConfig instance by hand.
        [Header("Night")]
        [Tooltip("Номер игровой ночи для этого уровня/сцены. Пока нет системы прогрессии между ночами — значение задаётся вручную для каждого экземпляра HouseConfig.")]
        public int NightNumber = 1;

        // Target infection range for this level. Checked once, at night end (see
        // MissionManager.Tick) — infection outside [Floor; Ceiling] when the timer runs out is a
        // defeat, same as the existing "hit 100%" hard cap but evaluated only at day-end rather
        // than instantly. No cross-night escalation yet — tune per level by hand, same as NightNumber.
        [Header("Infection Corridor")]
        [Tooltip("Нижняя граница допустимого заражения дома (0–1) на конец ночи. Проверяется один раз, когда истекает таймер миссии: если итоговое заражение ниже этого значения — поражение.")]
        [Range(0f, 1f)] public float InfectionFloor01 = 0.2f;
        [Tooltip("Верхняя граница допустимого заражения дома (0–1) на конец ночи. Если итоговое заражение выше этого значения — поражение (аналогично старому правилу «дошли до 100%», но проверяется только в конце дня, а не мгновенно).")]
        [Range(0f, 1f)] public float InfectionCeiling01 = 0.4f;
    }
}
