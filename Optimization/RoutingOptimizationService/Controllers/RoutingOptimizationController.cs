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

        /// <summary>
        /// Demo solving a Vehicle Routing Problem (VRP).  Given multiple vehicles and multiple sets of locations to visit, 
        /// find the optimal route, which we define as minimizing the length of the longest single route among all vehicles.
        /// <para>
        /// In this problem, we have 17 locations and 4 vehicles.
        /// </para>
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
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

        /// <summary>
        /// Demo solving a Vehicle Routing Problem (VRP).  Given multiple vehicles, multiple sets of locations to visit, 
        /// a carrying capacity for each vehicle, find the optimal route, defined here as minimizing the length of the longest single route
        /// among all vehicles.
        /// <para>
        /// In this problem, we have 17 locations and 4 vehicles.
        /// </para>
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        [HttpPost("DemoCapacitatedRoutingVehicleProblem")]
        public ActionResult<RoutingResponseDto> SolveCapacitatedVehicleRoutingProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("RoutingOptimization SolveCapacitatedVehicleRoutingProblem()");
                RoutingResponseDto result = _routingOptimizationEngine.SolveCapacitatedVehicleRoutingProblem();
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
        /// Demo solving a Vehicle Routing Problem (VRP) with pick-ups and deliveries.
        /// Given multiple vehicles and multiple pairs of pick-up and delivery locations,
        /// minimize the distance of the longest route.  A pick-up delivery pair means
        /// the pick-up location must be visited before the delivery location.
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        [HttpPost("DemoPickupDeliveryProblem")]
        public ActionResult<RoutingResponseDto> SolvePickupDeliveryProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("RoutingOptimization SolvePickupDeliveryProblem()");
                RoutingResponseDto result = _routingOptimizationEngine.SolvePickupDeliveryProblem();
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
        /// Demo solving a Vehicle Routing Problem (VRP) with time windows.
        /// Given multiple vehicles and a time window available to each location,
        /// minimize the total travel time of the vehicles.
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        [HttpPost("DemoTimeWindowProblem")]
        public ActionResult<RoutingResponseDto> SolveTimeWindowProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("RoutingOptimization SolveTimeWindowVehicleRoutingProblem()");
                RoutingResponseDto result = _routingOptimizationEngine.SolveTimeWindowVehicleRoutingProblem();
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
        /// Demo solving a Vehicle Routing Problem (VRP) with time windows and resource constraints.
        /// Given multiple vehicles, a time window available to each location, 
        /// constraints on loading and unloading times at each location, and
        /// a maximum number of vehicles that can load and unload at each location,
        /// minimize the total travel time of the vehicles.
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        [HttpPost("DemoResourceConstrainedTimeWindowProblem")]
        public ActionResult<RoutingResponseDto> SolveResourceConstrainedTimeWindowProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("RoutingOptimization SolveResourceConstrainedTimeWindowVehicleRoutingProblem()");
                RoutingResponseDto result = _routingOptimizationEngine.SolveResourceConstrainedTimeWindowVehicleRoutingProblem();
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

        [HttpPost("DemoPenaltyDefinedProblem")]
        public ActionResult<RoutingResponseDto> SolvePenaltyDefinedProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("RoutingOptimization SolvePenaltyDefinedVehicleRoutingProblem()");
                RoutingResponseDto result = _routingOptimizationEngine.SolvePenaltyDefinedVehicleRoutingProblem();
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