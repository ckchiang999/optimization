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

        /// <summary>
        /// Demo solving a Capacitated Vehicle Routing Problem (CVRP).  Given multiple vehicles and multiple sets of locations to visit, 
        /// find the optimal route, defined here as minimizing the length of the longest single route among all vehicles.
        /// <para>
        /// In this problem, we have 17 locations and 4 vehicles.
        /// </para>
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        RoutingResponseDto SolveVehicleRoutingProblem();

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
        RoutingResponseDto SolveCapacitatedVehicleRoutingProblem();

        /// <summary>
        /// Demo solving a Vehicle Routing Problem (VRP) with pick-ups and deliveries.
        /// Given multiple vehicles and multiple pairs of pick-up and delivery locations,
        /// minimize the distance of the longest route.  A pick-up delivery pair means
        /// the pick-up location must be visited before the delivery location.
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        RoutingResponseDto SolvePickupDeliveryProblem();

        /// <summary>
        /// Demo solving a Vehicle Routing Problem (VRP) with time windows.
        /// Given multiple vehicles and a time window available to each location,
        /// minimize the total travel time of the vehicles.
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        RoutingResponseDto SolveTimeWindowVehicleRoutingProblem();
    }
}
