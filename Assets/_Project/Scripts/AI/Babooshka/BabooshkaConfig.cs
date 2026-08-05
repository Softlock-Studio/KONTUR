using System.Collections.Generic;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Babooshka
{
    [CreateAssetMenu(menuName = "AI/Babooshka Config", fileName = "BabooshkaConfig")]
    public sealed class BabooshkaConfig : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Скорость передвижения бабушки в режиме патрулирования.")]
        public float PatrolSpeed = 1.5f;
        [Tooltip("Скорость передвижения бабушки в режиме погони.")]
        public float ChaseSpeed = 3f;

        [Header("Wander")]
        [Tooltip("Минимальное время (в секундах), которое бабушка стоит на месте во время блуждания (Wander).")]
        public float WanderStandStillMinSeconds = 1.5f;
        [Tooltip("Максимальное время (в секундах), которое бабушка стоит на месте во время блуждания (Wander).")]
        public float WanderStandStillMaxSeconds = 5f;
        [Tooltip("Вероятность (0–1), что во время блуждания бабушка зайдёт в квартиру.")]
        [Range(0f, 1f)] public float ApartmentVisitChance = 0.3f;
        [Tooltip("Бросается один раз на каждую остановку внутри квартиры.")]
        [Range(0f, 1f)] public float WallLickChance = 0.35f;
        [Tooltip("Как долго бабушка стоит на месте, облизывая стену, прежде чем продолжить путь — подгоняйте примерно под длину анимационного клипа WallLick.")]
        public float WallLickDuration = 2f;
        [Tooltip("Бросается один раз на каждую остановку внутри квартиры, только если бросок на облизывание стены выше не сработал (не больше одного «жуткого события» за визит) — реальный шанс равен (1 - WallLickChance) * LightOffChance, а не этому значению напрямую.")]
        [Range(0f, 1f)] public float LightOffChance = 0.2f;
        [Tooltip("Как долго бабушка стоит на месте, тянясь к выключателю, прежде чем продолжить путь — подгоняйте примерно под длину анимационного клипа LightOff.")]
        public float LightOffDuration = 1.5f;

        [Header("Senses")]
        [Tooltip("Радиус зрения бабушки, в метрах.")]
        public float SightRadius = 10f;
        [Tooltip("Угол обзора бабушки (конус зрения), в градусах.")]
        [Range(0f, 360f)] public float SightAngle = 110f;
        [Tooltip("Базовый радиус слуха бабушки, в метрах.")]
        public float HearingRadius = 12f;
        [Tooltip("Окно времени (в секундах), за которое бабушка успевает среагировать на услышанный звук.")]
        public float HearingReactionWindow = 0.3f;
        [Tooltip("Слой (Layer), на котором находятся сотрудники — используется для проверок зрения бабушки.")]
        public LayerMask EmployeeLayer;
        [Tooltip("Слои, которые считаются препятствиями для линии видимости (Raycast) бабушки.")]
        public LayerMask ObstacleMask;

        [Header("Hearing — loudness scaling (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Множитель HearingRadius для тихих звуков (SoundLoudness.Low).")]
        public float LowLoudnessRadiusMultiplier = 0.5f;
        [Tooltip("Множитель HearingRadius для звуков средней громкости (SoundLoudness.Medium).")]
        public float MediumLoudnessRadiusMultiplier = 1f;
        [Tooltip("Множитель HearingRadius для громких звуков (SoundLoudness.High).")]
        public float HighLoudnessRadiusMultiplier = 1.75f;

        [Header("Hearing — cross-floor (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Разница по высоте (Y), в пределах которой звук считается идущим с того же этажа, что и датчик бабушки. Отдельной системы отслеживания этажей пока нет, это временная замена понятию «тот же этаж».")]
        public float SameFloorHeightTolerance = 2.5f;
        [Tooltip("Множитель эффективного радиуса слуха для звуков с другого этажа — держите меньше 1, чтобы такие звуки было сложнее заметить, чем звуки с того же этажа.")]
        [Range(0f, 1f)] public float DifferentFloorRadiusMultiplier = 0.35f;

        public float GetHearingRadius(SoundLoudness loudness, bool sameFloor)
        {
            float multiplier = loudness switch
            {
                SoundLoudness.Low => LowLoudnessRadiusMultiplier,
                SoundLoudness.High => HighLoudnessRadiusMultiplier,
                _ => MediumLoudnessRadiusMultiplier,
            };

            float radius = HearingRadius * multiplier;
            if (!sameFloor) radius *= DifferentFloorRadiusMultiplier;
            return radius;
        }

        [Header("Aggression")]
        [Tooltip("Вероятность (0–1), что бабушка отреагирует на впервые замеченного сотрудника и начнёт погоню. 0 = никогда не нападает, 1 = нападает всегда (как раньше), среднее значение = нападает иногда. Бросок происходит один раз на каждого новозамеченного сотрудника и не повторяется, пока он остаётся в поле зрения непрерывно — потеря видимости сбрасывает решение, так что при следующей встрече (даже с тем же сотрудником) шанс кидается заново.")]
        [Range(0f, 1f)] public float AggressionChance01 = 1f;

        [Header("Fight")]
        [Tooltip("Дистанция, с которой бабушка может атаковать сотрудника.")]
        public float AttackRange = 1.5f;
        [Tooltip("Сколько секунд бабушка и атакованный сотрудник остаются замороженными в анимациях атаки/реакции, прежде чем применяется исход «выжил/погиб».")]
        public float AttackReactionDuration = 1f;
        [Tooltip("Сколько секунд бабушка задерживается после применения исхода боя (смерть/бегство), прежде чем вернуться к блужданию.")]
        public float FightResolutionDuration = 1.5f;
        [Tooltip("Сколько секунд бабушка проверяет место подозрительного звука/события, прежде чем сдаться и вернуться к блужданию.")]
        public float InvestigateTimeout = 6f;
        [Tooltip("Сколько секунд выживший сотрудник невидим для SightSensor сразу после боя, чтобы у него был шанс сбежать вместо мгновенного повторного столкновения.")]
        public float PostFightMercyDuration = 3f;

        [Header("Death chance")]
        [Tooltip("Кривая вероятности смерти сотрудника в бою в зависимости от текущего заражения дома. По оси X — заражение (0–1), по оси Y — вероятность смерти (0–1).")]
        public AnimationCurve DeathChanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Audio")]
        [Tooltip("Звук при облизывании стены.")]
        public SfxCue WallLickCue;
        [Tooltip("Звук при выключении света.")]
        public SfxCue LightOffCue;
        [Tooltip("Звук при атаке.")]
        public SfxCue AttackCue;

        [Header("Sounds (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Настройки повторного проигрывания по каждому типу звука бабушки — см. тултипы полей SoundDefinition. Footstep/Laugh/Anger повторяются периодически; у Attack есть свой отдельный разовый AttackCue выше, он не периодический.")]
        public List<SoundDefinition<BabooshkaSoundType>> Sounds = new();

        [Header("Debug")]
        [Tooltip("Гизмо (конус зрения, радиус слуха, подпись состояния) и логи в консоль для этой бабушки.")]
        public bool EnableDebugVisuals = false;

        public float ResolveDeathChance(float infection)
        {
            return DeathChanceCurve.Evaluate(Mathf.Clamp01(infection));
        }
    }
}
