using BinPackingOptimizationService.Models;

namespace BinPackingOptimizationService.Interfaces
{
    public interface IBinPackingOptimization
    {
        /// <summary>
        /// Demo solving the Knapsack Problem.
        /// <para>
        /// We are packing a set of items with given values and sizes into a container with a maximum capacity.
        /// The problem is to choose a subset of items with maximum total value that will fit in the container.
        /// </para>
        /// </summary>
        /// <returns>
        /// A <see cref="BinPackingResponseDto"/>
        /// </returns>
        BinPackingResponseDto SolveKnapsackProblem();

        /// <summary>
        /// Demo solving a multiple knapsack problem using MIP solver.
        /// <para>
        /// We have five bins each with a capacity of 100.  We want to maximize total packed value.
        /// </para>
        /// </summary>
        /// <returns>
        /// A <see cref="BinPackingResponseDto"/>
        /// </returns>
        BinPackingResponseDto SolveMultipleKnapsackProblemWithMip();
    }
}
