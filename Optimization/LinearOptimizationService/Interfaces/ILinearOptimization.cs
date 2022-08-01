using LinearOptimizationService.Models;

namespace LinearOptimizationService
{
    public interface ILinearOptimization
    {
        LinearResponseDto SolveDemo();
        LinearResponseDto Solve(LinearProblemDto problem);

        LinearResponseDto SolveStiglerDietProblemDemo();
    }
}
