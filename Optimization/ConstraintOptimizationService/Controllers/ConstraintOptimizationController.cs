using ConstraintOptimizationService.Interfaces;
using ConstraintOptimizationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConstraintOptimizationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConstraintOptimizationController : ControllerBase
    { 
        private readonly ILogger<ConstraintOptimizationController> _logger;
        private readonly IConstraintOptimization _constraintOptimizationEngine;

        public ConstraintOptimizationController(
            ILogger<ConstraintOptimizationController> logger,
            IConstraintOptimization constraintOptimizationEngine)
        {
            _logger = logger;
            _constraintOptimizationEngine = constraintOptimizationEngine;
        }

        /// <summary>
        /// Find the first solution to a simple constraint optimization problem 
        /// </summary>
        /// <returns>
        /// A <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/> with the solution
        /// </returns>
        [HttpPost("DemoFirstSolution")]
        public ActionResult<ConstraintResponseDto> SolveDemoFirstSolution()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("ConstraintOptimization SolveDemoFirstSolution()");
                ConstraintResponseDto result = _constraintOptimizationEngine.SolveDemoFirstSolution();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw;
            }
            finally
            {
                logScope?.Dispose();
            }
        }

        /// <summary>
        /// Find all the solutions to a simple constraint optimization problem
        /// </summary>
        /// <returns>
        /// A <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/> with all the solutions
        /// </returns>
        [HttpPost("DemoAllSolutions")]
        public ActionResult<ConstraintResponseDto> SolveDemoAllSolutions()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("ConstraintOptimization SolveDemoAllSolutions()");
                ConstraintResponseDto result = _constraintOptimizationEngine.SolveDemoAllSolutions();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw;
            }
            finally
            {
                logScope?.Dispose();
            }
        }

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
        /// An <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/>
        /// </returns>
        [HttpPost("CpDemo")]
        public ActionResult<ConstraintResponseDto> SolveCpOptimization()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("ConstraintOptimization SolveCpOptimization()");
                ConstraintResponseDto result = _constraintOptimizationEngine.SolveCpOptimization();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw;
            }
            finally
            {
                logScope?.Dispose();
            }
        }

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
        [HttpPost("CryptarithmeticDemo")]
        public ActionResult<ConstraintResponseDto> SolveCryptarithmetic()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("ConstraintOptimization SolveCryptarithmetic()");
                ConstraintResponseDto result = _constraintOptimizationEngine.SolveCryptarithmetic();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw;
            }
            finally
            {
                logScope?.Dispose();
            }
        }

        /// <summary>
        /// Demo solving the N-Queens problem using CP solver.
        /// This is a combinatorial problem based on the game of chess.
        /// The CP approach uses propagation and backtracking strategy to solve the problem.
        /// Note that the number of solutions goes up roughly exponentially with the size of the board.
        /// <para>How can N queens be placed on an NxN chessboard so that no two of them attack each other?</para>
        /// </summary>
        /// <param name="boardSize">Board size</param>
        /// <param name="solutionLimit">Sets the maximum number of solutions to find and then stop searching after it reaches this limit</param>
        /// <returns>
        /// An <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/>
        /// </returns>
        [HttpPost("NqueensDemo")]
        public ActionResult<ConstraintResponseDto> SolveNQueens(int boardSize, int? solutionLimit = null)
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("ConstraintOptimization SolveCryptarithmetic()");
                ConstraintResponseDto result = _constraintOptimizationEngine.SolveNQueens(boardSize, solutionLimit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw;
            }
            finally
            {
                logScope?.Dispose();
            }
        }
    }
}