using RoutingOptimizationService.Models;

namespace RoutingOptimizationService.Interfaces
{
    public interface IRoutingOptimization
    {
        /// <summary>
        /// Demo solving Traveling Salesperson Problem (TSP).
        /// In this example, we have 13 locations with New York as the starting point:
        /// New York, Los Angeles, Chicago, Minneapolis, Denver, Dallas, Seattle, Boston, San Francisco, St. Louis, Houston, Phoenix, Salt Lake City
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        RoutingResponseDto SolveTSP();

        /// <summary>
        /// Demo solving the shortest route for the drilling of hole in a circuit board
        /// with an automated drill.
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        RoutingResponseDto SolveDrillingCircuitBoard();

        RoutingResponseDto SolveVehicleRoutingProblem();
    }
}
