namespace Game.Mission
{
    public class DayCycle
    {
        private double _dayDuration;
        private double _nightDuration;
        private TimeType _startTimeType;
        private double _time = 0;
        private bool _isEndDay = false;

        public TimeType TimeType { get; private set; }
        public bool IsEndDay => _isEndDay;
        public double Timer => _time;


        public DayCycle(double dayDuration, double nightDuration, bool isDayStart = true)
        {
            _dayDuration = dayDuration;
            _nightDuration = nightDuration;

            TimeType = isDayStart ? TimeType.Day : TimeType.Night;
            _startTimeType = TimeType;

            ResetCycle();
        }

        public void Update(float deltaTime)
        {
            if (_isEndDay)
                return;

            _time -= deltaTime;

            if (_time <= 0)
            {
                if (TimeType == TimeType.Night)
                    _isEndDay = true;

                TimeType = TimeType.Night;
            }
        }

        public void ResetCycle()
        {
            _time = _startTimeType == TimeType.Day ? _dayDuration : _nightDuration;
            _isEndDay = false;
        }
    }
}