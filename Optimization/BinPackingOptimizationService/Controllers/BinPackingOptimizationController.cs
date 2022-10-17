using BinPackingOptimizationService.Interfaces;
using BinPackingOptimizationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace BinPackingOptimizationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BinPackingOptimizationController : ControllerBase
    {
        private readonly ILogger<BinPackingOptimizationController> _logger;
        private readonly IBinPackingOptimization _binPackingOptimizationEngine;

        public BinPackingOptimizationController(
            ILogger<BinPackingOptimizationController> logger,
            IBinPackingOptimization binPackingOptimizationEngine)
        {
            _logger = logger;
            _binPackingOptimizationEngine = binPackingOptimizationEngine;
        }

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
        [HttpPost("DemoKnapsackProblem")]
        public ActionResult<BinPackingResponseDto> SolveKnapsackProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("BinPackingOptimization SolveKnapsackProblem()");
                BinPackingResponseDto result = _binPackingOptimizationEngine.SolveKnapsackProblem();
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
        /// Demo solving a multiple knapsack problem using MIP solver.
        /// <para>
        /// We have five bins each with a capacity of 100.  We want to maximize total packed value.
        /// </para>
        /// </summary>
        /// <returns>
        /// A <see cref="BinPackingResponseDto"/>
        /// </returns>
        [HttpPost("DemoMultipleBinProblemWithMip")]
        public ActionResult<BinPackingResponseDto> SolveMultipleKnapsackProblemWithMip()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("BinPackingOptimization SolveMultipleKnapsackProblemWithMip()");
                BinPackingResponseDto result = _binPackingOptimizationEngine.SolveMultipleKnapsackProblemWithMip();
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

        [HttpPost("DemoMultipleBinProblemWithCpSat")]
        public ActionResult<BinPackingResponseDto> SolveMultipleKnapsackProblemWithCpSat()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("BinPackingOptimization SolveMultipleKnapsackProblemWithCpSat()");
                BinPackingResponseDto result = _binPackingOptimizationEngine.SolveMultipleKnapsackProblemWithCpSat();
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

        [HttpPost("DemoBinPackingProblem")]
        public ActionResult<BinPackingResponseDto> SolveBinPackingProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("BinPackingOptimization SolveBinPackingProblem()");
                BinPackingResponseDto result = _binPackingOptimizationEngine.SolveBinPackingProblem();
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