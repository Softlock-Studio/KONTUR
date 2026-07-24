namespace Game.AI.Employee
{
    // Mirrors the state names registered in EmployeeController.BuildStateMachine exactly — names
    // must match so EmployeeController.StateId can parse CurrentStateName straight into this enum.
    public enum EmployeeStateId
    {
        Idle,
        MovingTo,
        PerformingTask,
        ReturningToBase,
        Fleeing,
    }
}
