using System.Diagnostics;
using ConstraintOptimizationService.Models;
using Google.OrTools.Sat;

namespace ConstraintOptimizationService.Helpers
{
    public class SolutionAggregator : CpSolverSolutionCallback
    {
        private readonly ConstraintResponseDto _responseDto;
        private readonly IntVar[] _variables;
        private readonly int? _solutionLimit;

        public SolutionAggregator(ConstraintResponseDto responseDto,  IntVar[] variables, int? solutionLimit = null)
        {
            _responseDto = responseDto;
            _variables = variables;
            _solutionLimit = solutionLimit;
        }

        public override void OnSolutionCallback()
        {
            SolutionDto solutionDto = new();
            solutionDto.TimeToSolve = WallTime();
            foreach (IntVar variable in _variables)
            {
                VariableResponseDto variableResponseDto = new()
                {
                    Name = variable.Name(),
                    Solution = Value(variable)
                };
                solutionDto.Variables.Add(variableResponseDto);
            }
            _responseDto.Solutions.Add(solutionDto);
            Debug.WriteLine($"Solution #{_responseDto.Solutions.Count()}: time = {solutionDto.TimeToSolve:F2}");
            if (_solutionLimit.HasValue && _responseDto.SolutionCount >= _solutionLimit)
            {
                StopSearch();
            }
        }
    }
}
