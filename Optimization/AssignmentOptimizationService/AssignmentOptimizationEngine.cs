using System.Diagnostics;
using AssignmentOptimizationService.Interfaces;
using AssignmentOptimizationService.Models;
using Google.OrTools.Graph;
using Google.OrTools.LinearSolver;
using Google.OrTools.Sat;

namespace AssignmentOptimizationService
{
    /// <summary>
    /// Demonstrate problem solving using both MIP and CP-SAT solvers,
    /// Mixed Integer Problem solvers generally are faster at solving convex problems.
    /// Convex problems are ones in which a solution is one where all constraints must be satisfied.
    /// On the other hand, CP-SAT solvers are generally faster at solving non-convex problems.
    /// Non-convex problems are ones in which a feasible solution is one 
    /// where any of the constraints is satisfied.
    /// </summary>
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
            var variables = new Variable[numWorkers, numTasks];
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    variables[worker, task] = solver.MakeIntVar(0, 1, name: $"worker_{worker}_task_{task}");
                }
            }

            // Define the constraints.
            // Each worker is assigned zero or one task.
            for (int worker = 0; worker < numWorkers; worker++)
            {
                var constraint = solver.MakeConstraint(0, 1, "");
                for (int task = 0; task < numTasks; task++)
                {
                    constraint.SetCoefficient(variables[worker, task], 1);
                }
            }
            // Each task is assigned to exactly one worker.
            for (int task = 0; task < numTasks; task++)
            {
                var constraint = solver.MakeConstraint(1, 1, "");
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    constraint.SetCoefficient(variables[worker, task], 1);
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
            Solver.ResultStatus status = solver.Solve();

            if (status == Solver.ResultStatus.OPTIMAL || status == Solver.ResultStatus.FEASIBLE)
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
                response.Status = status == Solver.ResultStatus.OPTIMAL ?
                    OptimizationStatus.Optimal : OptimizationStatus.Feasible;
            }
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = status switch
                {
                    Solver.ResultStatus.UNBOUNDED => OptimizationStatus.Unbounded,
                    Solver.ResultStatus.INFEASIBLE => OptimizationStatus.Infeasible,
                    Solver.ResultStatus.NOT_SOLVED => OptimizationStatus.NotSolved,
                    _ => OptimizationStatus.NotSolved
                };
            }
            response.TimeToSolve = solver.WallTime();
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
                response.NumberOfConflicts = solver.NumConflicts();
                response.NumberOfBranches = solver.NumBranches();
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
            response.TimeToSolve = solver.WallTime();
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
                response.NumberOfConflicts = solver.NumConflicts();
                response.NumberOfBranches = solver.NumBranches();
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
            response.TimeToSolve = solver.WallTime();
            return response;
        }

        /// <summary>
        /// Demo solving an assignment problem with teams of workers 
        /// using the MIP (Mixed Integer Programming) solver.
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
        public AssignmentResponseDto SolveMultiTeamAssignmentWithMip()
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

            // Create the solver
            Solver solver = Solver.CreateSolver("SCIP");

            // Create the variables
            var variables = new Variable[numWorkers, numTasks];
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    variables[worker, task] = solver.MakeBoolVar($"worker_{worker}_task_{task}");
                }
            }

            // Create the constraints
            // Each worker is assigned to at most one task.
            for (int worker = 0; worker < numWorkers; worker++)
            {
                var constraint = solver.MakeConstraint(0, 1, "");
                for (int task = 0; task < numTasks; task++)
                {
                    constraint.SetCoefficient(variables[worker, task], 1);
                }
            }
            // Each task is assigned to exactly one worker.
            for (int task = 0; task < numTasks; task++)
            {
                var constraint = solver.MakeConstraint(1, 1, "");
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    constraint.SetCoefficient(variables[worker, task], 1);
                }
            }
            // Each team takes at most two tasks.
            var team1Tasks = solver.MakeConstraint(0, teamMaxTasks, "");
            foreach (int worker in team1)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    team1Tasks.SetCoefficient(variables[worker, task], 1);
                }
            }

            var team2Tasks = solver.MakeConstraint(0, teamMaxTasks, "");
            foreach (int worker in team2)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    team2Tasks.SetCoefficient(variables[worker, task], 1);
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

            // Create the solver
            Solver.ResultStatus status = solver.Solve();

            if (status == Solver.ResultStatus.OPTIMAL|| status == Solver.ResultStatus.FEASIBLE)
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
                response.Status = status == Solver.ResultStatus.OPTIMAL ?
                    OptimizationStatus.Optimal : OptimizationStatus.Feasible;
            }
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = status switch
                {
                    Solver.ResultStatus.UNBOUNDED => OptimizationStatus.Unbounded,
                    Solver.ResultStatus.INFEASIBLE => OptimizationStatus.Infeasible,
                    Solver.ResultStatus.NOT_SOLVED => OptimizationStatus.NotSolved,
                    _ => OptimizationStatus.NotSolved
                };
            }
            response.TimeToSolve = solver.WallTime();
            return response;
        }

        /// <summary>
        /// Demo solving an task size assignment problem using the CP-SAT (Constraint Programming - Satisfiability) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// zero or more tasks with a total maximum task size, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost, Task 5 Cost, Task 6 Cost, Task 7 Cost, Task 8 Cost
        /// 1       90           76           75           70           50           74           12           68
        /// 2       35           85           55           65           48           101          70           83
        /// 3       125          95           90           105          59           120          36           73
        /// 4       45           110          95           115          104          83           37           71
        /// 5       60           105          80           75           59           62           93           88
        /// 6       45           65           110          95           47           31           81           34
        /// 7       38           51           107          41           69           99           115          48
        /// 8       47           85           57           71           92           77           109          36
        /// 9       39           63           97           49           118          56           92           61
        /// 10      47           101          71           60           88           109          52           90
        /// 
        ///         Task 1 Size, Task 2 Size, Task 3 Size, Task 4 Size, Task 5 Size, Task 6 Size, Task 7 Size, Task 8 Size
        ///         10           7            3            12           15           4            11           5
        ///
        /// Maximu task size: 15
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        public AssignmentResponseDto SolveTaskSizeAssignmentWithCpSat()
        {
            AssignmentResponseDto response = new();

            const int numWorkers = 10;
            const int numTasks = 8;

            // Define the data.
            int[,] workerTaskCosts =
            {
                { 90, 76, 75, 70, 50, 74, 12, 68 },
                { 35, 85, 55, 65, 48, 101, 70, 83 },
                { 125, 95, 90, 105, 59, 120, 36, 73 },
                { 45, 110, 95, 115, 104, 83, 37, 71 },
                { 60, 105, 80, 75, 59, 62, 93, 88 },
                { 45, 65, 110, 95, 47, 31, 81, 34 },
                { 38, 51, 107, 41, 69, 99, 115, 48 },
                { 47, 85, 57, 71, 92, 77, 109, 36 },
                { 39, 63, 97, 49, 118, 56, 92, 61 },
                { 47, 101, 71, 60, 88, 109, 52, 90 }
            };

            // Task size represents how much time or effort the task requires
            int[] taskSizes = { 10, 7, 3, 12, 15, 4, 11, 5 };

            // Maximum total of task sizes for any worker
            int totalSizeMax = 15;

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

            // Add the constraints
            // Each worker is assigned to at most max task size
            for (int worker = 0; worker < numWorkers; worker++)
            {
                var taskSizeVariables = new BoolVar[numTasks];
                for (int task = 0; task < numTasks; task++)
                {
                    taskSizeVariables[task] = variables[worker, task];
                }
                model.Add(Google.OrTools.Sat.LinearExpr.WeightedSum(taskSizeVariables, coeffs: taskSizes) <= totalSizeMax);
            }
            // Each task is assigned to exactlyone worker.
            for (int task = 0; task < numTasks; task++)
            {
                var workers = new List<ILiteral>();
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    workers.Add(variables[worker, task]);
                }
                model.AddExactlyOne(workers);
            }

            // Create the objective
            LinearExprBuilder objective = Google.OrTools.Sat.LinearExpr.NewBuilder();
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    objective.AddTerm(variables[worker, task], workerTaskCosts[worker, task]);
                }
            }
            model.Minimize(objective);

            // Invoke the solver
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
                response.NumberOfConflicts = solver.NumConflicts();
                response.NumberOfBranches = solver.NumBranches();
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
            response.TimeToSolve = solver.WallTime();
            return response;
        }

        /// <summary>
        /// Demo solving an task size assignment problem using the MIP (Mixed Integer Programming) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// zero or more tasks with a total maximum task size, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost, Task 5 Cost, Task 6 Cost, Task 7 Cost, Task 8 Cost
        /// 1       90           76           75           70           50           74           12           68
        /// 2       35           85           55           65           48           101          70           83
        /// 3       125          95           90           105          59           120          36           73
        /// 4       45           110          95           115          104          83           37           71
        /// 5       60           105          80           75           59           62           93           88
        /// 6       45           65           110          95           47           31           81           34
        /// 7       38           51           107          41           69           99           115          48
        /// 8       47           85           57           71           92           77           109          36
        /// 9       39           63           97           49           118          56           92           61
        /// 10      47           101          71           60           88           109          52           90
        /// 
        ///         Task 1 Size, Task 2 Size, Task 3 Size, Task 4 Size, Task 5 Size, Task 6 Size, Task 7 Size, Task 8 Size
        ///         10           7            3            12           15           4            11           5
        ///
        /// Maximu task size: 15
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        public AssignmentResponseDto SolveTaskSizeAssignmentWithMip()
        {
            AssignmentResponseDto response = new();

            const int numWorkers = 10;
            const int numTasks = 8;

            // Define the data.
            int[,] workerTaskCosts =
            {
                { 90, 76, 75, 70, 50, 74, 12, 68 },
                { 35, 85, 55, 65, 48, 101, 70, 83 },
                { 125, 95, 90, 105, 59, 120, 36, 73 },
                { 45, 110, 95, 115, 104, 83, 37, 71 },
                { 60, 105, 80, 75, 59, 62, 93, 88 },
                { 45, 65, 110, 95, 47, 31, 81, 34 },
                { 38, 51, 107, 41, 69, 99, 115, 48 },
                { 47, 85, 57, 71, 92, 77, 109, 36 },
                { 39, 63, 97, 49, 118, 56, 92, 61 },
                { 47, 101, 71, 60, 88, 109, 52, 90 }
            };

            // Task size represents how much time or effort the task requires
            int[] taskSizes = { 10, 7, 3, 12, 15, 4, 11, 5 };

            // Maximum total of task sizes for any worker
            int totalSizeMax = 15;

            // Create the solver
            var solver = Solver.CreateSolver("SCIP");

            // Create the variables
            var variables = new Variable[numWorkers, numTasks];
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    variables[worker, task] = solver.MakeBoolVar($"worker_{worker}_task_{task}");
                }
            }

            // Add the constraints
            // Each worker is assigned to at most max task size
            for (int worker = 0; worker < numWorkers; worker++)
            {
                var constraint = solver.MakeConstraint(0, totalSizeMax, "");
                for (int task = 0; task < numTasks; task++)
                {
                    constraint.SetCoefficient(variables[worker, task], taskSizes[task]);
                }
            }
            // Each task is assigned to exactlyone worker.
            for (int task = 0; task < numTasks; task++)
            {
                var constraint = solver.MakeConstraint(1, 1, "");
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    constraint.SetCoefficient(variables[worker, task], 1);
                }
            }

            // Create the objective
            var objective = solver.Objective();
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    objective.SetCoefficient(variables[worker, task], workerTaskCosts[worker, task]);
                }
            }
            objective.SetMinimization();

            // Invoke the solver
            Solver.ResultStatus status = solver.Solve();

            if (status == Solver.ResultStatus.OPTIMAL || status == Solver.ResultStatus.FEASIBLE)
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
                response.Status = status == Solver.ResultStatus.OPTIMAL ?
                    OptimizationStatus.Optimal : OptimizationStatus.Feasible;
            }
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = status switch
                {
                    Solver.ResultStatus.UNBOUNDED => OptimizationStatus.Unbounded,
                    Solver.ResultStatus.INFEASIBLE => OptimizationStatus.Infeasible,
                    Solver.ResultStatus.NOT_SOLVED => OptimizationStatus.NotSolved,
                    _ => OptimizationStatus.NotSolved
                };
            }
            response.TimeToSolve = solver.WallTime();
            return response;
        }

        /// <summary>
        /// Demo solving an allowed groups assignment problem using the CP-SAT (Constraint Programming - Satisfiability) solver.
        /// <para>
        /// This is a problem in which only certain groups of workers can be assigned to the tasks.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost, Task 5 Cost, Task 6 Cost
        /// 1       90           76           75           70           50           74
        /// 2       35           85           55           65           48           101
        /// 3       125          95           90           105          59           120
        /// 4       45           110          95           115          104          83
        /// 5       60           105          80           75           59           62
        /// 6       45           65           110          95           47           31
        /// 7       38           51           107          41           69           99
        /// 8       47           85           57           71           92           77
        /// 9       39           63           97           49           118          56
        /// 10      47           101          71           60           88           109
        /// 11      17           39           103          64           61           92
        /// 12      101          45           83           59           92           27
        /// 
        /// group 1 (subgroups of workers 0 - 3): [2, 3], [1, 3], [1, 2], [0, 1], [0, 2]
        /// group 2 (subgroups of workers 4 - 7): [6, 7], [5, 7], [5, 6], [4, 5], [4, 7]
        /// group 3 (subgroups of workers 8 -11): [10, 11], [9, 11], [9, 10], [8, 10], [8, 11]
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        public AssignmentResponseDto SolveAllowedGroupsAssignmentWithCpSat()
        {
            AssignmentResponseDto response = new();

            // Define the data.
            const int numWorkers = 12;
            const int numTasks = 6;
            int[,] workerTaskCosts =
            {
                { 90, 76, 75, 70, 50, 74 },
                { 35, 85, 55, 65, 48, 101 },
                { 125, 95, 90, 105, 59, 120 },
                { 45, 110, 95, 115, 104, 83 },
                { 60, 105, 80, 75, 59, 62 },
                { 45, 65, 110, 95, 47, 31 },
                { 38, 51, 107, 41, 69, 90 },
                { 47, 85, 57, 71, 92, 77 },
                { 39, 63, 97, 49, 118, 56 },
                { 47, 101, 71, 60, 88, 109 },
                { 17, 39, 103, 64, 61, 92 },
                { 101, 45, 83, 59, 92, 27 },
            };

            // Define the allowed groups as binary vectors.
            // For example [0, 0, 1, 1] specifies a group containing workers 2 and 3.
            // First group of workers consists of workers 0 - 3
            long[,] group1 =
            {
                { 0, 0, 1, 1 }, // workers 2, 3
                { 0, 1, 0, 1 }, // workers 1, 3
                { 0, 1, 1, 0 }, // workers 1, 2
                { 1, 1, 0, 0 }, // workers 0, 1
                { 1, 0, 1, 0 }, // workers 0, 2
            };

            long[,] group2 =
            {
                { 0, 0, 1, 1 }, // workers 6, 7
                { 0, 1, 0, 1 }, // workers 5, 7
                { 0, 1, 1, 0 }, // workers 5, 6
                { 1, 1, 0, 0 }, // workers 4, 5
                { 1, 0, 0, 1 }, // workers 4, 7
            };

            long[,] group3 =
            {
                { 0, 0, 1, 1 }, // workers 10, 11
                { 0, 1, 0, 1 }, // workers 9, 11
                { 0, 1, 1, 0 }, // workers 9, 10
                { 1, 0, 1, 0 }, // workers 8, 10
                { 1, 0, 0, 1 }, // workers 8, 11
            };

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
            // Each worker is assigned to at most one task
            for (int worker = 0; worker < numWorkers; worker++)
            {
                var tasks = new List<ILiteral>();
                for (int task = 0; task < numTasks; task++)
                {
                    tasks.Add(variables[worker, task]);
                }
                model.AddAtMostOne(tasks);
            }
            // Each task is assigned to exactly one worker
            for (int task = 0; task < numTasks; task++)
            {
                var workers = new List<ILiteral>();
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    workers.Add(variables[worker, task]);
                }
                model.AddExactlyOne(workers);
            }

            // Create variables for each worker, indicating whether they work on some task.
            var work = new BoolVar[numWorkers];
            for (int worker = 0; worker < numWorkers; worker++)
            {
                work[worker] = model.NewBoolVar($"work_{worker}");
            }

            for (int worker = 0; worker < numWorkers; worker++)
            {
                var tasks = new List<ILiteral>();
                for (int task = 0; task < numTasks; task++)
                {
                    tasks.Add(variables[worker, task]);
                }
                model.Add(work[worker] == Google.OrTools.Sat.LinearExpr.Sum(tasks));
            }

            // Define the allowed groups of workers
            model.AddAllowedAssignments(new IntVar[] { work[0], work[1], work[2], work[3] }).AddTuples(group1);
            model.AddAllowedAssignments(new IntVar[] { work[4], work[5], work[6], work[7] }).AddTuples(group2);
            model.AddAllowedAssignments(new IntVar[] { work[8], work[9], work[10], work[11] }).AddTuples(group3);

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

            // Solve
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
                response.NumberOfConflicts = solver.NumConflicts();
                response.NumberOfBranches = solver.NumBranches();
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
            response.TimeToSolve = solver.WallTime();

            return response;
        }

        /// <summary>
        /// Demo solving an allowed groups assignment problem using the MIP (Mixed Integer Programming) solver.
        /// <para>
        /// This is a problem in which only certain groups of workers can be assigned to the tasks.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost, Task 5 Cost, Task 6 Cost
        /// 1       90           76           75           70           50           74
        /// 2       35           85           55           65           48           101
        /// 3       125          95           90           105          59           120
        /// 4       45           110          95           115          104          83
        /// 5       60           105          80           75           59           62
        /// 6       45           65           110          95           47           31
        /// 7       38           51           107          41           69           99
        /// 8       47           85           57           71           92           77
        /// 9       39           63           97           49           118          56
        /// 10      47           101          71           60           88           109
        /// 11      17           39           103          64           61           92
        /// 12      101          45           83           59           92           27
        /// 
        /// group 1 (subgroups of workers 0 - 3): [2, 3], [1, 3], [1, 2], [0, 1], [0, 2]
        /// group 2 (subgroups of workers 4 - 7): [6, 7], [5, 7], [5, 6], [4, 5], [4, 7]
        /// group 3 (subgroups of workers 8 -11): [10, 11], [9, 11], [9, 10], [8, 10], [8, 11]
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        public AssignmentResponseDto SolveAllowedGroupsAssignmentWithMip()
        {
            AssignmentResponseDto response = new();

            // Define the data.
            const int numWorkers = 12;
            const int numTasks = 6;
            int[,] workerTaskCosts =
            {
                { 90, 76, 75, 70, 50, 74 },
                { 35, 85, 55, 65, 48, 101 },
                { 125, 95, 90, 105, 59, 120 },
                { 45, 110, 95, 115, 104, 83 },
                { 60, 105, 80, 75, 59, 62 },
                { 45, 65, 110, 95, 47, 31 },
                { 38, 51, 107, 41, 69, 90 },
                { 47, 85, 57, 71, 92, 77 },
                { 39, 63, 97, 49, 118, 56 },
                { 47, 101, 71, 60, 88, 109 },
                { 17, 39, 103, 64, 61, 92 },
                { 101, 45, 83, 59, 92, 27 },
            };

            // Define the allowed groups
            int[,] group1 =
            {
                { 2, 3 }, // workers 2, 3
                { 1, 3 }, // workers 1, 3
                { 1, 2 }, // workers 1, 2
                { 1, 2 }, // workers 0, 1
                { 0, 1 }, // workers 0, 2
            };

            int[,] group2 =
            {
                { 6, 7 }, // workers 6, 7
                { 5, 7 }, // workers 5, 7
                { 5, 6 }, // workers 5, 6
                { 4, 5 }, // workers 4, 5
                { 4, 7 }, // workers 4, 7
            };

            int[,] group3 =
            {
                { 10, 11 }, // workers 10, 11
                { 9, 11 }, // workers 9, 11
                { 9, 10 }, // workers 9, 10
                { 8, 10 }, // workers 8, 10
                { 8, 11 }, // workers 8, 11
            };

            // Create the solver
            var solver = Solver.CreateSolver("SCIP");

            // Create the variables
            var variables = new Variable[numWorkers, numTasks];
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    variables[worker, task] = solver.MakeBoolVar($"worker_{worker}_task_{task}");
                }
            }

            // Create the constraints
            // Each worker is assigned to at most one task
            for (int worker = 0; worker < numWorkers; worker++)
            {
                var constraint = solver.MakeConstraint(0, 1, "");
                for (int task = 0; task < numTasks; task++)
                {
                    constraint.SetCoefficient(variables[worker, task], 1);
                }
            }
            // Each task is assigned to exactly one worker
            for (int task = 0; task < numTasks; task++)
            {
                var constraint = solver.MakeConstraint(1, 1, "");
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    constraint.SetCoefficient(variables[worker, task], 1);
                }
            }

            // Create variables for each worker, indicating whether they work on some task.
            var work = new Variable[numWorkers];
            for (int worker = 0; worker < numWorkers; worker++)
            {
                work[worker] = solver.MakeBoolVar($"work_{worker}");
            }

            for (int worker = 0; worker < numWorkers; worker++)
            {
                var tasks = new Variable[numTasks];
                for (int task = 0; task < numTasks; task++)
                {
                    tasks[task] = variables[worker, task];
                }
                solver.Add(work[worker] == LinearExprArrayHelper.Sum(tasks));
            }

            // Group 1 constraint
            var constraintG1 = solver.MakeConstraint(1, 1, "");
            for (int i = 0; i < group1.GetLength(0); i++)
            {
                // a*b can be transformed into 0 <= a + b - 2*p <=1 with p in [0, 1]
                // p is true if a AND b, false otherwise
                var constraint = solver.MakeConstraint(0, 1, "");
                constraint.SetCoefficient(work[group1[i, 0]], 1);
                constraint.SetCoefficient(work[group1[i, 1]], 1);
                Variable p = solver.MakeBoolVar($"group1_p{i}");
                constraint.SetCoefficient(p, -2);
                constraintG1.SetCoefficient(p, 1);
            }
            // Group 2 constraint
            var constraintG2 = solver.MakeConstraint(1, 1, "");
            for (int i = 0; i < group2.GetLength(0); i++)
            {
                // a*b can be transformed into 0 <= a + b - 2*p <=1 with p in [0, 1]
                // p is true if a AND b, false otherwise
                var constraint = solver.MakeConstraint(0, 1, "");
                constraint.SetCoefficient(work[group2[i, 0]], 1);
                constraint.SetCoefficient(work[group2[i, 1]], 1);
                Variable p = solver.MakeBoolVar($"group2_p{i}");
                constraint.SetCoefficient(p, -2);
                constraintG2.SetCoefficient(p, 1);
            }
            // Group 3 constraint
            var constraintG3 = solver.MakeConstraint(1, 1, "");
            for (int i = 0; i < group3.GetLength(0); i++)
            {
                // a*b can be transformed into 0 <= a + b - 2*p <=1 with p in [0, 1]
                // p is true if a AND b, false otherwise
                var constraint = solver.MakeConstraint(0, 1, "");
                constraint.SetCoefficient(work[group3[i, 0]], 1);
                constraint.SetCoefficient(work[group3[i, 1]], 1);
                Variable p = solver.MakeBoolVar($"group3_p{i}");
                constraint.SetCoefficient(p, -2);
                constraintG3.SetCoefficient(p, 1);
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

            // Invoke the solver
            Solver.ResultStatus status = solver.Solve();

            if (status == Solver.ResultStatus.OPTIMAL || status == Solver.ResultStatus.FEASIBLE)
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
                response.Status = status == Solver.ResultStatus.OPTIMAL ?
                    OptimizationStatus.Optimal : OptimizationStatus.Feasible;
            }
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = status switch
                {
                    Solver.ResultStatus.UNBOUNDED => OptimizationStatus.Unbounded,
                    Solver.ResultStatus.INFEASIBLE => OptimizationStatus.Infeasible,
                    Solver.ResultStatus.NOT_SOLVED => OptimizationStatus.NotSolved,
                    _ => OptimizationStatus.NotSolved
                };
            }
            response.TimeToSolve = solver.WallTime();
            return response;
        }

        /// <summary>
        /// Demo solving an assignment problem using the Linear Sum Assignment solver.
        /// This is a specialized solver for assignment problems and is faster than the MIP and CP-SAT
        /// solvers for the assignment problem.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// at most one task, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost
        /// 1       90           76           75           70
        /// 2       35           85           55           65
        /// 3       125          95           90           105
        /// 4       45           110          95           115
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        public AssignmentResponseDto SolveAssignmentWithLinearSumAssignmentSolver()
        {
            AssignmentResponseDto response = new();

            // Define the data
            const int numWorkers = 4;
            const int numTasks = 4;
            int[,] workerTaskCosts =
            {
                { 90, 76, 75, 70 },
                { 35, 85, 55, 65 },
                { 125, 95, 90, 105 },
                { 45, 110, 95, 115 },
            };

            // Create the solver
            var assignment = new LinearSumAssignment();

            // Create the arcs
            for (int worker = 0; worker < numWorkers; worker++)
            {
                for (int task = 0; task < numTasks; task++)
                {
                    // Conditionally set assignments.
                    // In this scenario, if we had set the cost to 0, it means the worker cannot do the task at all.
                    if (workerTaskCosts[worker, task] > 0)
                    {
                        assignment.AddArcWithCost(worker, task, workerTaskCosts[worker, task]);
                    }
                }
            }

            // Solve
            LinearSumAssignment.Status status = assignment.Solve();
            if (status == LinearSumAssignment.Status.OPTIMAL)
            {
                Debug.WriteLine($"Total cost: {assignment.OptimalCost()}");
                for (int worker = 0; worker < numWorkers; worker++)
                {
                    Debug.WriteLine($"Worker {worker} assigned to task {assignment.RightMate(worker)}. Cost {assignment.AssignmentCost(worker)}");
                    response.Variables.Add(
                        new VariableResponseDto
                        {
                            Name = $"worker_{worker}_task_{assignment.RightMate(worker)}",
                            Solution = assignment.AssignmentCost(worker)
                        }
                    );
                }
                response.Status = OptimizationStatus.Optimal;
            } 
            else
            {
                Debug.WriteLine("No solution found.");
                response.Status = status switch
                {
                    LinearSumAssignment.Status.POSSIBLE_OVERFLOW => OptimizationStatus.PossibleOverflow,
                    LinearSumAssignment.Status.INFEASIBLE => OptimizationStatus.Infeasible,
                    _ => OptimizationStatus.NotSolved
                };
            }

            return response;
        }
    }
}
