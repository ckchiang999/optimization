using LinearOptimizationService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinearOptimizationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinearOptimizationController : ControllerBase
    {
        private readonly ILogger<LinearOptimizationController> _logger;

        public LinearOptimizationController(ILogger<LinearOptimizationController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// An example solving a simple Linear Programming problem.
        /// <para>Maximize 3x + y subject to the following constraints:</para>
        /// <code>
        /// 0 &lt;= x &lt;= 1
        /// 0 &lt;= y &lt;= 2
        /// x + y &lt;= 2
        /// </code>
        /// </summary>
        /// <returns>
        /// A <see cref="LinearOptimizationService.Models.LinearResponseDto"/> with the solution
        /// </returns>
        [HttpPost("demo")]
        public ActionResult<LinearResponseDto> SolveDemo()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("LinearOptimization Solve");
                ILinearOptimization optimizer = new LinearOptimizationEngine();
                LinearResponseDto result = optimizer.SolveDemo();

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

        [HttpPost("solve")]
        public ActionResult<LinearResponseDto> Solve(LinearProblemDto problem)
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("LinearOptimization Solve");
                ILinearOptimization optimizer = new LinearOptimizationEngine();
                LinearResponseDto result = optimizer.Solve(problem);

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

        [HttpPost("stiglerdiet")]
        public ActionResult<LinearResponseDto> SolveStiglerDietProblemDemo()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("LinearOptimization SolveStiglerDietProblemDemo");
                ILinearOptimization optimizer = new LinearOptimizationEngine();
                LinearResponseDto result = optimizer.SolveStiglerDietProblemDemo();

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
