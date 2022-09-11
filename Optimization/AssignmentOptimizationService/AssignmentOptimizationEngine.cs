using System.Diagnostics;
using AssignmentOptimizationService.Interfaces;
using AssignmentOptimizationService.Models;
using Google.OrTools.LinearSolver;
using Google.OrTools.Sat;

namespace AssignmentOptimizationService
{
    public class AssignmentOptimizationEngine : IAssignmentOptimization
    {
        /// <summary>
        /// Demo solving an assignment problem using the MIP (Mixed-Integer Programming) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// at most one task, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost
        /// 1       90           80           75           70
        /// 2       35           85           55           65
        /// 3       125          95           90           95
        /// 4       45           110          95           115
        /// 5       50           100          90           100
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        public AssignmentResponseDto SolveAssignmentWithMip()
        {
            AssignmentResponseDto response = new();

            // Define the data
            const int numWorkers = 5;
            const int numTasks = 4;
            int[,] workerTaskCosts =
            {
                { 90, 80, 75, 70 },
                { 35, 85, 55, 65 },
                { 125, 95, 90, 95 },
                { 45, 110, 95, 115 },
                { 50, 100, 90, 100 }
            };

            // Create the solver.
            var solver = Solver.CreateSolver("SCIP");

            // Define a matrix of worker, task variables.
            const int min = 0;
            const int max = 1;
            var variables = new Variable[numWorkers, numTasks];
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    variables[worker, task] = solver.MakeIntVar(min, max, name: $"worker_{worker}_task_{task}");
                }
            }

            // Define the constraints.
            // Each worker is assigned zero or one task.
            for (int worker = 0; worker < numWorkers; worker++)
            {
                var constraint = solver.MakeConstraint(min, max, "");
                for (int task = 0; task < numTasks; task++)
                {
                    constraint.SetCoefficient(variables[worker, task], max);
                }
            }
            // Each task is assigned to exactly one worker.
            for (int task = 0; task < numTasks; task++)
            {
                var constraint = solver.MakeConstraint(max, max, "");
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    constraint.SetCoefficient(variables[worker, task], max);
                }
            }

            // Define the objective
            var objective = solver.Objective();
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    objective.SetCoefficient(variables[worker, task], workerTaskCosts[worker, task]);
                }
            }
            objective.SetMinimization();

            // Solve
            Solver.ResultStatus resultStatus = solver.Solve();

            if (resultStatus == Solver.ResultStatus.OPTIMAL || resultStatus == Solver.ResultStatus.FEASIBLE)
            {
                Debug.WriteLine($"Total cost: {solver.Objective().Value()}");
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    for (int task = 0; task < numTasks; task++)
                    {
                        if (variables[worker, task].SolutionValue() > 0.5)
                        {
                            Debug.WriteLine($"Worker {worker} assigned to task {task}. Cost {workerTaskCosts[worker, task]}");
                            response.Variables.Add(
                                new VariableResponseDto
                                {
                                    Name = variables[worker, task].Name(),
                                    Solution = workerTaskCosts[worker, task]
                                }
                            );
                        }
                    }
                }
                response.Status = resultStatus == Solver.ResultStatus.OPTIMAL ?
                    OptimizationStatus.Optimal : OptimizationStatus.Feasible;
            }
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = resultStatus switch
                {
                    Solver.ResultStatus.UNBOUNDED => OptimizationStatus.Unbounded,
                    Solver.ResultStatus.INFEASIBLE => OptimizationStatus.Infeasible,
                    Solver.ResultStatus.NOT_SOLVED => OptimizationStatus.NotSolved,
                    _ => OptimizationStatus.NotSolved
                };
            }
            return response;
        }

        /// <summary>
        /// Demo solving an assignment problem using the CP-SAT (Constraint Programming - Satisfiability) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// at most one task, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost
        /// 1       90           80           75           70
        /// 2       35           85           55           65
        /// 3       125          95           90           95
        /// 4       45           110          95           115
        /// 5       50           100          90           100
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        public AssignmentResponseDto SolveAssignmentWithCpSat()
        {
            AssignmentResponseDto response = new();

            // Define the data.
            const int numWorkers = 5;
            const int numTasks = 4;
            int[,] workerTaskCosts =
            {
                { 90, 80, 75, 70 },
                { 35, 85, 55, 65 },
                { 125, 95, 90, 95 },
                { 45, 110, 95, 115 },
                { 50, 100, 90, 100 }
            };

            // Define the solver.
            var model = new CpModel();

            // Define a matrix of worker, task variables.
            var variables = new BoolVar[numWorkers, numTasks];
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    variables[worker, task] = model.NewBoolVar(name: $"worker_{worker}_task_{task}");
                }
            }

            // Define the constraints.
            // Each worker is assigned at most one task.
            for (int worker = 0; worker < numWorkers; worker++)
            {
                var tasks = new List<ILiteral>();
                for (int task = 0; task < numTasks; task++)
                {
                    tasks.Add(variables[worker, task]);
                }
                model.AddAtMostOne(tasks);
            }
            // Each task is assigned to exactly one worker.
            for (int task = 0; task < numTasks; task++)
            {
                var workers = new List<ILiteral>();
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    workers.Add(variables[worker, task]);
                }
                model.AddExactlyOne(workers);
            }

            // Define the objective
            LinearExprBuilder objective = Google.OrTools.Sat.LinearExpr.NewBuilder();
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    objective.AddTerm((IntVar)variables[worker, task], workerTaskCosts[worker, task]);
                }
            }
            model.Minimize(objective);

            // Solve
            var solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Debug.WriteLine($"Total cost: {solver.ObjectiveValue}");
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    for (int task = 0; task < numTasks; task++)
                    {
                        if (solver.Value(variables[worker, task]) > 0.5)
                        {
                            Debug.WriteLine($"Worker {worker} assigned to task {task}. Cost {workerTaskCosts[worker, task]}");
                            response.Variables.Add(
                                new VariableResponseDto
                                {
                                    Name = variables[worker, task].Name(),
                                    Solution = workerTaskCosts[worker, task]
                                }
                            );
                        }
                    }
                }
                response.Status = status == CpSolverStatus.Optimal ?
                    OptimizationStatus.Optimal : OptimizationStatus.Feasible;
            }
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = status switch
                {
                    CpSolverStatus.Infeasible => OptimizationStatus.Infeasible,
                    CpSolverStatus.ModelInvalid => OptimizationStatus.ModelInvalid,
                    CpSolverStatus.Unknown => OptimizationStatus.Unknown,
                    _ => OptimizationStatus.Unknown
                };
            }
            return response;
        }

        /// <summary>
        /// Demo solving an assignment problem with teams of workers 
        /// using the CP-SAT (Constraint Programming - Satisfiability) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by multiple teams of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// at most one task, no two workers can perform the same task, and
        /// each team can perform at most two tasks.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost
        /// 1       90           76           75           70
        /// 2       35           85           55           65
        /// 3       125          95           90           105
        /// 4       45           110          95           115
        /// 5       60           105          80           75
        /// 6       45           65           110          95
        /// 
        /// Team 1: worker 1, 3, 5
        /// Team 2: worker 2, 4, 6
        /// Max number of tasks for any team: 2
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        public AssignmentResponseDto SolveMultiTeamAssignmentWithCpSat()
        {
            AssignmentResponseDto response = new();

            const int numWorkers = 6;
            const int numTasks = 4;

            // Define the data
            int[,] workerTaskCosts =
            {
                { 90, 76, 75, 70 },
                { 35, 85, 55, 65 },
                { 125, 95, 90, 105 },
                { 45, 110, 95, 115 },
                { 60, 105, 80, 75 },
                { 45, 65, 110, 95 }
            };

            int[] team1 = { 0, 2, 4 };
            int[] team2 = { 1, 3, 5 };

            // Maximum total of tasks for any team
            int teamMaxTasks = 2;

            // Create the model
            CpModel model = new();

            // Create the variables
            var variables = new BoolVar[numWorkers, numTasks];
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    variables[worker, task] = model.NewBoolVar($"worker_{worker}_task_{task}");
                }
            }

            // Create the constraints
            // Each worker is assigned to at most one task.
            for (int worker = 0; worker < numWorkers; worker++)
            {
                var tasks = new List<ILiteral>();
                for (int task = 0; task < numTasks; task++)
                {
                    tasks.Add(variables[worker, task]);
                }
                model.AddAtMostOne(tasks);
            }
            // Each task is assigned to exactly one worker.
            for (int task = 0; task < numTasks; task++)
            {
                var workers = new List<ILiteral>();
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    workers.Add(variables[worker, task]);
                }
                model.AddExactlyOne(workers);
            }
            // Each team takes at most two tasks.
            var team1Tasks = new List<IntVar>();
            foreach (int worker in team1)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    team1Tasks.Add(variables[worker, task]);
                }
            }
            model.Add(Google.OrTools.Sat.LinearExpr.Sum(team1Tasks.ToArray()) <= teamMaxTasks);

            var team2Tasks = new List<IntVar>();
            foreach (int worker in team2)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    team2Tasks.Add(variables[worker, task]);
                }
            }
            model.Add(Google.OrTools.Sat.LinearExpr.Sum(team2Tasks.ToArray()) <= teamMaxTasks);

            // Define the objective
            LinearExprBuilder objective = Google.OrTools.Sat.LinearExpr.NewBuilder();
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    objective.AddTerm(variables[worker, task], workerTaskCosts[worker, task]);
                }
            }
            model.Minimize(objective);

            // Create the solver
            CpSolver solver = new();
            CpSolverStatus status = solver.Solve(model);

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Debug.WriteLine($"Total cost: {solver.ObjectiveValue}");
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    for (int task = 0; task < numTasks; task++)
                    {
                        if (solver.Value(variables[worker, task]) > 0.5)
                        {
                            Debug.WriteLine($"Worker {worker} assigned to task {task}. Cost {workerTaskCosts[worker, task]}");
                            response.Variables.Add(
                                new VariableResponseDto
                                {
                                    Name = variables[worker, task].Name(),
                                    Solution = workerTaskCosts[worker, task]
                                }
                            );
                        }
                    }
                }
                response.Status = status == CpSolverStatus.Optimal ?
                    OptimizationStatus.Optimal : OptimizationStatus.Feasible;
            }
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = status switch
                {
                    CpSolverStatus.Infeasible => OptimizationStatus.Infeasible,
                    CpSolverStatus.ModelInvalid => OptimizationStatus.ModelInvalid,
                    CpSolverStatus.Unknown => OptimizationStatus.Unknown,
                    _ => OptimizationStatus.Unknown
                };
            }
            return response;
        }
    }
}
