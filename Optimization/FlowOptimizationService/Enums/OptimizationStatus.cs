namespace FlowOptimizationService.Enums
{
    public enum OptimizationStatus
    {
        Optimal = 0,
        BadInput = 1,
        BadCostRange = 2,
        BadResult = 3,
        Infeasible = 4,
        PossibleOverflow = 5,
        Unbalanced = 6,
        NotSolved = 7,
        Unknown = 8
    }
}
