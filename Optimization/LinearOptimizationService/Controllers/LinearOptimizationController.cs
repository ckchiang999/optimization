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

        [HttpGet(Name = "SolveDemo")]
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

        [HttpPost(Name = "Solve")]
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
    }
}
