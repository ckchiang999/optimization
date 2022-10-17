using FlowOptimizationService.Interfaces;
using FlowOptimizationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlowOptimizationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlowOptimizationController : ControllerBase
    {
        private readonly ILogger<FlowOptimizationController> _logger;
        private readonly IFlowOptimization _flowOptimizationEngine;

        public FlowOptimizationController(
            ILogger<FlowOptimizationController> logger,
            IFlowOptimization flowOptimizationEngine)
        {
            _logger = logger;
            _flowOptimizationEngine = flowOptimizationEngine;
        }

        /// <summary>
        /// Demo solving maximum flow from a source node to a sink node.
        /// <para>
        /// At each node, other than the source or the sink, the total flow of all
        /// arcs leading in to the node equals the total flow of all arcs leading out of it.
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        [HttpPost("DemoMaximumFlow")]
        public ActionResult<FlowResponseDto> SolveMaximumFlow()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("FlowOptimization SolveMaximumFlow()");
                FlowResponseDto result = _flowOptimizationEngine.SolveMaximumFlow();
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

        /// <summary>
        /// Demo solving minimum cost flow from a source node to a sink node.
        /// Each line connecting one node to the next has a cost associated wiht it.
        /// Solve by finding the flow with the least total cost.
        /// <para>
        /// <list type="bullet">
        /// <item>
        /// <description>At the supply node, a positive amount is added to the flow.</description>    
        /// </item>
        /// <item>
        /// <description>At the demand node, a negative amount is taken away from the flow.</description>
        /// </item>
        /// <item>
        /// <description>Supply is indicated by a positive value and demand by a negative value.</description>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        [HttpPost("DemoMinimumCostFlow")]
        public ActionResult<FlowResponseDto> SolveMinimumCostFlow()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("FlowOptimization SolveMinimumCostFlow()");
                FlowResponseDto result = _flowOptimizationEngine.SolveMinimumCostFlow();
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

        /// <summary>
        /// Demo solving an assignment problem using the minimum flow solver.
        /// <para>
        /// Flow is 1 and capacity is 1.  Flow is from worker to task.
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        [HttpPost("DemoAssignmentAsFlow")]
        public ActionResult<FlowResponseDto> SolveAssignmentAsFlow()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("FlowOptimization SolveAssignmentAsFlow()");
                FlowResponseDto result = _flowOptimizationEngine.SolveAssignmentAsFlow();
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

        /// <summary>
        /// Demo solving a team assignment problem using the minimum flow solver.
        /// <para>
        /// Flow is 1 and capacity is 1.  Flow is from worker to task.
        /// There are six workers divided into two teams, and
        /// there are four tasks.  Each team will perform 2 tasks.
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        [HttpPost("DemoTeamAssignmentAsFlow")]
        public ActionResult<FlowResponseDto> SolveTeamAssignmentAsFlow()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("FlowOptimization SolveTeamAssignmentAsFlow()");
                FlowResponseDto result = _flowOptimizationEngine.SolveTeamAssignmentAsFlow();
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