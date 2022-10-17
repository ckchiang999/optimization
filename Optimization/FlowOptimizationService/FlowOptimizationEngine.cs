using System.Diagnostics;
using FlowOptimizationService.Enums;
using FlowOptimizationService.Interfaces;
using FlowOptimizationService.Models;

using Google.OrTools.Graph;

namespace FlowOptimizationService
{
    public class FlowOptimizationEngine : IFlowOptimization
    {
        /// <summary>
        /// Demo solving maximum flow from a source node to a sink node.
        /// <para>
        /// At each node, other than the source or the sink, the total flow of all
        /// arcs leading in to the node equals the total flow of all arcs leading out of it.
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        public FlowResponseDto SolveMaximumFlow()
        {
            var response = new FlowResponseDto();

            // Define the solver.
            var solver = new MaxFlow();

            // Define start nodes.
            int[] startNodes = { 0, 0, 0, 1, 1, 2, 2, 3, 3 };

            // Define end nodes for each of the start nodes.
            int[] endNodes = { 1, 2, 3, 2, 4, 3, 4, 2, 4 };

            // Define the capacities between the pair of start and end nodes.
            int[] capacities = { 20, 30, 10, 40, 30, 10, 20, 5, 20 };

            // Add each arc.
            for (int i = 0; i < startNodes.Length; i++)
            {
                solver.AddArcWithCapacity(startNodes[i], endNodes[i], capacities[i]);
            }

            // Find the maximum flow between node 0 and node 4.
            MaxFlow.Status status = solver.Solve(0, 4);

            response.Status = status switch
            {
                MaxFlow.Status.OPTIMAL => OptimizationStatus.Optimal,
                MaxFlow.Status.BAD_INPUT => OptimizationStatus.BadInput,
                MaxFlow.Status.BAD_RESULT => OptimizationStatus.BadResult,
                MaxFlow.Status.POSSIBLE_OVERFLOW => OptimizationStatus.PossibleOverflow,
                _ => OptimizationStatus.Infeasible
            };
            if (status == MaxFlow.Status.OPTIMAL)
            {
                response.OptimalValue = solver.OptimalFlow();
                Debug.WriteLine($"Max flow: {response.OptimalValue}");
                Debug.WriteLine("Arc,Flow,Capacity");
                for (int i = 0; i < solver.NumArcs(); i++)
                {
                    Debug.WriteLine($"{solver.Tail(i)} -> {solver.Head(i)},{solver.Flow(i)},{solver.Capacity(i)}");
                    response.Flows.Add(new FlowDto
                    {
                        Head = solver.Head(i),
                        Tail = solver.Tail(i),
                        Flow = solver.Flow(i),
                        Capacity = solver.Capacity(i)
                    });
                }
            }
            else
            {
                Debug.WriteLine($"Solving the max flow problem failed.  Solver status: {status}");
            }
            return response;
        }

        /// <summary>
        /// Demo solving minimum cost flow from a source node to a sink node.
        /// Each line connecting one node to the next has a cost associated wiht it.
        /// Solve by finding the flow with the least total cost.
        /// <para>
        /// <list type="bullet">
        /// <item>
        /// <description>At the supply node, a positive amount is added to the flow.</description>    
        /// </item>
        /// <item>
        /// <description>At the demand node, a negative amount is taken away from the flow.</description>
        /// </item>
        /// <item>
        /// <description>Supply is indicated by a positive value and demand by a negative value.</description>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        public FlowResponseDto SolveMinimumCostFlow()
        {
            var response = new FlowResponseDto();

            // Define the source nodes.
            int[] startNodes = { 0, 0, 1, 1, 1, 2, 2, 3, 4 };

            // Define end nodes for each of the start nodes.
            int[] endNodes = { 1, 2, 2, 3, 4, 3, 4, 4, 2 };

            // Define the capacities between the pair of start and end nodes.
            int[] capacities = { 15, 8, 20, 4, 10, 15, 4, 20, 5 };

            // Define the unit cost between the pair of start and end nodes.
            int[] unitCosts = { 4, 4, 2, 2, 6, 1, 3, 2, 3 };

            // Define the supplies/demands at each node.
            int[] supplies = { 20, 0, 0, -5, -15 };

            // Create the solver.
            var solver = new MinCostFlow();

            // Add each arc.
            for (int i = 0; i < startNodes.Length; i++)
            {
                solver.AddArcWithCapacityAndUnitCost(
                    startNodes[i], 
                    endNodes[i], 
                    capacities[i], 
                    unitCosts[i]);
            }

            // Add node supplies.
            for (int i = 0; i < supplies.Length; i++)
            {
                solver.SetNodeSupply(i, supplies[i]);
            }

            // Find the min cost flow.
            MinCostFlow.Status status = solver.Solve();

            response.Status = status switch
            {
                MinCostFlow.Status.OPTIMAL => OptimizationStatus.Optimal,
                MinCostFlow.Status.BAD_COST_RANGE => OptimizationStatus.BadInput,
                MinCostFlow.Status.BAD_RESULT => OptimizationStatus.BadResult,
                MinCostFlow.Status.NOT_SOLVED => OptimizationStatus.NotSolved,
                MinCostFlow.Status.UNBALANCED => OptimizationStatus.Unbalanced,
                MinCostFlow.Status.INFEASIBLE => OptimizationStatus.Infeasible,
                _ => OptimizationStatus.Unknown
            };
            if (status == MinCostFlow.Status.OPTIMAL)
            {
                response.OptimalValue = solver.OptimalCost();                
                Debug.WriteLine($"Max flow: {response.OptimalValue}");
                Debug.WriteLine("Arc,Flow,Capacity");
                for (int i = 0; i < solver.NumArcs(); i++)
                {
                    Debug.WriteLine($"{solver.Tail(i)} -> {solver.Head(i)},{solver.Flow(i)},{solver.Capacity(i)}");
                    response.Flows.Add(new FlowDto
                    {
                        Tail = solver.Tail(i),
                        Head = solver.Head(i),
                        Flow = solver.Flow(i),
                        Capacity = solver.Capacity(i),
                        Cost = solver.Flow(i) * solver.UnitCost(i)
                    });
                }
            }
            else
            {
                Debug.WriteLine($"Solving the min cost flow problem failed.  Solver status: {status}");
            }

            return response;
        }

        /// <summary>
        /// Demo solving an assignment problem using the minimum flow solver.
        /// <para>
        /// Flow is 1 and capacity is 1.  Flow is from worker to task.
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        public FlowResponseDto SolveAssignmentAsFlow()
        {
            var response = new FlowResponseDto();

            // Define the start nodes.
            int[] startNodes = { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 6, 7, 8 };

            // Define end nodes for each of the start nodes.
            int[] endNodes = { 1, 2, 3, 4, 5, 6, 7, 8, 5, 6, 7, 8, 5, 6, 7, 8, 5, 6, 7, 8, 9, 9, 9, 9 };

            // Define the capacities between the pair of start and end nodes.  For assignment, these are all 1.
            int[] capacities = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            // Define the unit cost between the pair of start and end nodes.
            int[] unitCosts = { 0, 0, 0, 0, 90, 76,  75, 70, 35, 85, 55, 65, 125, 95, 90, 105, 45, 110, 95, 115, 0, 0, 0, 0 };

            // Source node
            int source = 0;

            // Sink node
            int sink = 9;

            // Number of tasks
            int tasks = 4;

            // Define an array of supplies at each node.
            int[] supplies = { tasks, 0, 0, 0, 0, 0, 0, 0, 0, -tasks };

            // Create the solver
            var solver = new MinCostFlow();

            // Add each arc.
            for (int i = 0; i < startNodes.Length; i++)
            {
                solver.AddArcWithCapacityAndUnitCost(startNodes[i], endNodes[i], capacities[i], unitCosts[i]);
            }

            // Add node supplies
            for (int i = 0; i < supplies.Length; i++)
            {
                solver.SetNodeSupply(i, supplies[i]);
            }

            // Find the min cost flow.
            MinCostFlow.Status status = solver.Solve();

            response.Status = status switch
            {
                MinCostFlow.Status.OPTIMAL => OptimizationStatus.Optimal,
                MinCostFlow.Status.BAD_COST_RANGE => OptimizationStatus.BadInput,
                MinCostFlow.Status.BAD_RESULT => OptimizationStatus.BadResult,
                MinCostFlow.Status.NOT_SOLVED => OptimizationStatus.NotSolved,
                MinCostFlow.Status.UNBALANCED => OptimizationStatus.Unbalanced,
                MinCostFlow.Status.INFEASIBLE => OptimizationStatus.Infeasible,
                _ => OptimizationStatus.Unknown
            };
            if (status == MinCostFlow.Status.OPTIMAL)
            {
                response.OptimalValue = solver.OptimalCost();
                Debug.WriteLine($"Total cost: {response.OptimalValue}");
                for (int i = 0; i < solver.NumArcs(); i++)
                {
                    if (solver.Tail(i) == source || solver.Head(i) == sink)
                    {
                        // Ignore the tail and head since they are not relevant parts of the solution.
                        continue;
                    }

                    if (solver.Flow(i) > 0)
                    {
                        // When flow value is 1, it means a task has been assigned to a worker.
                        // The tail is the worker and the head is the task.
                        Debug.WriteLine($"Worker {solver.Tail(i)} assigned to task {solver.Head(i)} Cost: {solver.UnitCost(i)}");
                        response.Flows.Add(new FlowDto
                        {
                            Tail = solver.Tail(i),
                            Head = solver.Head(i),
                            Cost = solver.UnitCost(i)
                        });
                    }
                    
                }
            }
            else
            {
                Debug.WriteLine($"Solving the min cost flow problem failed.  Solver status: {status}");
            }

            return response;
        }

        /// <summary>
        /// Demo solving a team assignment problem using the minimum flow solver.
        /// <para>
        /// Flow is 1 and capacity is 1.  Flow is from worker to task.
        /// There are six workers divided into two teams, and
        /// there are four tasks.  Each team will perform 2 tasks.
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        public FlowResponseDto SolveTeamAssignmentAsFlow()
        {
            var response = new FlowResponseDto();

            // Define the two teams
            int[] teamA = { 1, 3, 5 };
            int[] teamB = { 2, 4, 6 };

            // Define the start nodes.
            int[] startNodes = { 0, 0, 11, 11, 11, 12, 12, 12, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4,  4,  4,  4,  5,  5,  5, 5, 6, 6, 6, 6, 7, 8, 9, 10 };

            // Define the end nodes paired to the start nodes.
            int[] endNodes = { 11, 12, 1, 3, 5, 2,  4, 6, 7, 8, 9, 10, 7, 8,  9, 10, 7, 8, 9,  10, 7, 8, 9, 10, 7, 8, 9, 10, 7, 8,  9, 10, 13, 13, 13, 13 };

            // Define the capacities of each node.
            // The source node goes to the two team nodes.  The team node has a capacity of 2 (tasks).
            int[] capacities = { 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            // Define the cost of each flow from start node to end node.
            int[] unitCosts = { 0, 0, 0, 0, 0, 0, 0, 0, 90, 76, 75, 70, 35,  85, 55, 65, 125, 95, 90, 105, 45, 110, 95, 115, 60, 105, 80, 75, 45, 65, 110, 95, 0, 0, 0, 0 };

            // Define the source node.
            int source = 0;

            // Define the sink node.
            int sink = 13;

            // Define the number of tasks.
            int tasks = 4;

            // Define the team nodes.
            int team1Node = 11;
            int team2Node = 12;

            // Define an array of supplies at each node.
            int[] supplies = { tasks, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -tasks };

            // Create the solver
            var solver = new MinCostFlow();

            // Add each arc.
            for (int i = 0; i < startNodes.Length; ++i)
            {
                solver.AddArcWithCapacityAndUnitCost(startNodes[i], endNodes[i], capacities[i], unitCosts[i]);
            }

            // Add node supplies.
            for (int i = 0; i < supplies.Length; ++i)
            {
                solver.SetNodeSupply(i, supplies[i]);
            }

            // Find the min cost flow.
            MinCostFlow.Status status = solver.Solve();

            response.Status = status switch
            {
                MinCostFlow.Status.OPTIMAL => OptimizationStatus.Optimal,
                MinCostFlow.Status.BAD_COST_RANGE => OptimizationStatus.BadInput,
                MinCostFlow.Status.BAD_RESULT => OptimizationStatus.BadResult,
                MinCostFlow.Status.NOT_SOLVED => OptimizationStatus.NotSolved,
                MinCostFlow.Status.UNBALANCED => OptimizationStatus.Unbalanced,
                MinCostFlow.Status.INFEASIBLE => OptimizationStatus.Infeasible,
                _ => OptimizationStatus.Unknown
            };
            if (status == MinCostFlow.Status.OPTIMAL)
            {
                response.OptimalValue = solver.OptimalCost();
                Debug.WriteLine($"Total cost: {response.OptimalValue}");
                for (int i = 0; i < solver.NumArcs(); i++)
                {
                    if (solver.Tail(i) == source || 
                        solver.Tail(i) == team1Node || 
                        solver.Tail(i) == team2Node ||
                        solver.Head(i) == sink )
                    {
                        // Ignore the tail and head since they are not relevant parts of the solution.
                        continue;
                    }

                    if (solver.Flow(i) > 0)
                    {
                        // When flow value is 1, it means a task has been assigned to a worker.
                        // The tail is the worker and the head is the task.
                        Debug.WriteLine($"Worker {solver.Tail(i)} assigned to task {solver.Head(i)} Cost: {solver.UnitCost(i)}");
                        response.Flows.Add(new FlowDto
                        {
                            Tail = solver.Tail(i),
                            Head = solver.Head(i),
                            Cost = solver.UnitCost(i)
                        });
                    }

                }
            }
            else
            {
                Debug.WriteLine($"Solving the min cost flow problem failed.  Solver status: {status}");
            }

            return response;
        }
    }
}
