using System.Diagnostics;
using BinPackingOptimizationService.Interfaces;
using BinPackingOptimizationService.Models;
using Google.OrTools.Algorithms;
using Google.OrTools.LinearSolver;

namespace BinPackingOptimizationService
{
    public class BinPackingOptimizationEngine : IBinPackingOptimization
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
        public BinPackingResponseDto SolveKnapsackProblem()
        {
            var response = new BinPackingResponseDto();

            // value for each of the items
            long[] values = 
            { 
                360, 83, 59, 130, 431, 67,  230, 52,  93,  125, 670, 892, 600, 38,  48,  147, 78,
                256, 63, 17, 120, 164, 432, 35,  92,  110, 22,  42,  50,  323, 514, 28,  87,  73,
                78,  15, 26, 78,  210, 36,  85,  189, 274, 43,  33,  10,  19,  389, 276, 312
            };

            // weights of each of the items
            long[,] weights = 
            { 
                { 
                    7,  0,  30, 22, 80, 94, 11, 81, 70, 64, 59, 18, 0,  36, 3,  8,  15,
                    42, 9,  0,  42, 47, 52, 32, 26, 48, 55, 6,  29, 84, 2,  4,  18, 56,
                    7,  29, 93, 44, 71, 3,  86, 66, 31, 65, 0,  79, 20, 65, 52, 13 
                }
            };

            // maximum weight the knapsack can hold
            long[] capacities = { 850 };

            var solver = new KnapsackSolver(
                KnapsackSolver.SolverType.KNAPSACK_MULTIDIMENSION_BRANCH_AND_BOUND_SOLVER,
                "KnapsackExample" // just give the solver a name
            );

            solver.Init(values, weights, capacities);
            long computedValue = solver.Solve();
            Debug.WriteLine($"Optimal Value = {computedValue}");

            foreach (var capacity in capacities)
            {
                response.Capacities.Add(capacity);
            }
            response.OptimalValue = computedValue;
            for (int i = 0; i < weights.GetLength(1); i++)
            {
                if (solver.BestSolutionContains(i))
                {
                    response.Items.Add(
                        new ItemDto
                        {
                            ItemIndex = i,
                            Value = values[i],
                            Weight = weights[0, i]
                        }
                    );
                }
            }

            Debug.WriteLine($"Total weight: {response.Items.Sum(i => i.Weight)}");
            Debug.WriteLine($"Packed items: [{string.Join(", ", response.Items.Select(i => i.ItemIndex))}]");
            Debug.WriteLine($"Packed weights: [{string.Join(", ", response.Items.Select(i => i.Weight))}]");

            return response;
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
        public BinPackingResponseDto SolveMultipleKnapsackProblemWithMip()
        {
            var response = new BinPackingResponseDto();

            // item weights
            double[] weights = { 48, 30, 42, 36, 36, 48, 42, 42, 36, 24, 30, 30, 42, 36, 36 };

            // item values
            double[] values = { 10, 30, 25, 50, 35, 30, 15, 40, 30, 35, 45, 10, 20, 30, 25 };

            int numItems = weights.Length;
            int[] allItems = Enumerable.Range(0, numItems).ToArray();
            double[] binCapacities = { 100, 100, 100, 100, 100 };
            int numBins = binCapacities.Length;
            int[] allBins = Enumerable.Range(0, numBins).ToArray();

            // Create a linear solver
            Solver solver = Solver.CreateSolver("SCIP");
            var variables = new Variable[numItems, numBins];
            foreach (int item in allItems)
            {
                foreach (int bin in allBins)
                {
                    variables[item, bin] = solver.MakeBoolVar($"variable_{item}_{bin}");
                }
            }

            // Create the constraints
            // Each item is assigned to at most one bin.
            foreach (int item in allItems)
            {
                Constraint constraint = solver.MakeConstraint(0, 1, "");
                foreach (int bin in allBins)
                {
                    constraint.SetCoefficient(variables[item, bin], 1);
                }
            }

            // The amount packed in each bin cannot exceed its capacity.
            foreach (int bin in allBins)
            {
                Constraint constraint = solver.MakeConstraint(0, binCapacities[bin], "");
                foreach (int item in allItems)
                {
                    constraint.SetCoefficient(variables[item, bin], weights[item]);
                }
            }

            // Define the objective
            var objective = solver.Objective();
            foreach (int item in allItems)
            {
                foreach (int bin in allBins)
                {
                    objective.SetCoefficient(variables[item, bin], values[item]);
                }
            }
            objective.SetMaximization();

            Solver.ResultStatus status = solver.Solve();

            foreach (var capacity in binCapacities)
            {
                response.Capacities.Add(capacity);
            }
            // Check that the problem has an optimal solution.
            if (status == Solver.ResultStatus.OPTIMAL)
            {
                response.OptimalValue = solver.Objective().Value();
                Debug.WriteLine($"Total packed value: {response.OptimalValue}");
                double totalWeight = 0.0;
                foreach (int bin in allBins)
                {
                    double binWeight = 0.0;
                    double binValue = 0.0;
                    Debug.WriteLine($"Bin {bin}");
                    foreach (int item in allItems)
                    {
                        if (variables[item, bin].SolutionValue() == 1)
                        {
                            Debug.WriteLine($"Item {item} weight: {weights[item]} values: {values[item]}");
                            binWeight += weights[item];
                            binValue += values[item];
                            response.Items.Add(new ItemDto
                            {
                                ItemIndex = item,
                                Value = values[item],
                                Weight = weights[item]
                            });
                        }
                    }
                    Debug.WriteLine($"Packed bin weight: {binWeight}");
                    Debug.WriteLine($"Packed bin value: {binValue}");
                    totalWeight += binWeight;
                }
                Debug.WriteLine($"Total packed weight: {totalWeight}");
            }
            else
            {
                Debug.WriteLine("The problem does not have an optimal solution!");
            }

            return response;
        }
    }
}
