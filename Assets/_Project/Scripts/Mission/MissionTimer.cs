namespace Game.Mission
{
    public class MissionTimer
    {
        private readonly double duration;
        private double timeRemaining;
        private bool isEndOfDay;

        public double TimeRemaining => timeRemaining;
        public bool IsEndOfDay => isEndOfDay;

        public MissionTimer(double durationInSeconds)
        {
            duration = durationInSeconds;
            Reset();
        }

        public void Update(float deltaTime)
        {
            if (isEndOfDay) return;

            timeRemaining -= deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isEndOfDay = true;
            }
        }

        public void Reset()
        {
            timeRemaining = duration;
            isEndOfDay = false;
        }
    }
}
