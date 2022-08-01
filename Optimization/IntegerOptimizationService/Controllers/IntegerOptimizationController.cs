using IntegerOptimizationService.Interfaces;
using IntegerOptimizationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntegerOptimizationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegerOptimizationController : ControllerBase
    {
        private readonly ILogger<IntegerOptimizationController> _logger;
        private readonly IIntegerOptimization _integerOptimizationEngine;

        public IntegerOptimizationController(
            ILogger<IntegerOptimizationController> logger,
            IIntegerOptimization integerOptimizationEngine)
        {
            _logger = logger;
            _integerOptimizationEngine = integerOptimizationEngine;
        }

        /// <summary>
        /// Demo solving a Mixed Integer Problem
        /// <para>Maximize x + 10y subject to the following constraints:</para>
        /// <code>
        /// x + 7y &lt;= 17.5
        /// x &lt;= 3.5
        /// x &gt;= 0
        /// y &gt;= 0
        /// x, y integers
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="IntegerOptimizationService.Models.IntegerResponseDto"/>
        /// </returns>
        [HttpGet("SolveDemo")]
        public ActionResult<IntegerResponseDto> SolveDemo()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("IntegerOptimization Solve");
                // define the variables
                const string variable1Name = "x";
                const string variable2Name = "y";

                var problem = new IntegerProblemDto();
                var variable1 = new VariableDto
                {
                    Name = variable1Name,
                    Min = 0.0,
                    Max = double.PositiveInfinity
                };
                problem.Variables.Add(variable1);
                var variable2 = new VariableDto
                {
                    Name = variable2Name,
                    Min = 0.0,
                    Max = double.PositiveInfinity
                };
                problem.Variables.Add(variable2);

                // define the constraints
                // constraint 1: x + 7y <= 17.5
                // constraint 2: x <= 3.5
                var constraint1 = new ConstraintDto
                {
                    Name = "constraint1",
                    Min = 0,
                    Max = 17.5
                };
                constraint1.Coefficients.Add(new CoefficientDto
                {
                    Value = 1,
                    VariableName = variable1Name
                });
                constraint1.Coefficients.Add(new CoefficientDto
                {
                    Value = 7,
                    VariableName = variable2Name
                });
                problem.Constraints.Add(constraint1);

                var constraint2 = new ConstraintDto
                {
                    Name = "c2",
                    Min = 0,
                    Max = 3.5
                };
                constraint1.Coefficients.Add(new CoefficientDto
                {
                    Value = 1,
                    VariableName = variable1Name
                });
                problem.Constraints.Add(constraint2);

                // define the objective
                problem.Objective.Goal = OptimizationGoal.Maximization;
                problem.Objective.Coefficients.Add(new CoefficientDto
                {
                    Value = 1,
                    VariableName = variable1Name
                });
                problem.Objective.Coefficients.Add(new CoefficientDto
                {
                    Value = 10,
                    VariableName = variable2Name
                });

                var result = _integerOptimizationEngine.Solve(problem);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw;
            }
            finally
            {
                logScope?.Dispose();
            }
        }
    }
}
