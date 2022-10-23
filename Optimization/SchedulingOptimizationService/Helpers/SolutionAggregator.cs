using System.Diagnostics;
using Google.OrTools.Sat;
using SchedulingOptimizationService.Models;

namespace SchedulingOptimizationService.Helpers
{
    public class SolutionAggregator : CpSolverSolutionCallback
    {
        private readonly SchedulingResponseDto _responseDto;
        private readonly int[] _employees;
        private readonly int[] _days;
        private readonly int[] _shifts;
        private readonly Dictionary<(int employee, int day, int shift), BoolVar> _variables;
        private readonly int? _solutionLimit;
        private int _solutionCount;

        public SolutionAggregator(
            SchedulingResponseDto responseDto,
            int[] employees,
            int[] days,
            int[] shifts,
            Dictionary<(int employee, int day, int shift), BoolVar> variables, 
            int? solutionLimit = null)
        {
            _responseDto = responseDto;
            _employees = employees;
            _days = days;
            _shifts = shifts;
            _variables = variables;
            _solutionLimit = solutionLimit;
            _solutionCount = 0;
        }

        public override void OnSolutionCallback()
        {
            var solution = new SolutionDto();
            foreach (int day in _days)
            {
                Debug.WriteLine($"Day {day}");
                foreach (int employee in _employees)
                {
                    bool isWorking = false;
                    foreach (int shift in _shifts)
                    {
                        if (Value(_variables[(employee, day, shift)]) == 1)
                        {
                            isWorking = true;
                            Debug.WriteLine($"Employee {employee} work shift {shift}");
                            solution.Variables.Add(new VariableResponseDto
                            {
                                Day = day,
                                Employee = employee,
                                Shift = shift,
                                Assigned = true
                            });
                        }
                    }
                    if (!isWorking)
                    {
                        Debug.WriteLine($"Employee {employee} does not work");
                    }
                }
            }
            solution.TimeToSolve = WallTime();
            _responseDto.Solutions.Add(solution);
            _solutionCount++;
            if (_solutionCount >= _solutionLimit)
            {
                Debug.WriteLine($"Stop search after {_solutionLimit} solutions");
                StopSearch();
            }
        }
    }
}
