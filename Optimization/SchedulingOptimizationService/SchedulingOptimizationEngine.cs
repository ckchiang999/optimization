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
        /// We need to create a schedule for four nurses over a three day period.
        /// The constraints are:
        /// </para>
        /// <list type="bullet">
        ///     <item>Each day is divided into three 8 hour shifts.</item>
        ///     <item>Every day, each shift is assigned to a single nurse, and no nurse works more than one shift.</item>
        ///     <item>Each nurse is assigned to at least two shifts during the three day period.</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public SchedulingResponseDto SolveEmployeeScheduling()
        {
            var response = new SchedulingResponseDto();

            const int numNurses = 4;
            const int numDays = 3;
            const int numShifts = 3;

            int[] allNurses = Enumerable.Range(0, numNurses).ToArray();
            int[] allDays = Enumerable.Range(0, numDays).ToArray();
            int[] allShifts = Enumerable.Range(0, numShifts).ToArray();

            // Create the model
            var model = new CpModel();
            model.Model.Variables.Capacity = numNurses * numDays * numShifts;

            // Create variables for each shift.
            // shifts[(nurse, day, shift)] = 1 if 'nurse' works 'shift' on 'day', 0 otherwise.
            var shifts = new Dictionary<(int, int, int), BoolVar>(numNurses * numDays * numShifts);
            foreach (int nurse in allNurses)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        shifts.Add((nurse, day, shift), model.NewBoolVar($"shifts_nurse{nurse}day{day}shift{shift}"));
                    }
                }
            }

            // Each shift is assigned to exaclty one nurse in the schedule period.
            var literals = new List<ILiteral>();
            foreach (int day in allDays)
            {
                foreach (int shift in allShifts)
                {
                    foreach (int nurse in allNurses)
                    {
                        literals.Add(shifts[(nurse, day, shift)]);
                    }
                    model.AddExactlyOne(literals);
                    literals.Clear();
                }
            }

            // Each nurse works at most one shift per day.
            foreach (int nurse in allNurses)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        literals.Add(shifts[(nurse, day, shift)]);
                    }
                    model.AddAtMostOne(literals);
                    literals.Clear();
                }
            }

            // Distribute shifts evenly.  Some nurses may be assigned one more shift than others.
            int minShiftsPerNurse = (numShifts * numDays) / numNurses;
            int maxShiftsPerNurse = (numShifts * numDays) % numNurses == 0 ? minShiftsPerNurse : minShiftsPerNurse + 1;

            var numShiftsWorked = new List<IntVar>();
            foreach (int nurse in allNurses)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        numShiftsWorked.Add(shifts[(nurse, day, shift)]);
                    }
                }
                // No nurse is assigned more than one extra shift
                model.AddLinearConstraint(LinearExpr.Sum(numShiftsWorked), minShiftsPerNurse, maxShiftsPerNurse);
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
                allNurses,
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
        /// We need to create a schedule for five nurses over a seven day period.
        /// The constraints are:
        /// </para>
        /// <list type="bullet">
        ///     <item>Each day is divided into three 8 hour shifts.</item>
        ///     <item>Every day, each shift is assigned to a single nurse, and no nurse works more than one shift.</item>
        ///     <item>Each nurse is assigned to at least two shifts during the three day period.</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public SchedulingResponseDto SolveEmployeeSchedulingWithShiftRequests()
        {
            var response = new SchedulingResponseDto();

            const int numNurses = 5;
            const int numDays = 7;
            const int numShifts = 3;

            int[] allNurses = Enumerable.Range(0, numNurses).ToArray();
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
            model.Model.Variables.Capacity = numNurses * numDays * numShifts;

            // Create variables for each shift.
            // shifts[(nurse, day, shift)] = 1 if 'nurse' works 'shift' on 'day', 0 otherwise.
            var shifts = new Dictionary<(int, int, int), BoolVar>(numNurses * numDays * numShifts);
            foreach (int nurse in allNurses)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        shifts.Add((nurse, day, shift), model.NewBoolVar($"shifts_nurse{nurse}day{day}shift{shift}"));
                    }
                }
            }

            // Each shift is assigned to exaclty one nurse in the schedule period.
            var literals = new List<ILiteral>();
            foreach (int day in allDays)
            {
                foreach (int shift in allShifts)
                {
                    var nurseAssignment = new IntVar[numNurses];
                    foreach (int nurse in allNurses)
                    {
                        nurseAssignment[nurse] = shifts[(nurse, day, shift)];
                    }
                    model.Add(LinearExpr.Sum(nurseAssignment) == 1);
                }
            }

            // Each nurse works at most one shift per day.
            foreach (int nurse in allNurses)
            {
                foreach (int day in allDays)
                {
                    var shiftAssignment = new IntVar[numShifts];
                    foreach (int shift in allShifts)
                    {
                        shiftAssignment[shift] = shifts[(nurse, day, shift)];
                    }
                    model.Add(LinearExpr.Sum(shiftAssignment) <= 1);
                }
            }

            // Distribute shifts evenly.  Some nurses may be assigned one more shift than others.
            int minShiftsPerNurse = (numShifts * numDays) / numNurses;
            int maxShiftsPerNurse = (numShifts * numDays) % numNurses == 0 ? minShiftsPerNurse : minShiftsPerNurse + 1;

            foreach (int nurse in allNurses)
            {
                var numShiftsWorked = new IntVar[numDays * numShifts];
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        numShiftsWorked[day * numShifts + shift] = shifts[(nurse, day, shift)];
                    }
                }
                // No nurse is assigned more than one extra shift
                model.AddLinearConstraint(LinearExpr.Sum(numShiftsWorked), minShiftsPerNurse, maxShiftsPerNurse);
            }

            var flatShifts = new IntVar[numNurses * numDays * numShifts];
            var flatShiftRequests = new int[numNurses * numDays * numShifts];
            foreach (int nurse in allNurses)
            {
                foreach (int day in allDays)
                {
                    foreach (int shift in allShifts)
                    {
                        flatShifts[nurse * numDays * numShifts + day * numShifts + shift] = shifts[(nurse, day, shift)];
                        flatShiftRequests[nurse * numDays * numShifts + day * numShifts + shift] = shiftRequests[nurse, day, shift];
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
                    foreach (int nurse in allNurses)
                    {
                        foreach (int shift in allShifts)
                        {
                            if (solver.Value(shifts[(nurse, day, shift)]) == 1)
                            {
                                if (shiftRequests[nurse, day, shift] == 1)
                                {
                                    Debug.WriteLine($"Nurse {nurse} work shift {shift} (requested).");
                                    solution.Variables.Add(new VariableResponseDto
                                    {
                                        Day = day,
                                        Employee = nurse,
                                        Shift = shift,
                                        Requested = true
                                    });
                                }
                                else
                                {
                                    Debug.WriteLine($"Nurse {nurse} work shift {shift} (not requested).");
                                    solution.Variables.Add(new VariableResponseDto
                                    {
                                        Day = day,
                                        Employee = nurse,
                                        Shift = shift,
                                        Requested = false
                                    });
                                }
                                solution.TimeToSolve = solver.WallTime();
                            }
                        }
                    }
                }
                Debug.WriteLine($"Number of shift requests met = {solver.ObjectiveValue} (out of {numNurses * minShiftsPerNurse}.");
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
    }
}
