using System.Diagnostics;
using BinPackingOptimizationService.Interfaces;
using BinPackingOptimizationService.Models;
using Google.OrTools.Algorithms;
using Google.OrTools.LinearSolver;
using Google.OrTools.Sat;

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
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, 1, "");
                foreach (int bin in allBins)
                {
                    constraint.SetCoefficient(variables[item, bin], 1);
                }
            }

            // The amount packed in each bin cannot exceed its capacity.
            foreach (int bin in allBins)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, binCapacities[bin], "");
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
                                BinIndex = bin,
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

        /// <summary>
        /// Demo solving a multiple knapsack problem using CP-SAT (Constrained Programming - Satisfiability) solver.
        /// <para>
        /// We have five bins each with a capacity of 100.  We want to maximize total packed value.
        /// </para>
        /// </summary>
        /// <returns>
        /// A <see cref="BinPackingResponseDto"/>
        /// </returns>
        public BinPackingResponseDto SolveMultipleKnapsackProblemWithCpSat()
        {
            var response = new BinPackingResponseDto();

            // item weights
            int[] weights = { 48, 30, 42, 36, 36, 48, 42, 42, 36, 24, 30, 30, 42, 36, 36 };

            // item values
            int[] values = { 10, 30, 25, 50, 35, 30, 15, 40, 30, 35, 45, 10, 20, 30, 25 };

            int numItems = weights.Length;
            int[] allItems = Enumerable.Range(0, numItems).ToArray();
            int[] binCapacities = { 100, 100, 100, 100, 100 };
            int numBins = binCapacities.Length;
            int[] allBins = Enumerable.Range(0, numBins).ToArray();

            // Create the model
            var model = new CpModel();

            // Define the variables
            var variables = new ILiteral[numItems, numBins];
            foreach (int item in allItems)
            {
                foreach (int bin in allBins)
                {
                    variables[item, bin] = model.NewBoolVar($"variable_{item}_{bin}");
                }
            }

            // Define the constraints
            // Each item is assigned to at most one bin.
            foreach (int item in allItems)
            {
                var literals = new List<ILiteral>();
                foreach (int bin in allBins)
                {
                    literals.Add(variables[item, bin]);
                }
                model.AddAtMostOne(literals);
            }

            // The amount packed in each bin cannot exceed its capacity.
            foreach (int bin in allBins)
            {
                var items = new List<ILiteral>();
                foreach (int item in allItems)
                {
                    items.Add(variables[item, bin]);
                }
                model.Add(Google.OrTools.Sat.LinearExpr.WeightedSum(items, weights) <= binCapacities[bin]);
            }

            // Define the objective
            LinearExprBuilder objective = Google.OrTools.Sat.LinearExpr.NewBuilder();
            foreach (int item in allItems)
            {
                foreach (int bin in allBins)
                {
                    objective.AddTerm(variables[item, bin], values[item]);
                }
            }
            model.Maximize(objective);

            // Solve
            var solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);

            // Check that the problem has a feasible solution.
            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                response.OptimalValue = solver.ObjectiveValue; 
                Debug.WriteLine($"Total packed value: {response.OptimalValue}");
                double totalWeight = 0.0;
                foreach (int bin in allBins)
                {
                    double binWeight = 0.0;
                    double binValue = 0.0;
                    Debug.WriteLine($"Bin {bin}");
                    foreach (int item in allItems)
                    {
                        if (solver.BooleanValue(variables[item, bin]))
                        {
                            Debug.WriteLine($"Item {item} weight: {weights[item]} values: {values[item]}");
                            binWeight += weights[item];
                            binValue += values[item];
                            response.Items.Add(new ItemDto
                            {
                                BinIndex = bin,
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

            Debug.WriteLine("Statistics");
            Debug.WriteLine($"Conflicts: {solver.NumConflicts()}");
            Debug.WriteLine($"Branches: {solver.NumBranches()}");
            Debug.WriteLine($"Wall Time: {solver.WallTime()}s");

            return response;
        }

        /// <summary>
        /// Demo solving a bin packing problem.
        /// <para>
        /// Given same size bins, find the fewest that will hold all the items.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public BinPackingResponseDto SolveBinPackingProblem()
        {
            var response = new BinPackingResponseDto();

            double[] weights = { 48, 30, 19, 36, 36, 27, 42, 42, 36, 24, 30 };
            int numItems = weights.Length;
            int numBins = weights.Length;
            double binCapacity = 100.0;

            // Create the solver
            Solver solver = Solver.CreateSolver("SCIP");

            // Define the variables
            // Variable whose value is 1 if item i is placed in bin j and 0 otherwise.
            var itemPlacedVariables = new Variable[numItems, numBins];
            for (int i = 0; i < numItems; i++)
            {
                for (int j = 0; j < numBins; j++)
                {
                    itemPlacedVariables[i, j] = solver.MakeIntVar(0, 1, $"item_placed_{i}_{j}");
                }
            }

            // Variable whose value is 1 if bin j is used and 0 otherwise.
            // The sum of these variables will the number of bins used.
            var binUsedVariables = new Variable[numBins];
            for (int j = 0; j < numBins; j++)
            {
                binUsedVariables[j] = solver.MakeIntVar(0, 1, $"bin_used_{j}");
            }

            // Each item must be placed in exactly one bin.
            // The sum of itemPlacedVariables over all bins is equal to 1.
            for (int i = 0; i < numItems; i++)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(1, 1, "");
                for (int j = 0; j < numBins; j++)
                {
                    constraint.SetCoefficient(itemPlacedVariables[i, j], 1);
                }
            }

            for (int j = 0; j < numBins; j++)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, Double.PositiveInfinity, "");
                // Force binUsedVariables[j] to equal 1 if any item is packed in bin j.
                constraint.SetCoefficient(binUsedVariables[j], binCapacity);

                for (int i = 0; i < numItems; i++)
                {
                    constraint.SetCoefficient(itemPlacedVariables[i, j], -weights[i]);
                }
            }

            // Define the objective.
            // Minimize the sum of the number of bins used.
            var objective = solver.Objective();
            for (int j = 0; j < numBins; j++)
            {
                objective.SetCoefficient(binUsedVariables[j], 1);
            }
            objective.SetMinimization();

            // Solve
            Solver.ResultStatus status = solver.Solve();

            // Check that the problem has an optimal solution.
            if (status != Solver.ResultStatus.OPTIMAL)
            {
                Debug.WriteLine("The problem does not have an optimal solution!");
                return response;
            }

            response.Capacities.Add(binCapacity);
            double totalWeight = 0.0;
            for (int j = 0; j < numBins; j++)
            {
                double binWeight = 0.0;
                if (binUsedVariables[j].SolutionValue() == 1)
                {
                    Debug.WriteLine($"Bin {j}");
                    for (int i = 0; i < numItems; i++)
                    {
                        if (itemPlacedVariables[i, j].SolutionValue() == 1)
                        {
                            Debug.WriteLine($"Item {i} weight: {weights[i]}");
                            binWeight += weights[i];
                            response.Items.Add(new ItemDto()
                            {
                                BinIndex = j,
                                ItemIndex = i,
                                Value = null,
                                Weight = weights[i]
                            });
                        }
                    }
                    Debug.WriteLine($"Packed bin weight: {binWeight}");
                    totalWeight += binWeight;
                }
            }
            Debug.WriteLine($"Total packed weight: {totalWeight}");

            response.OptimalValue = totalWeight;
            return response;
        }
    }
}
