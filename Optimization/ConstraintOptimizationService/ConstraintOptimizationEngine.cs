using System.Diagnostics;
using ConstraintOptimizationService.Helpers;
using ConstraintOptimizationService.Interfaces;
using ConstraintOptimizationService.Models;
using Google.OrTools.Sat;

namespace ConstraintOptimizationService
{
    public class ConstraintOptimizationEngine : IConstraintOptimization
    {
        /// <summary>
        /// Find the first solution to a simple constraint optimization problem 
        /// </summary>
        /// <returns>
        /// A <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/> with the solution
        /// </returns>
        public ConstraintResponseDto SolveDemoFirstSolution()
        {
            // Declare a CP-SAT model (Constrained Programming - Satisfiability)
            CpModel model = new();
            int min = 0;
            int max = 2;

            // Declare 3 variables
            IntVar x = model.NewIntVar(min, max, "x");
            IntVar y = model.NewIntVar(min, max, "x");
            IntVar z = model.NewIntVar(min, max, "x");

            // Create a constraint
            model.Add(x != y);

            // Solve
            CpSolver solver = new();
            CpSolverStatus status = solver.Solve(model);

            // Wrap the solution in a dto
            ConstraintResponseDto response = new();
            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Debug.WriteLine("x = " + solver.Value(x));
                Debug.WriteLine("y = " + solver.Value(y));
                Debug.WriteLine("z = " + solver.Value(z));

                SolutionDto solution = new();
                solution.TimeToSolve = solver.WallTime();
                solution.Variables.Add(new VariableResponseDto() { Name = "x", Solution = solver.Value(x) });
                solution.Variables.Add(new VariableResponseDto() { Name = "y", Solution = solver.Value(y) });
                solution.Variables.Add(new VariableResponseDto() { Name = "z", Solution = solver.Value(z) });                
                response.Solutions.Add(solution);
                response.Status = (status == CpSolverStatus.Optimal)
                    ? OptimizationStatus.Optimal : OptimizationStatus.Feasible;
            }
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = OptimizationStatus.Infeasible;
            }
            return response;
        }

        /// <summary>
        /// Find all the solutions to a simple constraint optimization problem
        /// </summary>
        /// <returns>
        /// A <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/> with all the solutions
        /// </returns>
        public ConstraintResponseDto SolveDemoAllSolutions()
        {
            ConstraintResponseDto response = new();

            // Declare a CP-SAT model (Constrained Programming - Satisfiability)
            CpModel model = new();
            int min = 0;
            int max = 2;

            // Declare 3 variables
            IntVar x = model.NewIntVar(min, max, "x");
            IntVar y = model.NewIntVar(min, max, "x");
            IntVar z = model.NewIntVar(min, max, "x");

            // Create a constraint
            model.Add(x != y);

            // Create solver
            CpSolver solver = new();

            // Create a callback to aggregate solutions
            IntVar[] variables = new[] { x, y, z };
            SolutionAggregator solutionAggregator = new(response, variables);

            // Search for all solutions.
            solver.StringParameters = "enumerate_all_solutions:true";

            // Solve
            CpSolverStatus status = solver.Solve(model, solutionAggregator);

            // Wrap the solution in a dto
            Debug.WriteLine($"Number of solutions found: {response.Solutions.Count()}");
            return response;
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
        public ConstraintResponseDto SolveCpOptimization()
        {
            CpModel model = new();

            // Define the variables.

            // Since the first constraint has fractional y and z coefficients,
            // we need to normalize it so all coefficients are integers.
            // Multiply the constraint by 2.
            //   2x + 7y + 3z <= 50
            IntVar x = model.NewIntVar(0, 50, "x");
            IntVar y = model.NewIntVar(0, 45, "y");
            IntVar z = model.NewIntVar(0, 37, "z");

            // Define the constraints.
            model.Add(2 * x + 7 * y + 3 * z <= 50);
            model.Add(3 * x - 5 * y + 7 * z <= 45);
            model.Add(5 * x + 2 * y - 6 * z <= 37);

            // Define the objective
            model.Maximize((2 * x) + (2 * y) + (3 * z));

            // Call the solver
            CpSolver solver = new();
            CpSolverStatus status = solver.Solve(model);

            // Wrap solution in a response dto
            ConstraintResponseDto response = new();
            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Debug.WriteLine($"Maximum of objective function: {solver.ObjectiveValue}");
                Debug.WriteLine("x = " + solver.Value(x));
                Debug.WriteLine("y = " + solver.Value(y));
                Debug.WriteLine("z = " + solver.Value(z));

                SolutionDto solution = new();
                solution.TimeToSolve = solver.WallTime();
                solution.Variables.Add(new VariableResponseDto() { Name = "x", Solution = solver.Value(x) });
                solution.Variables.Add(new VariableResponseDto() { Name = "y", Solution = solver.Value(y) });
                solution.Variables.Add(new VariableResponseDto() { Name = "z", Solution = solver.Value(z) });
                response.Solutions.Add(solution);
                response.Status = (status == CpSolverStatus.Optimal)
                    ? OptimizationStatus.Optimal : OptimizationStatus.Feasible;

                response.NumberOfConflicts = solver.NumConflicts();
                response.NumberOfBranches = solver.NumBranches();
                response.TimeToSolve = solver.WallTime();
            }
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = OptimizationStatus.Infeasible;
            }

            Debug.WriteLine("Statistics");
            Debug.WriteLine($"\tconflicts: {solver.NumConflicts()}");
            Debug.WriteLine($"\tbranches: {solver.NumBranches()}");
            Debug.WriteLine($"\twall time: {solver.WallTime()}s");
            return response;
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
        public ConstraintResponseDto SolveCryptarithmetic()
        {
            CpModel model = new();

            // Define the variables.
            // In this particular puzzle, C, I, F, and T cannot be 0
            // since we don't begin a number with 0
            IntVar c = model.NewIntVar(1, 9, "C");
            IntVar p = model.NewIntVar(0, 9, "P");
            IntVar i = model.NewIntVar(1, 9, "I");
            IntVar s = model.NewIntVar(0, 9, "S");
            IntVar f = model.NewIntVar(1, 9, "F");
            IntVar u = model.NewIntVar(0, 9, "U");
            IntVar n = model.NewIntVar(0, 9, "N");
            IntVar t = model.NewIntVar(1, 9, "T");
            IntVar r = model.NewIntVar(0, 9, "R");
            IntVar e = model.NewIntVar(0, 9, "E");

            // Group the variables in a list to use the contraint AllDifferent
            var letters = new IntVar[] { c, p, i, s, f, u, n, t, r, e };

            // Define the constraints.
            model.AddAllDifferent(letters);

            // Add the constraint: CP + IS + FUN = TRUE
            model.Add(10 * c + p + 10 * i + s + 100 * f + 10 * u + n == 1000 * t + 100 * r + 10 * r + e);

            // Call the solver
            CpSolver solver = new();
            ConstraintResponseDto response = new();
            SolutionAggregator aggregator = new(response, letters);
            // Search for all solutions
            solver.StringParameters = "enumerate_all_solutions:true";
            CpSolverStatus status = solver.Solve(model, aggregator);

            response.Status = status switch
            {
                CpSolverStatus.Optimal => OptimizationStatus.Optimal,
                CpSolverStatus.Infeasible => OptimizationStatus.Infeasible,
                CpSolverStatus.Feasible => OptimizationStatus.Feasible,
                _ => OptimizationStatus.NotSolved
            };
            response.NumberOfConflicts = solver.NumConflicts();
            response.NumberOfBranches = solver.NumBranches();
            response.TimeToSolve = solver.WallTime();
            Debug.WriteLine("Statistics");
            Debug.WriteLine($"\tconflicts: {response.NumberOfConflicts}");
            Debug.WriteLine($"\tbranches: {response.NumberOfBranches}");
            Debug.WriteLine($"\twall time: {response.TimeToSolve}s");
            Debug.WriteLine($"\tnumber of solutions found: {response.Solutions.Count}");
            return response;
        }

        /// <summary>
        /// Demo solving the N-Queens problem using CP solver.
        /// This is a combinatorial problem based on the game of chess.
        /// The CP approach uses propagation and backtracking strategy to solve the problem.
        /// <para>How can N queens be placed on an NxN chessboard so that no two of them attack each other?</para>
        /// </summary>
        /// <param name="boardSize">Board size</param>
        /// <returns>
        /// An <see cref="ConstraintOptimizationService.Models.ConstraintResponseDto"/>
        /// </returns>
        public ConstraintResponseDto SolveNQueens(int boardSize, int? solutionLimit = null)
        {
            // Declare the model
            CpModel model = new();

            // Declare the size of the NxN matrix
            var queens = new IntVar[boardSize];
            for (int i = 0; i < boardSize; i++)
            {
                // A variable representing a queen in all possible columns in a row
                queens[i] = model.NewIntVar(0, boardSize - 1, $"x{i}");
            }

            // Define constraints.
            // All rows must be different.
            model.AddAllDifferent(queens);

            // All columns must be different because the indices of queens are all different.
            // No two queens can be on the same diagonal.
            var diag1 = new LinearExpr[boardSize];
            var diag2 = new LinearExpr[boardSize];
            for (int i = 0; i < boardSize; i++)
            {
                diag1[i] = LinearExpr.Affine(expr: queens[i], coeff: 1, offset: i);
                diag2[i] = LinearExpr.Affine(expr: queens[i], coeff: 1, offset: -i);
            }

            model.AddAllDifferent(diag1);
            model.AddAllDifferent(diag2);

            // Create a solver and solve the model.
            CpSolver solver = new();
            ConstraintResponseDto response = new();
            SolutionAggregator aggregator = new(response, queens, solutionLimit);

            // Search for all solutions.
            solver.StringParameters = "enumerate_all_solutions:true";

            // Instead of searching for all solutions,
            // we can set a time limit for the solver.
            // solver.StringParameters = "max_time_in_seconds:10.0";

            // For a complete list of string parameters
            // https://github.com/google/or-tools/blob/stable/ortools/sat/sat_parameters.proto

            // Solve
            CpSolverStatus status = solver.Solve(model, aggregator);

            response.Status = status switch
            {
                CpSolverStatus.Optimal => OptimizationStatus.Optimal,
                CpSolverStatus.Infeasible => OptimizationStatus.Infeasible,
                CpSolverStatus.Feasible => OptimizationStatus.Feasible,
                _ => OptimizationStatus.NotSolved
            };
            response.NumberOfConflicts = solver.NumConflicts();
            response.NumberOfBranches = solver.NumBranches();
            response.TimeToSolve = solver.WallTime();
            Debug.WriteLine("Statistics");
            Debug.WriteLine($"\tconflicts: {response.NumberOfConflicts}");
            Debug.WriteLine($"\tbranches: {response.NumberOfBranches}");
            Debug.WriteLine($"\twall time: {response.TimeToSolve}s");
            Debug.WriteLine($"\tnumber of solutions found: {response.Solutions.Count}");
            return response;
        }
    }
}
