using System.Diagnostics;
using Google.OrTools.Sat;
using SchedulingOptimizationService.Enums;
using SchedulingOptimizationService.Helpers;
using SchedulingOptimizationService.Interfaces;
using SchedulingOptimizationService.Models;

namespace SchedulingOptimizationService
{
    public class SchedulingOptimizationEngine : ISchedulingOptimization
    {
        /// <summary>
        /// Demo solving a scheduling problem with Constraint Programming - Satisfiability (CP-SAT).
        /// <para>
        /// We need to create a schedule for four employees over a three day period.
        /// The constraints are:
        /// </para>
        /// <list type="bullet">
        ///     <item>Each day is divided into three 8 hour shifts.</item>
        ///     <item>Every day, each shift is assigned to a single employee, and no employee works more than one shift.</item>
        ///     <item>Each employee is assigned to at least two shifts during the three day period.</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public SchedulingResponseDto SolveEmployeeScheduling()
        {
            var response = new SchedulingResponseDto();

            const int numEmployees = 4;
            const int numDays = 3;
            const int numShifts = 3;

            int[] allEmployees = Enumerable.Range(0, numEmployees).ToArray();
            int[] allDays = Enumerable.Range(0, numDays).ToArray();
            int[] allShifts = Enumerable.Range(0, numShifts).ToArray();

            // Create the model
            var model = new CpModel();
            model.Model.Variables.Capacity = numEmployees * numDays * numShifts;

            // Create variables for each shift.
            // shifts[(employee, day, shift)] = 1 if 'employee' works 'shift' on 'day', 0 otherwise.
            var shifts = new Dictionary<(int employee, int day, int shift), BoolVar>(numEmployees * numDays * numShifts);
            foreach (int employee in allEmployees)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        shifts.Add((employee, day, shift), model.NewBoolVar($"shifts_employee{employee}day{day}shift{shift}"));
                    }
                }
            }

            // Each shift is assigned to exaclty one employee in the schedule period.
            var literals = new List<ILiteral>();
            foreach (int day in allDays)
            {
                foreach (int shift in allShifts)
                {
                    foreach (int employee in allEmployees)
                    {
                        literals.Add(shifts[(employee, day, shift)]);
                    }
                    model.AddExactlyOne(literals);
                    literals.Clear();
                }
            }

            // Each employee works at most one shift per day.
            foreach (int employee in allEmployees)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        literals.Add(shifts[(employee, day, shift)]);
                    }
                    model.AddAtMostOne(literals);
                    literals.Clear();
                }
            }

            // Distribute shifts evenly.  Some employees may be assigned one more shift than others.
            int minShiftsPerEmployee = (numShifts * numDays) / numEmployees;
            int maxShiftsPerEmployee = (numShifts * numDays) % numEmployees == 0 ? minShiftsPerEmployee : minShiftsPerEmployee + 1;

            var numShiftsWorked = new List<IntVar>();
            foreach (int employee in allEmployees)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        numShiftsWorked.Add(shifts[(employee, day, shift)]);
                    }
                }
                // No employee is assigned more than one extra shift
                model.AddLinearConstraint(LinearExpr.Sum(numShiftsWorked), minShiftsPerEmployee, maxShiftsPerEmployee);
                numShiftsWorked.Clear();
            }

            // Create the solver
            var solver = new CpSolver();
            // Tell solver to enumate all solutions.
            solver.StringParameters += "linearization_level:0 " + "enumerate_all_solutions:true ";

            // Display the first five solutions
            const int solutionLimit = 5;

            var solutionCallBack = new SolutionAggregator(
                response,
                allEmployees,
                allDays,
                allShifts,
                shifts,
                solutionLimit);

            // Solve
            CpSolverStatus status = solver.Solve(model, solutionCallBack);

            response.Status = status switch
            {
                CpSolverStatus.Optimal => OptimizationStatus.Optimal,
                CpSolverStatus.Feasible => OptimizationStatus.Feasible,
                CpSolverStatus.ModelInvalid => OptimizationStatus.ModelInvalid,
                CpSolverStatus.Infeasible => OptimizationStatus.Infeasible,
                CpSolverStatus.Unknown => OptimizationStatus.Unknown,
                _ => OptimizationStatus.Unknown
            };
            Debug.WriteLine($"Solve status: {status}");

            response.NumberOfBranches = solver.NumConflicts();
            response.NumberOfBranches = solver.NumBranches();
            response.TimeToSolve = solver.WallTime();

            Debug.WriteLine("Statistics");
            Debug.WriteLine($"\tconflicts: {solver.NumConflicts()}");
            Debug.WriteLine($"\tbranches: {solver.NumBranches()}");
            Debug.WriteLine($"\twall time: {solver.WallTime()}s");
            return response;
        }

        /// <summary>
        /// Demo solving a scheduling problem with shift requests using Constraint Programming - Satisfiability (CP-SAT).
        /// <para>
        /// We need to create a schedule for five employees over a seven day period.
        /// The constraints are:
        /// </para>
        /// <list type="bullet">
        ///     <item>Each day is divided into three 8 hour shifts.</item>
        ///     <item>Every day, each shift is assigned to a single employee, and no employee works more than one shift.</item>
        ///     <item>Each employee is assigned to at least two shifts during the three day period.</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public SchedulingResponseDto SolveEmployeeSchedulingWithShiftRequests()
        {
            var response = new SchedulingResponseDto();

            const int numEmployees = 5;
            const int numDays = 7;
            const int numShifts = 3;

            int[] allEmployees = Enumerable.Range(0, numEmployees).ToArray();
            int[] allDays = Enumerable.Range(0, numDays).ToArray();
            int[] allShifts = Enumerable.Range(0, numShifts).ToArray();

            int[,,] shiftRequests = new int[,,] 
            {
                {
                    { 0, 0, 1 },
                    { 0, 0, 0 },
                    { 0, 0, 0 },
                    { 0, 0, 0 },
                    { 0, 0, 1 },
                    { 0, 1, 0 },
                    { 0, 0, 1 },
                },
                {
                    { 0, 0, 0 },
                    { 0, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 1, 0 },
                    { 1, 0, 0 },
                    { 0, 0, 0 },
                    { 0, 0, 1 },
                },
                {
                    { 0, 1, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 0 },
                    { 1, 0, 0 },
                    { 0, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 0 },
                },
                {
                    { 0, 0, 1 },
                    { 0, 0, 0 },
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 0 },
                    { 1, 0, 0 },
                    { 0, 0, 0 },
                },
                {
                    { 0, 0, 0 },
                    { 0, 0, 1 },
                    { 0, 1, 0 },
                    { 0, 0, 0 },
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 0 },
                },
            };

            // Create the model
            var model = new CpModel();
            model.Model.Variables.Capacity = numEmployees * numDays * numShifts;

            // Create variables for each shift.
            // shifts[(employee, day, shift)] = 1 if 'employee' works 'shift' on 'day', 0 otherwise.
            var shifts = new Dictionary<(int employee, int day, int shift), BoolVar>(numEmployees * numDays * numShifts);
            foreach (int employee in allEmployees)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        shifts.Add((employee, day, shift), model.NewBoolVar($"shifts_employee{employee}day{day}shift{shift}"));
                    }
                }
            }

            // Each shift is assigned to exaclty one employee in the schedule period.
            foreach (int day in allDays)
            {
                foreach (int shift in allShifts)
                {
                    var employeeAssignment = new IntVar[numEmployees];
                    foreach (int employee in allEmployees)
                    {
                        employeeAssignment[employee] = shifts[(employee, day, shift)];
                    }
                    model.Add(LinearExpr.Sum(employeeAssignment) == 1);
                }
            }

            // Each employee works at most one shift per day.
            foreach (int employee in allEmployees)
            {
                foreach (int day in allDays)
                {
                    var shiftAssignment = new IntVar[numShifts];
                    foreach (int shift in allShifts)
                    {
                        shiftAssignment[shift] = shifts[(employee, day, shift)];
                    }
                    model.Add(LinearExpr.Sum(shiftAssignment) <= 1);
                }
            }

            // Distribute shifts evenly.  Some employees may be assigned one more shift than others.
            int minShiftsPerEmployee = (numShifts * numDays) / numEmployees;
            int maxShiftsPerEmployee = (numShifts * numDays) % numEmployees == 0 ? minShiftsPerEmployee : minShiftsPerEmployee + 1;

            foreach (int employee in allEmployees)
            {
                var numShiftsWorked = new IntVar[numDays * numShifts];
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        numShiftsWorked[day * numShifts + shift] = shifts[(employee, day, shift)];
                    }
                }
                // No employee is assigned more than one extra shift
                model.AddLinearConstraint(LinearExpr.Sum(numShiftsWorked), minShiftsPerEmployee, maxShiftsPerEmployee);
            }

            var flatShifts = new IntVar[numEmployees * numDays * numShifts];
            var flatShiftRequests = new int[numEmployees * numDays * numShifts];
            foreach (int employee in allEmployees)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        flatShifts[employee * numDays * numShifts + day * numShifts + shift] = shifts[(employee, day, shift)];
                        flatShiftRequests[employee * numDays * numShifts + day * numShifts + shift] = shiftRequests[employee, day, shift];
                    }
                }
            }
            // Maximize the number shift of assignments.
            model.Maximize(LinearExpr.WeightedSum(flatShifts, flatShiftRequests));

            // Create the solver
            var solver = new CpSolver();

            // Solve
            CpSolverStatus status = solver.Solve(model);
            response.Status = status switch
            {
                CpSolverStatus.Optimal => OptimizationStatus.Optimal,
                CpSolverStatus.Feasible => OptimizationStatus.Feasible,
                CpSolverStatus.ModelInvalid => OptimizationStatus.ModelInvalid,
                CpSolverStatus.Infeasible => OptimizationStatus.Infeasible,
                CpSolverStatus.Unknown => OptimizationStatus.Unknown,
                _ => OptimizationStatus.Unknown
            };
            Debug.WriteLine($"Solve status: {status}");

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Debug.WriteLine("Solution:");
                var solution = new SolutionDto();
                foreach (int day in allDays)
                {
                    Debug.WriteLine($"Day {day}");
                    foreach (int employee in allEmployees)
                    {
                        foreach (int shift in allShifts)
                        {
                            if (solver.Value(shifts[(employee, day, shift)]) == 1)
                            {
                                if (shiftRequests[employee, day, shift] == 1)
                                {
                                    Debug.WriteLine($"Employee {employee} work shift {shift} (requested).");
                                    solution.Variables.Add(new VariableResponseDto
                                    {
                                        Day = day,
                                        Employee = employee,
                                        Shift = shift,
                                        Requested = true
                                    });
                                }
                                else
                                {
                                    Debug.WriteLine($"Employee {employee} work shift {shift} (not requested).");
                                    solution.Variables.Add(new VariableResponseDto
                                    {
                                        Day = day,
                                        Employee = employee,
                                        Shift = shift,
                                        Requested = false
                                    });
                                }
                                solution.TimeToSolve = solver.WallTime();
                            }
                        }
                    }
                }
                Debug.WriteLine($"Number of shift requests met = {solver.ObjectiveValue} (out of {numEmployees * minShiftsPerEmployee}.");
                response.Solutions.Add(solution);
            }
            else
            {
                Debug.WriteLine("No solution found.");
            }

            response.ObjectiveValue = solver.ObjectiveValue;

            response.NumberOfBranches = solver.NumConflicts();
            response.NumberOfBranches = solver.NumBranches();
            response.TimeToSolve = solver.WallTime();

            Debug.WriteLine("Statistics");
            Debug.WriteLine($"\tconflicts: {solver.NumConflicts()}");
            Debug.WriteLine($"\tbranches: {solver.NumBranches()}");
            Debug.WriteLine($"\twall time: {solver.WallTime()}s");
            return response;
        }

        /// <summary>
        /// Demo solving a "job shop" scheduling problem, where there are multiple jobs and multiple machines.
        /// <para>
        /// Minimize the total time to complete the jobs with the following constraints:
        /// </para>
        /// <list type="bullet">
        ///     <item>No task for a job can started until the previous task for that job is completed.</item>
        ///     <item>A machine can only work on one task at a time.</item>
        ///     <item>A task, once started, must run to completion.</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public AssignedSchedulingResponseDto SolveJobShopProblem()
        {
            var response = new AssignedSchedulingResponseDto();
            var allJobs = new List<List<(int machine, int duration)>>
                {
                    new List<(int machine, int duration)> 
                    {
                        // job0
                        (0, 3), // task0
                        (1, 2), // task1
                        (2, 2), // task2
                    },
                    new List<(int machine, int duration)>
                    {
                        // job1
                        (0, 2), // task0
                        (2, 1), // task1
                        (1, 4), // task2
                    },
                    new List<(int machine, int duration)>
                    {
                        // job2
                        (1, 4), // task0
                        (2, 3), // task1
                    },
                };

            const int numMachines = 3;
            int[] allMachines = Enumerable.Range(0, numMachines).ToArray();

            // Compute the horizon.  This is the maximum length of time if no jobs were to overlap.
            // Therefore, it is the sum of all durations.
            int horizon = 0;
            foreach (var job in allJobs)
            {
                foreach (var (machine, duration) in job)
                {
                    horizon += duration;
                }
            }

            // Create the model.
            var model = new CpModel();

            var allTasks = new Dictionary<(int job, int task), (IntVar start, IntVar end, IntervalVar duration)>();
            var machineToIntervals = new Dictionary<int, List<IntervalVar>>();

            // Create the variables.
            for (int jobIndex = 0; jobIndex < allJobs.Count; jobIndex++)
            {
                var job = allJobs[jobIndex];
                for (int taskIndex = 0; taskIndex < allJobs[jobIndex].Count; taskIndex++)
                {
                    var task = job[taskIndex];
                    var start = model.NewIntVar(0, horizon, $"start_{jobIndex}_{taskIndex}");
                    var end = model.NewIntVar(0, horizon, $"end_{jobIndex}_{taskIndex}");
                    var interval = model.NewIntervalVar(start, task.duration, end, $"interval_{jobIndex}_{taskIndex}");
                    allTasks[(jobIndex, taskIndex)] = (start, end, interval);
                    if (!machineToIntervals.ContainsKey(task.machine))
                    {
                        machineToIntervals.Add(task.machine, new List<IntervalVar>());
                    }
                    machineToIntervals[task.machine].Add(interval);
                }
            }

            // Create disjunctive (no overlap) constraints.
            foreach (int machine in allMachines)
            {
                model.AddNoOverlap(machineToIntervals[machine]);
            }

            // Create precedent constraints.
            for (int jobIndex = 0; jobIndex < allJobs.Count; jobIndex++)
            {
                var job = allJobs[jobIndex];
                for (int taskIndex = 0; taskIndex < job.Count - 1; taskIndex++)
                {
                    var key = (jobIndex, taskIndex);
                    var nextKey = (jobIndex, taskIndex + 1);
                    model.Add(allTasks[nextKey].start >= allTasks[key].end);
                }
            }

            // Define the objective.
            var objective = model.NewIntVar(0, horizon, "makespan");
            var ends = new List<IntVar>();
            // End times for all jobs
            for (int jobIndex = 0; jobIndex < allJobs.Count; jobIndex++)
            {
                var job = allJobs[jobIndex];
                var key = (jobIndex, job.Count - 1);
                ends.Add(allTasks[key].end);
            }
            // The objective value should not exceed the max end time.
            model.AddMaxEquality(objective, ends);
            // Minimize the max end time.
            model.Minimize(objective);

            // Create the solver.
            var solver = new CpSolver();

            // Solve
            CpSolverStatus status = solver.Solve(model);
            response.Status = status switch
            {
                CpSolverStatus.Optimal => OptimizationStatus.Optimal,
                CpSolverStatus.Feasible => OptimizationStatus.Feasible,
                CpSolverStatus.ModelInvalid => OptimizationStatus.ModelInvalid,
                CpSolverStatus.Infeasible => OptimizationStatus.Infeasible,
                CpSolverStatus.Unknown => OptimizationStatus.Unknown,
                _ => OptimizationStatus.Unknown
            };
            Debug.WriteLine($"Solve status: {status}");

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Debug.WriteLine("Solution:");
                response.ObjectiveValue = solver.ObjectiveValue;
                for (int jobIndex = 0; jobIndex < allJobs.Count; jobIndex++)
                {
                    var job = allJobs[jobIndex];
                    for (int taskIndex = 0; taskIndex < job.Count; taskIndex++)
                    {
                        var task = job[taskIndex];
                        var start = (int)solver.Value(allTasks[(jobIndex, taskIndex)].start);
                        var machine = task.machine;
                        var assignedJob = response.AssignedJobs.FirstOrDefault(a => a.Machine == machine);
                        if (assignedJob == null)
                        {
                            assignedJob = new AssignedSolutionDto { Machine = machine };
                            response.AssignedJobs.Add(assignedJob);
                        }
                        assignedJob.AssignedTasks.Add(
                            new AssignedTaskDto
                            {
                                Job = jobIndex,
                                Task = taskIndex,
                                Start = start,
                                Duration = task.duration,
                            });
                    }
                }
            }

            foreach (int machine in allMachines)
            {
                var assignedJob = response.AssignedJobs.FirstOrDefault(a => a.Machine == machine);
                if (assignedJob == null)
                    continue;

                Debug.WriteLine($"Machine {assignedJob.Machine}: ");
                assignedJob.AssignedTasks = assignedJob.AssignedTasks.OrderBy(t => t.Start).ToList();
                foreach (var assignedTask in assignedJob.AssignedTasks)
                {
                    Debug.WriteLine($"Job {assignedTask.Job} Task {assignedTask.Task} Start {assignedTask.Start} End {assignedTask.Start + assignedTask.Duration}");
                }
            }
            response.NumberOfConflicts = solver.NumConflicts();
            response.NumberOfBranches = solver.NumBranches();
            response.TimeToSolve = solver.WallTime();

            return response;
        }
    }
}
