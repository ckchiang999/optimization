using Microsoft.AspNetCore.Mvc;
using RoutingOptimizationService.Interfaces;
using RoutingOptimizationService.Models;

namespace RoutingOptimizationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoutingOptimizationController : ControllerBase
    {
        private readonly ILogger<RoutingOptimizationController> _logger;
        private readonly IRoutingOptimization _routingOptimizationEngine;

        public RoutingOptimizationController(
            ILogger<RoutingOptimizationController> logger,
            IRoutingOptimization routingOptimizationEngine)
        {
            _logger = logger;
            _routingOptimizationEngine = routingOptimizationEngine;
        }

        /// <summary>
        /// Demo solving Traveling Salesperson Problem (TSP).
        /// In this example, we have 13 locations with New York as the starting point:
        /// New York, Los Angeles, Chicago, Minneapolis, Denver, Dallas, Seattle, Boston, San Francisco, St. Louis, Houston, Phoenix, Salt Lake City
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        [HttpPost("DemoRoutingTravelingSalesPersonProblem")]
        public ActionResult<RoutingResponseDto> SolveTspProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("RoutingOptimization SolveTSP()");
                RoutingResponseDto result = _routingOptimizationEngine.SolveTSP();
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
        /// Demo solving the shortest route for the drilling of hole in a circuit board
        /// with an automated drill.
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        [HttpPost("DemoRoutingCircuitBoardProblem")]
        public ActionResult<RoutingResponseDto> SolveCircuitBoardProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("RoutingOptimization SolveDrillingCircuitBoard()");
                RoutingResponseDto result = _routingOptimizationEngine.SolveDrillingCircuitBoard();
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

        [HttpPost("DemoRoutingVehicleProblem")]
        public ActionResult<RoutingResponseDto> SolveVehicleRoutingProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("RoutingOptimization SolveVehicleRoutingProblem()");
                RoutingResponseDto result = _routingOptimizationEngine.SolveVehicleRoutingProblem();
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