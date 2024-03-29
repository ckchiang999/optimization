﻿namespace AssignmentOptimizationService
{
    public enum OptimizationStatus
    {
        Optimal = 0,
        Feasible = 1,
        Infeasible = 2,
        Unbounded = 3,
        Abnormal = 4,
        NotSolved = 6,
        ModelInvalid = 7,
        Unknown = 8,
        PossibleOverflow = 9
    }
}
