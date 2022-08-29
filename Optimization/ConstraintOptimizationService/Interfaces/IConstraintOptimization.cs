using ConstraintOptimizationService.Models;

namespace ConstraintOptimizationService.Interfaces
{
    public interface IConstraintOptimization
    {
        /// <summary>
        /// Find the first solution to a simple constraint optimization problem 
        /// </summary>
        /// <returns>
        /// A <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/> with the solution
        /// </returns>
        ConstraintResponseDto SolveDemoFirstSolution();

        /// <summary>
        /// Find all the solutions to a simple constraint optimization problem
        /// </summary>
        /// <returns>
        /// A <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/> with all the solutions
        /// </returns>
        ConstraintResponseDto SolveDemoAllSolutions();

        /// <summary>
        /// Demo solving a CP problem for optimal
        /// <para>Maximize 2x + 2y + 3z subject to the following constraints:</para>
        /// <code>
        /// x + (7/2)y + (3/2)z &lt;= 25
        /// 3x - 5y + 7z &lt;= 45
        /// 5x + 2y - 6zx &gt;= 37
        /// x, y, z &gt;= 0
        /// x, y, z integers
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="IntegerOptimizationService.Models.IntegerResponseDto"/>
        /// </returns>
        ConstraintResponseDto SolveCpOptimization();

        /// <summary>
        /// Demo solving a cryptarithmetic problem using CP solver and AllDifferent constraint.
        /// This is a puzzle where digits are represented by letters and
        /// each letter represents a different digit.
        /// <para>Find the digits such that:</para>
        /// <code>
        ///     CP
        /// +   IS
        /// +  FUN
        /// ------
        /// = TRUE
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/>
        /// </returns>
        ConstraintResponseDto SolveCryptarithmetic();

        /// <summary>
        /// Demo solving the N-Queens problem using CP solver.
        /// This is a combinatorial problem based on the game of chess.
        /// <para>How can N queens be placed on an NxN chessboard so that no two of them attack each other?</para>
        /// </summary>
        /// <param name="boardSize">Board size</param>
        /// <param name="solutionLimit">Sets the maximum number of solutions to find and then stop searching after it reaches this limit</param>
        /// <returns>
        /// An <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/>
        /// </returns>
        ConstraintResponseDto SolveNQueens(int boardSize, int? solutionLimit = null);
    }
}
