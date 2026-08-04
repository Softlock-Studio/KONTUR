using System;
using UnityEngine;

namespace Game.House
{
    [Serializable]
    public sealed class ZoneEventDefinition
    {
        [Tooltip("Тип события зоны — какое именно происшествие описывает эта запись.")]
        public ZoneEventType Type;
        [Tooltip("Вид события. Instant — разовое происшествие, может случаться повторно до MaxConcurrent раз одновременно. Condition — булево состояние (включено/выключено), не может сработать повторно, пока уже активно.")]
        public ZoneEventKind Kind = ZoneEventKind.Instant;
        [Tooltip("Как часто (в секундах) проверяется, не пора ли заспавнить это событие.")]
        public float CheckIntervalSeconds = 60f;

        [Range(0f, 1f)]
        [Tooltip("Вероятность (0–1), что событие произойдёт при каждой проверке (раз в CheckIntervalSeconds).")]
        public float SpawnChance = 0.1f;

        [Tooltip("Сколько экземпляров этого события может быть активно одновременно. Игнорируется, если Kind = Condition — событие-условие это просто булев флаг, а не счётчик.")]
        public int MaxConcurrent = 1;

        [Tooltip("<= 0 означает, что событие никогда не истекает само по себе. > 0 запускает обратный отсчёт в момент, когда событие становится активным (и для Instant, и для Condition); если оно не будет решено до истечения времени, это засчитывается как проваленная задача.")]
        public float ExpirySeconds = 0f;
    }
}
