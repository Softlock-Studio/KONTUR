using System.Collections.Generic;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Employee
{
    [CreateAssetMenu(menuName = "AI/Employee Config", fileName = "EmployeeConfig")]
    public sealed class EmployeeConfig : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Скорость передвижения сотрудника при обычном перемещении к цели.")]
        public float MoveSpeed = 3.5f;
        [Tooltip("Скорость передвижения сотрудника при возврате на исходную позицию/точку ожидания.")]
        public float ReturnSpeed = 3.5f;
        [Tooltip("Скорость передвижения сотрудника при бегстве (после нападения бабушки).")]
        public float FleeSpeed = 5f;
        [Tooltip("Расстояние до цели, при котором сотрудник считается «прибывшим» и останавливается.")]
        public float ArrivalThreshold = 0.15f;

        [Header("Locomotion Animation")]
        [Tooltip("Сколько секунд непрерывного движения нужно, чтобы разогнаться от шага до полного бега.")]
        public float AccelerationTime = 3f;
        [Tooltip("Оставшееся расстояние до цели, при котором сотрудник начинает тормозить обратно до шага перед прибытием.")]
        public float BrakingDistance = 4f;

        [Header("Attacked reaction")]
        [Tooltip("Сколько секунд сотрудник остаётся замороженным в состоянии Attacked (анимация реакции на удар), прежде чем ему разрешат бежать — независимо от того, когда бой с бабушкой уже определил исход. Подгоняйте под длительность анимации/последовательности реакции на удар, чтобы бегство её не обрезало.")]
        public float AttackedHoldDurationSeconds = 1.5f;

        [Header("Death")]
        [Tooltip("Включает автоматическое исчезновение тела после смерти сотрудника.")]
        public bool CorpseDespawnEnabled = false;
        [Tooltip("Через сколько секунд после смерти тело исчезает (если CorpseDespawnEnabled включён).")]
        public float CorpseDespawnDelaySeconds = 20f;

        [Tooltip("Замораживает рэгдолл на месте и отключает его столкновения со всем окружением, оставляя его при этом видимым.")]
        public bool CorpseCollisionDisableEnabled = false;
        [Tooltip("Через сколько секунд после смерти отключается коллизия тела (если CorpseCollisionDisableEnabled включён).")]
        public float CorpseCollisionDisableDelaySeconds = 5f;

        [Header("Audio")]
        [Tooltip("Звук при смерти сотрудника.")]
        public SfxCue DeathCue;
        [Tooltip("Звук при бегстве сотрудника.")]
        public SfxCue FleeCue;
        [Tooltip("Звук при получении удара сотрудником.")]
        public SfxCue AttackedCue;

        [Header("Animation variants")]
        [Tooltip("Сколько случайных вариантов клипа заведено в аниматоре для каждого разового действия " +
                 "— перед срабатыванием триггера драйвер бросает Random.Range(0, count) в целочисленный " +
                 "параметр аниматора «Variant». Ставьте столько, сколько вариантов состояний заведено в " +
                 "графе для этого действия (1 = без вариаций, всегда один и тот же клип). Значение для анимации уборки.")]
        public int CleaningVariantCount = 1;
        [Tooltip("Сколько случайных вариантов клипа заведено в аниматоре для анимации замены лампочки — см. подсказку CleaningVariantCount.")]
        public int LightbulbChangeVariantCount = 1;
        [Tooltip("Сколько случайных вариантов клипа заведено в аниматоре для анимации реакции на удар — см. подсказку CleaningVariantCount.")]
        public int AttackedVariantCount = 1;

        [Header("Sounds (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Настройки повторного проигрывания по каждому типу звука сотрудника — см. тултипы полей SoundDefinition (канал, приоритет, интервалы и т.д.).")]
        public List<SoundDefinition<EmployeeSoundType>> Sounds = new();
    }
}
