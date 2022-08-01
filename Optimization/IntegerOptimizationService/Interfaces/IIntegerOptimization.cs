using IntegerOptimizationService.Models;

namespace IntegerOptimizationService.Interfaces
{
    public interface IIntegerOptimization
    {
        IntegerResponseDto Solve(IntegerProblemDto problem);
    }
}
