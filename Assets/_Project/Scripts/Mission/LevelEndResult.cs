namespace Game.Mission
{
    // Snapshot of how a night ended — payload of MissionManager.LevelEnded, consumed by
    // ResultsScreenPresenter to drive IResultsScreenView.Show.
    public readonly struct LevelEndResult
    {
        public bool IsVictory { get; }
        public float MaxInfectionReached01 { get; }
        public int EmployeesKilled { get; }

        public LevelEndResult(bool isVictory, float maxInfectionReached01, int employeesKilled)
        {
            IsVictory = isVictory;
            MaxInfectionReached01 = maxInfectionReached01;
            EmployeesKilled = employeesKilled;
        }
    }
}
