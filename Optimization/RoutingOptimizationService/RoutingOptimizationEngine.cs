using System.Diagnostics;
using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;
using RoutingOptimizationService.Helpers;
using RoutingOptimizationService.Interfaces;
using RoutingOptimizationService.Models;

namespace RoutingOptimizationService
{
    public class RoutingOptimizationEngine : IRoutingOptimization
    {
        /// <summary>
        /// Demo solving Traveling Salesperson Problem (TSP).
        /// In this example, we have 13 locations with New York as the starting point:
        /// New York, Los Angeles, Chicago, Minneapolis, Denver, Dallas, Seattle, Boston, San Francisco, St. Louis, Houston, Phoenix, Salt Lake City
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        public RoutingResponseDto SolveTSP()
        {
            RoutingResponseDto response = new();

            // Define the names of the locations
            string[] locations =
            {
                "New York",
                "Los Angeles",
                "Chicago",
                "Minneapolis",
                "Denver",
                "Dallas",
                "Seattle",
                "Boston",
                "San Francisco",
                "St. Louis",
                "Houston",
                "Phoenix",
                "Salt Lake City"
            };

            // Define the distance matrix
            // distanceMatrix[i, j] is the distance from location i to location j in miles
            long[,] distanceMatrix =
            {
                { 0, 2451, 713, 1018, 1631, 1374, 2408, 213, 2571, 875, 1420, 2145, 1972 },
                { 2451, 0, 1745, 1524, 831, 1240, 959, 2596, 403, 1589, 1374, 357, 579 },
                { 713, 1745, 0, 355, 920, 803, 1737, 851, 1858, 262, 940, 1453, 1260 },
                { 1018, 1524, 355, 0, 700, 862, 1395, 1123, 1584, 466, 1056, 1280, 987 },
                { 1631, 831, 920, 700, 0, 663, 1021, 1769, 949, 796, 879, 586, 371 },
                { 1374, 1240, 803, 862, 663, 0, 1681, 1551, 1765, 547, 225, 887, 999 },
                { 2408, 959, 1737, 1395, 1021, 1681, 0, 2493, 678, 1724, 1891, 1114, 701 },
                { 213, 2596, 851, 1123, 1769, 1551, 2493, 0, 2699, 1038, 1605, 2300, 2099 },
                { 2571, 403, 1858, 1584, 949, 1765, 678, 2699, 0, 1744, 1645, 653, 600 },
                { 875, 1589, 262, 466, 796, 547, 1724, 1038, 1744, 0, 679, 1272, 1162 },
                { 1420, 1374, 940, 1056, 879, 225, 1891, 1605, 1645, 679, 0, 1017, 1200 },
                { 2145, 357, 1453, 1280, 586, 887, 1114, 2300, 653, 1272, 1017, 0, 504 },
                { 1972, 579, 1260, 987, 371, 999, 701, 2099, 600, 1162, 1200, 504, 0 }
            };

            const int locationNumber = 13;

            // Define how many vehicles are in the problem.  1 for TSP.
            const int vehicleNumber = 1;

            // Depot is the starting location
            const int depot = 0;

            // Create the routing index manager
            // The manager does conversions of the solver's internal indices to the numbers for locations.
            var manager = new RoutingIndexManager(locationNumber, vehicleNumber, depot);

            // Create the routing modexl
            var routing = new RoutingModel(manager);

            int transitCallBackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable index to distance matrix NodeIndex
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return distanceMatrix[fromNode, toNode];
                });

            // Define cost of each arc/edge/route
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallBackIndex);

            // Setting the first solution heuristic
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

            // Solve the problem
            Assignment solution = routing.SolveWithParameters(searchParameters);

            Debug.WriteLine($"Objective: {solution.ObjectiveValue()}");
            Debug.WriteLine("Route:");
            long routeDistance = 0;
            var index = routing.Start(0);
            var route = new RouteResponseDto();
            response.Routes.Add(route);
            while (!routing.IsEnd(index))
            {
                var fromLocationNode = manager.IndexToNode((int)index);
                Debug.Write($"{fromLocationNode} -> ");
                var previousIndex = index;
                index = solution.Value(routing.NextVar(index));
                var toLocationNode = manager.IndexToNode((int)index);
                routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                route.Locations.Add(
                    new LocationResponseDto
                    {
                        FromLocation = locations[fromLocationNode],
                        ToLocation = locations[toLocationNode],
                        Distance = routeDistance
                    });
                route.TotalDistance = routeDistance;
            }
            response.TotalDistance = routeDistance;
            response.MaximumDistance = routeDistance;

            Debug.WriteLine($"{manager.IndexToNode((int)index)}");
            Debug.WriteLine($"Route distance: {routeDistance} miles");

            return response;
        }

        /// <summary>
        /// Demo solving the shortest route for the drilling of hole in a circuit board
        /// with an automated drill.
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        public RoutingResponseDto SolveDrillingCircuitBoard()
        {
            RoutingResponseDto response = new();

            // 280 holes
            int[,] holeLocations = 
            {
                { 288, 149 }, { 288, 129 }, { 270, 133 }, { 256, 141 }, { 256, 157 }, { 246, 157 }, { 236, 169 },
                { 228, 169 }, { 228, 161 }, { 220, 169 }, { 212, 169 }, { 204, 169 }, { 196, 169 }, { 188, 169 },
                { 196, 161 }, { 188, 145 }, { 172, 145 }, { 164, 145 }, { 156, 145 }, { 148, 145 }, { 140, 145 },
                { 148, 169 }, { 164, 169 }, { 172, 169 }, { 156, 169 }, { 140, 169 }, { 132, 169 }, { 124, 169 },
                { 116, 161 }, { 104, 153 }, { 104, 161 }, { 104, 169 }, { 90, 165 },  { 80, 157 },  { 64, 157 },
                { 64, 165 },  { 56, 169 },  { 56, 161 },  { 56, 153 },  { 56, 145 },  { 56, 137 },  { 56, 129 },
                { 56, 121 },  { 40, 121 },  { 40, 129 },  { 40, 137 },  { 40, 145 },  { 40, 153 },  { 40, 161 },
                { 40, 169 },  { 32, 169 },  { 32, 161 },  { 32, 153 },  { 32, 145 },  { 32, 137 },  { 32, 129 },
                { 32, 121 },  { 32, 113 },  { 40, 113 },  { 56, 113 },  { 56, 105 },  { 48, 99 },   { 40, 99 },
                { 32, 97 },   { 32, 89 },   { 24, 89 },   { 16, 97 },   { 16, 109 },  { 8, 109 },   { 8, 97 },
                { 8, 89 },    { 8, 81 },    { 8, 73 },    { 8, 65 },    { 8, 57 },    { 16, 57 },   { 8, 49 },
                { 8, 41 },    { 24, 45 },   { 32, 41 },   { 32, 49 },   { 32, 57 },   { 32, 65 },   { 32, 73 },
                { 32, 81 },   { 40, 83 },   { 40, 73 },   { 40, 63 },   { 40, 51 },   { 44, 43 },   { 44, 35 },
                { 44, 27 },   { 32, 25 },   { 24, 25 },   { 16, 25 },   { 16, 17 },   { 24, 17 },   { 32, 17 },
                { 44, 11 },   { 56, 9 },    { 56, 17 },   { 56, 25 },   { 56, 33 },   { 56, 41 },   { 64, 41 },
                { 72, 41 },   { 72, 49 },   { 56, 49 },   { 48, 51 },   { 56, 57 },   { 56, 65 },   { 48, 63 },
                { 48, 73 },   { 56, 73 },   { 56, 81 },   { 48, 83 },   { 56, 89 },   { 56, 97 },   { 104, 97 },
                { 104, 105 }, { 104, 113 }, { 104, 121 }, { 104, 129 }, { 104, 137 }, { 104, 145 }, { 116, 145 },
                { 124, 145 }, { 132, 145 }, { 132, 137 }, { 140, 137 }, { 148, 137 }, { 156, 137 }, { 164, 137 },
                { 172, 125 }, { 172, 117 }, { 172, 109 }, { 172, 101 }, { 172, 93 },  { 172, 85 },  { 180, 85 },
                { 180, 77 },  { 180, 69 },  { 180, 61 },  { 180, 53 },  { 172, 53 },  { 172, 61 },  { 172, 69 },
                { 172, 77 },  { 164, 81 },  { 148, 85 },  { 124, 85 },  { 124, 93 },  { 124, 109 }, { 124, 125 },
                { 124, 117 }, { 124, 101 }, { 104, 89 },  { 104, 81 },  { 104, 73 },  { 104, 65 },  { 104, 49 },
                { 104, 41 },  { 104, 33 },  { 104, 25 },  { 104, 17 },  { 92, 9 },    { 80, 9 },    { 72, 9 },
                { 64, 21 },   { 72, 25 },   { 80, 25 },   { 80, 25 },   { 80, 41 },   { 88, 49 },   { 104, 57 },
                { 124, 69 },  { 124, 77 },  { 132, 81 },  { 140, 65 },  { 132, 61 },  { 124, 61 },  { 124, 53 },
                { 124, 45 },  { 124, 37 },  { 124, 29 },  { 132, 21 },  { 124, 21 },  { 120, 9 },   { 128, 9 },
                { 136, 9 },   { 148, 9 },   { 162, 9 },   { 156, 25 },  { 172, 21 },  { 180, 21 },  { 180, 29 },
                { 172, 29 },  { 172, 37 },  { 172, 45 },  { 180, 45 },  { 180, 37 },  { 188, 41 },  { 196, 49 },
                { 204, 57 },  { 212, 65 },  { 220, 73 },  { 228, 69 },  { 228, 77 },  { 236, 77 },  { 236, 69 },
                { 236, 61 },  { 228, 61 },  { 228, 53 },  { 236, 53 },  { 236, 45 },  { 228, 45 },  { 228, 37 },
                { 236, 37 },  { 236, 29 },  { 228, 29 },  { 228, 21 },  { 236, 21 },  { 252, 21 },  { 260, 29 },
                { 260, 37 },  { 260, 45 },  { 260, 53 },  { 260, 61 },  { 260, 69 },  { 260, 77 },  { 276, 77 },
                { 276, 69 },  { 276, 61 },  { 276, 53 },  { 284, 53 },  { 284, 61 },  { 284, 69 },  { 284, 77 },
                { 284, 85 },  { 284, 93 },  { 284, 101 }, { 288, 109 }, { 280, 109 }, { 276, 101 }, { 276, 93 },
                { 276, 85 },  { 268, 97 },  { 260, 109 }, { 252, 101 }, { 260, 93 },  { 260, 85 },  { 236, 85 },
                { 228, 85 },  { 228, 93 },  { 236, 93 },  { 236, 101 }, { 228, 101 }, { 228, 109 }, { 228, 117 },
                { 228, 125 }, { 220, 125 }, { 212, 117 }, { 204, 109 }, { 196, 101 }, { 188, 93 },  { 180, 93 },
                { 180, 101 }, { 180, 109 }, { 180, 117 }, { 180, 125 }, { 196, 145 }, { 204, 145 }, { 212, 145 },
                { 220, 145 }, { 228, 145 }, { 236, 145 }, { 246, 141 }, { 252, 125 }, { 260, 129 }, { 280, 133 },
            };

            // Define how many vehicles/drills are in the problem.  1 for TSP.
            const int vehicleNumber = 1;

            // Depot is the starting location
            const int depot = 0;

            // Convert all the hole locations to a matrix of distances between each hole
            long[,] distanceMatrix = DistanceCalculator.ComputeEuclideanDistanceMatrix(holeLocations);

            // Create the routing index manager
            // The manager does conversions of the solver's internal indices to the numbers for locations.
            var manager = new RoutingIndexManager(holeLocations.GetLength(0), vehicleNumber, depot);

            // Create the routing modexl
            var routing = new RoutingModel(manager);

            int transitCallBackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable index to distance matrix NodeIndex
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return distanceMatrix[fromNode, toNode];
                });

            // Define cost of each arc/edge/route
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallBackIndex);

            // Setting the first solution heuristic
            // This does not always return the optimal solution.
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            // Set guided local search for finding a local minimum, which will be a better solution.
            // Must set TimeLimit when using guided local search.
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Duration { Seconds = 30 };
            searchParameters.LogSearch = true;
            

            // Solve the problem
            Assignment solution = routing.SolveWithParameters(searchParameters);

            Debug.WriteLine($"Objective: {solution.ObjectiveValue()}");
            Debug.WriteLine("Route:");
            long routeDistance = 0;
            var index = routing.Start(0);
            var route = new RouteResponseDto();
            response.Routes.Add(route);
            while (!routing.IsEnd(index))
            {
                var fromLocationNode = manager.IndexToNode((int)index);
                Debug.Write($"{fromLocationNode} -> ");
                var previousIndex = index;
                index = solution.Value(routing.NextVar(index));
                var toLocationNode = manager.IndexToNode((int)index);
                routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                route.Locations.Add(
                    new LocationResponseDto
                    {
                        FromLocation = $"({holeLocations[fromLocationNode, 0]},{holeLocations[fromLocationNode, 1]})",
                        ToLocation = $"({holeLocations[toLocationNode, 0]},{holeLocations[toLocationNode, 1]})",
                        Distance = routeDistance
                    });
                route.TotalDistance = routeDistance;
            }
            response.TotalDistance = routeDistance;
            response.MaximumDistance = routeDistance;

            Debug.WriteLine($"{manager.IndexToNode((int)index)}");
            Debug.WriteLine($"Route distance: {routeDistance}");

            return response;
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
        public RoutingResponseDto SolveVehicleRoutingProblem()
        {
            RoutingResponseDto response = new();

            int[,] locations = 
            {
                { 456, 320 },  // location 0
                { 228, 0 },    // location 1
                { 912, 0 },    // location 2
                { 0, 80 },     // location 3
                { 114, 80 },   // location 4
                { 570, 160 },  // locaiton 5
                { 798, 160 },  // location 6
                { 342, 240 },  // location 7
                { 684, 240 },  // location 8
                { 570, 400 },  // location 9
                { 912, 400 },  // location 10
                { 114, 480 },  // location 11
                { 228, 480 },  // location 12
                { 342, 560 },  // location 13
                { 684, 560 },  // location 14
                { 0, 640 },    // location 15
                { 798, 640 },  // location 16
            };

            // Define how many vehicles are in the problem.
            const int vehicleNumber = 4;

            // Depot is the starting location
            const int depot = 0;

            // Convert all the locations to a matrix of distances between each location
            long[,] distanceMatrix = DistanceCalculator.ComputeMahattanDistanceMatrix(locations);

            // Create the routing index manager
            // The manager does conversions of the solver's internal indices to the numbers for locations.
            var manager = new RoutingIndexManager(locations.GetLength(0), vehicleNumber, depot);

            // Create the routing modexl
            var routing = new RoutingModel(manager);

            int transitCallBackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable index to distance matrix NodeIndex
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return distanceMatrix[fromNode, toNode];
                });

            // Define cost of each arc/edge/route
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallBackIndex);

            // Add Distance constraint
            // This is the cumulative distance of a route.
            // We want to solve for the least distance travel.
            routing.AddDimension(
                transitCallBackIndex, // index of the callback function
                slack_max: 0, // represents waiting times at the locations
                capacity: 3000, // represents the maximum limit for the total distance accumulated along each route
                fix_start_cumul_to_zero: true, // cumulative value to start at 0
                name: "Distance");

            RoutingDimension distanceDimension = routing.GetMutableDimension("Distance");
            // Set the cost modifier associated with the dimension.
            // Setting it to a high value will make it the most important factor in the heuristic.
            distanceDimension.SetGlobalSpanCostCoefficient(100);

            // Setting the first solution heuristic
            // This does not always return the optimal solution.
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

            // Solve the problem
            Assignment solution = routing.SolveWithParameters(searchParameters);

            Debug.WriteLine($"Objective: {solution.ObjectiveValue()}");
            Debug.WriteLine("Route:");
            long maxRouteDistance = 0;
            for (int i = 0; i < vehicleNumber; i++)
            {
                Debug.WriteLine($"Route for vehicle {i}:");
                long routeDistance = 0;
                var index = routing.Start(i);
                var route = new RouteResponseDto();
                response.Routes.Add(route);
                while (!routing.IsEnd(index))
                {
                    var fromLocationNode = manager.IndexToNode((int)index);
                    Debug.Write($"{fromLocationNode} -> ");
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    var toLocationNode = manager.IndexToNode((int)index);
                    routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                    route.Locations.Add(
                        new LocationResponseDto
                        {
                            FromLocation = $"Location {fromLocationNode}",
                            ToLocation = $"Location {toLocationNode}",
                            Distance = routeDistance
                        });
                    route.TotalDistance = routeDistance;
                }
                Debug.WriteLine($"{manager.IndexToNode((int)index)}");
                Debug.WriteLine($"Route distance: {routeDistance}");
                maxRouteDistance = Math.Max(routeDistance, maxRouteDistance);
            }
            response.TotalDistance = response.Routes.Sum(r => r.TotalDistance);
            response.MaximumDistance = maxRouteDistance;

            Debug.WriteLine($"Maximum route distance: {maxRouteDistance}");

            return response;
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
        public RoutingResponseDto SolveCapacitatedVehicleRoutingProblem()
        {
            RoutingResponseDto response = new();

            int[,] locations = 
            {
                { 456, 320 },  // location 0
                { 228, 0 },    // location 1
                { 912, 0 },    // location 2
                { 0, 80 },     // location 3
                { 114, 80 },   // location 4
                { 570, 160 },  // locaiton 5
                { 798, 160 },  // location 6
                { 342, 240 },  // location 7
                { 684, 240 },  // location 8
                { 570, 400 },  // location 9
                { 912, 400 },  // location 10
                { 114, 480 },  // location 11
                { 228, 480 },  // location 12
                { 342, 560 },  // location 13
                { 684, 560 },  // location 14
                { 0, 640 },    // location 15
                { 798, 640 },  // location 16
            };

            // Each location has a demand corresponding to quantity of the item to be picked up
            long[] demands = { 0, 1, 1, 2, 4, 2, 4, 8, 8, 1, 2, 1, 2, 4, 4, 8, 8 };
            // Each vehicle has a capacity, the maximum quantity the vehicle can hold
            long[] vehicleCapacities = { 15, 15, 15, 15 };

            // Define how many vehicles are in the problem.
            const int vehicleNumber = 4;

            // Depot is the starting location
            const int depot = 0;

            // Convert all the locations to a matrix of distances between each location
            long[,] distanceMatrix = DistanceCalculator.ComputeMahattanDistanceMatrix(locations);

            // Create the routing index manager
            // The manager does conversions of the solver's internal indices to the numbers for locations.
            var manager = new RoutingIndexManager(locations.GetLength(0), vehicleNumber, depot);

            // Create the routing modexl
            var routing = new RoutingModel(manager);

            int transitCallBackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable index to distance matrix NodeIndex
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return distanceMatrix[fromNode, toNode];
                });

            // Define cost of each arc/edge/route
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallBackIndex);

            // Add Capacity constraint
            int demandCallBackIndex = routing.RegisterUnaryTransitCallback(
                (long fromIndex) =>
                {
                    // Convert from routing variable index to demand NodeIndex
                    var fromNode = manager.IndexToNode(fromIndex);
                    return demands[fromNode];
                });

            routing.AddDimensionWithVehicleCapacity(
                demandCallBackIndex,  // index of the call back function
                slack_max: 0, // null capacity slack
                vehicle_capacities: vehicleCapacities, // vehicle maximum capacities
                fix_start_cumul_to_zero: true, // cumulative value to start at 0
                name: "Capacity");

            // Setting the first solution heuristic
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Duration { Seconds = 1 };

            // Solve the problem
            Assignment solution = routing.SolveWithParameters(searchParameters);

            Debug.WriteLine($"Objective: {solution.ObjectiveValue()}");
            Debug.WriteLine("Route:");
            long maxRouteDistance = 0;
            for (int i = 0; i < vehicleNumber; i++)
            {
                Debug.WriteLine($"Route for vehicle {i}:");
                long routeDistance = 0;
                long routeLoad = 0;
                var index = routing.Start(i);
                var route = new RouteResponseDto();
                response.Routes.Add(route);
                while (!routing.IsEnd(index))
                {
                    var fromLocationNode = manager.IndexToNode((int)index);
                    routeLoad += demands[fromLocationNode];
                    Debug.Write($"{fromLocationNode} Load({routeLoad}) -> ");
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    var toLocationNode = manager.IndexToNode((int)index);
                    routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                    route.Locations.Add(
                        new LocationResponseDto
                        {
                            FromLocation = $"Location {fromLocationNode}",
                            ToLocation = $"Location {toLocationNode}",
                            Distance = routeDistance,
                            Load = routeLoad
                        });
                    route.TotalDistance = routeDistance;
                    route.TotalLoad = routeLoad;
                }
                Debug.WriteLine($"{manager.IndexToNode((int)index)}");
                Debug.WriteLine($"Route distance: {routeDistance}");
                Debug.WriteLine($"Route load: {routeLoad}");
                maxRouteDistance = Math.Max(routeDistance, maxRouteDistance);
            }
            response.TotalDistance = response.Routes.Sum(r => r.TotalDistance);
            response.MaximumDistance = maxRouteDistance;
            response.TotalLoad = response.Routes.Sum(r => r.TotalLoad);

            Debug.WriteLine($"Maximum route distance: {maxRouteDistance}");

            return response;
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
        public RoutingResponseDto SolvePickupDeliveryProblem()
        {
            RoutingResponseDto response = new();

            int[,] locations = 
            {
                { 456, 320 },  // location 0
                { 228, 0 },    // location 1
                { 912, 0 },    // location 2
                { 0, 80 },     // location 3
                { 114, 80 },   // location 4
                { 570, 160 },  // locaiton 5
                { 798, 160 },  // location 6
                { 342, 240 },  // location 7
                { 684, 240 },  // location 8
                { 570, 400 },  // location 9
                { 912, 400 },  // location 10
                { 114, 480 },  // location 11
                { 228, 480 },  // location 12
                { 342, 560 },  // location 13
                { 684, 560 },  // location 14
                { 0, 640 },    // location 15
                { 798, 640 },  // location 16
            };

            // Define the pick and delivery location pairings
            int[,] pickupsDeliveries =
            {
                { 1, 6 },
                { 2, 10 },
                { 4, 3 },
                { 5, 9 },
                { 7, 8 },
                { 15, 11 },
                { 13, 12 },
                { 16, 14 }
            };

            // Define how many vehicles are in the problem.
            const int vehicleNumber = 4;

            // Depot is the starting location
            const int depot = 0;

            // Convert all the locations to a matrix of distances between each location
            long[,] distanceMatrix = DistanceCalculator.ComputeMahattanDistanceMatrix(locations);

            // Create the routing index manager
            // The manager does conversions of the solver's internal indices to the numbers for locations.
            var manager = new RoutingIndexManager(locations.GetLength(0), vehicleNumber, depot);

            // Create the routing modexl
            var routing = new RoutingModel(manager);

            int transitCallBackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable index to distance matrix NodeIndex
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return distanceMatrix[fromNode, toNode];
                });

            // Define cost of each arc/edge/route
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallBackIndex);

            // Add Distance constraint
            // This is the cumulative distance of a route.
            // We want to solve for the least distance travel.
            routing.AddDimension(
                transitCallBackIndex, // index of the callback function
                slack_max: 0, // represents waiting times at the locations
                capacity: 3000, // represents the maximum limit for the total distance accumulated along each route
                fix_start_cumul_to_zero: true, // cumulative value to start at 0
                name: "Distance");

            RoutingDimension distanceDimension = routing.GetMutableDimension("Distance");
            // Set the cost modifier associated with the dimension.
            // Setting it to a high value will make it the most important factor in the heuristic.
            distanceDimension.SetGlobalSpanCostCoefficient(100);

            // Define Transportation requests
            Solver solver = routing.solver();
            for (int i = 0; i < pickupsDeliveries.GetLength(0); i++)
            {
                long pickupIndex = manager.NodeToIndex(pickupsDeliveries[i, 0]);
                long deliveryIndex = manager.NodeToIndex(pickupsDeliveries[i, 1]);
                // Create a pick-up and delivery request for each item
                routing.AddPickupAndDelivery(pickupIndex, deliveryIndex);
                // Request item must be picked up and delivered by the same vehicle
                solver.Add(routing.VehicleVar(pickupIndex) == routing.VehicleVar(deliveryIndex));
                // Request item must be picked up before it is delivered (by requiring cumulative distance to pick-up location <= cumulative distance of delivery location)
                solver.Add(distanceDimension.CumulVar(pickupIndex) <= distanceDimension.CumulVar(deliveryIndex));
            }

            // Setting the first solution heuristic
            // This does not always return the optimal solution.
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

            // Solve the problem
            Assignment solution = routing.SolveWithParameters(searchParameters);

            Debug.WriteLine($"Objective: {solution.ObjectiveValue()}");
            Debug.WriteLine("Route:");
            long maxRouteDistance = 0;
            for (int i = 0; i < vehicleNumber; i++)
            {
                Debug.WriteLine($"Route for vehicle {i}:");
                long routeDistance = 0;
                var index = routing.Start(i);
                var route = new RouteResponseDto();
                response.Routes.Add(route);
                while (!routing.IsEnd(index))
                {
                    var fromLocationNode = manager.IndexToNode((int)index);
                    Debug.Write($"{fromLocationNode} -> ");
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    var toLocationNode = manager.IndexToNode((int)index);
                    routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                    route.Locations.Add(
                        new LocationResponseDto
                        {
                            FromLocation = $"Location {fromLocationNode}",
                            ToLocation = $"Location {toLocationNode}",
                            Distance = routeDistance
                        });
                    route.TotalDistance = routeDistance;
                }
                Debug.WriteLine($"{manager.IndexToNode((int)index)}");
                Debug.WriteLine($"Route distance: {routeDistance}");
                maxRouteDistance = Math.Max(routeDistance, maxRouteDistance);
            }
            response.TotalDistance = response.Routes.Sum(r => r.TotalDistance);
            response.MaximumDistance = maxRouteDistance;

            Debug.WriteLine($"Maximum route distance: {maxRouteDistance}");

            return response;
        }

        /// <summary>
        /// Demo solving a Vehicle Routing Problem (VRP) with time windows.
        /// Given multiple vehicles and a time window available to each location,
        /// minimize the total travel time of the vehicles.
        /// </summary>
        /// <returns>
        /// A <see cref="RoutingResponseDto"/>
        /// </returns>
        public RoutingResponseDto SolveTimeWindowVehicleRoutingProblem()
        {
            RoutingResponseDto response = new();

            // Time between locations
            long[,] timeMatrix = 
            {
                { 0, 6, 9, 8, 7, 3, 6, 2, 3, 2, 6, 6, 4, 4, 5, 9, 7 },
                { 6, 0, 8, 3, 2, 6, 8, 4, 8, 8, 13, 7, 5, 8, 12, 10, 14 },
                { 9, 8, 0, 11, 10, 6, 3, 9, 5, 8, 4, 15, 14, 13, 9, 18, 9 },
                { 8, 3, 11, 0, 1, 7, 10, 6, 10, 10, 14, 6, 7, 9, 14, 6, 16 },
                { 7, 2, 10, 1, 0, 6, 9, 4, 8, 9, 13, 4, 6, 8, 12, 8, 14 },
                { 3, 6, 6, 7, 6, 0, 2, 3, 2, 2, 7, 9, 7, 7, 6, 12, 8 },
                { 6, 8, 3, 10, 9, 2, 0, 6, 2, 5, 4, 12, 10, 10, 6, 15, 5 },
                { 2, 4, 9, 6, 4, 3, 6, 0, 4, 4, 8, 5, 4, 3, 7, 8, 10 },
                { 3, 8, 5, 10, 8, 2, 2, 4, 0, 3, 4, 9, 8, 7, 3, 13, 6 },
                { 2, 8, 8, 10, 9, 2, 5, 4, 3, 0, 4, 6, 5, 4, 3, 9, 5 },
                { 6, 13, 4, 14, 13, 7, 4, 8, 4, 4, 0, 10, 9, 8, 4, 13, 4 },
                { 6, 7, 15, 6, 4, 9, 12, 5, 9, 6, 10, 0, 1, 3, 7, 3, 10 },
                { 4, 5, 14, 7, 6, 7, 10, 4, 8, 5, 9, 1, 0, 2, 6, 4, 8 },
                { 4, 8, 13, 9, 8, 7, 10, 3, 7, 4, 8, 3, 2, 0, 4, 5, 6 },
                { 5, 12, 9, 14, 12, 6, 6, 7, 3, 3, 4, 7, 6, 4, 0, 9, 2 },
                { 9, 10, 18, 6, 8, 12, 15, 8, 13, 9, 13, 3, 4, 5, 9, 0, 9 },
                { 7, 14, 9, 16, 14, 8, 5, 10, 6, 5, 4, 10, 8, 6, 2, 9, 0 },
            };

            // Time windows of each location
            long[,] timeWindows = 
            {
                { 0, 5 },   // depot
                { 7, 12 },  // 1
                { 10, 15 }, // 2
                { 16, 18 }, // 3
                { 10, 13 }, // 4
                { 0, 5 },   // 5
                { 5, 10 },  // 6
                { 0, 4 },   // 7
                { 5, 10 },  // 8
                { 0, 3 },   // 9
                { 10, 16 }, // 10
                { 10, 15 }, // 11
                { 0, 5 },   // 12
                { 5, 10 },  // 13
                { 7, 8 },   // 14
                { 10, 15 }, // 15
                { 11, 15 }, // 16
            };

            // Define how many vehicles are in the problem.
            const int vehicleNumber = 4;

            // Depot is the starting location
            const int depot = 0;

            // Create the routing index manager
            // The manager does conversions of the solver's internal indices to the numbers for locations.
            var manager = new RoutingIndexManager(timeMatrix.GetLength(0), vehicleNumber, depot);

            // Create the routing modexl
            var routing = new RoutingModel(manager);

            int transitCallBackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable index to time matrix NodeIndex
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    return timeMatrix[fromNode, toNode];
                });

            // Define cost of each arc/edge/route
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallBackIndex);

            // Add Time constraint
            // This is the cumulative time of a route.
            // We want to solve for the least time travel.
            routing.AddDimension(
                transitCallBackIndex, // index of the callback function
                slack_max: 30, // represents waiting times at the locations due to time window constraints
                capacity: 30, // represents the maximum limit for the total time accumulated along each route
                fix_start_cumul_to_zero: false, // cumulative value not to start at 0
                name: "Time");

            RoutingDimension timeDimension = routing.GetMutableDimension("Time");
            // Add time window constraints for each location except depot.
            for (int i = 1; i < timeWindows.GetLength(0); i++)
            {
                long index = manager.NodeToIndex(i);
                timeDimension.CumulVar(index).SetRange(timeWindows[i, 0], timeWindows[i, 1]);
            }
            // Add time window constraints for each vehicle start node.
            for (int i = 0; i < vehicleNumber; i++)
            {
                long index = routing.Start(i);
                timeDimension.CumulVar(index).SetRange(timeWindows[0, 0], timeWindows[0, 1]);
            }

            // Instantiate route start and end times to produce feasible times.
            for (int i = 0; i < vehicleNumber; i++)
            {
                routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(routing.Start(i)));
                routing.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(routing.End(i)));
            }

            // Setting the first solution heuristic
            // This does not always return the optimal solution.
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

            // Solve the problem
            Assignment solution = routing.SolveWithParameters(searchParameters);

            Debug.WriteLine($"Objective: {solution.ObjectiveValue()}");
            Debug.WriteLine("Route:");
            long totalTime = 0;
            for (int i = 0; i < vehicleNumber; i++)
            {
                Debug.WriteLine($"Route for vehicle {i}:");
                var index = routing.Start(i);
                var route = new RouteResponseDto();
                response.Routes.Add(route);
                while (!routing.IsEnd(index))
                {
                    var startLocationNode = manager.IndexToNode((int)index);
                    var timeVariable = timeDimension.CumulVar(index);
                    var fromTime = solution.Min(timeVariable);
                    var toTime = solution.Max(timeVariable);
                    Debug.Write($"{startLocationNode} Time({fromTime}, {toTime}) -> ");
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    route.Locations.Add(
                        new LocationResponseDto
                        {
                            ToLocation = $"Location {startLocationNode}",
                            FromTime = fromTime,
                            ToTime = toTime
                        });
                }
                var endLocationNode = manager.IndexToNode((int)index);
                var endTimeVariable = timeDimension.CumulVar(index);
                var endLocationFromTime = solution.Min(endTimeVariable);
                var endLocationToTime = solution.Max(endTimeVariable);
                Debug.Write($"{endLocationNode} Time({endLocationFromTime}, {endLocationToTime}) -> ");
                route.Locations.Add(
                    new LocationResponseDto
                    {
                        ToLocation = $"Location {endLocationNode}",
                        FromTime = endLocationFromTime,
                        ToTime = endLocationToTime
                    });

                Debug.WriteLine($"Time of the route: {endLocationFromTime}min");
                route.TotalTime = endLocationFromTime;
                totalTime += endLocationFromTime;
            }
            response.TotalTime = totalTime;

            Debug.WriteLine($"Total time of all routes: {totalTime}min");

            return response;
        }
    }
}
