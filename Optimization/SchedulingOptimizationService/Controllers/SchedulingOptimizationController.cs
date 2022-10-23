using Microsoft.AspNetCore.Mvc;
using SchedulingOptimizationService.Interfaces;
using SchedulingOptimizationService.Models;

namespace SchedulingOptimizationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchedulingOptimizationController : ControllerBase
    {
        private readonly ILogger<SchedulingOptimizationController> _logger;
        private readonly ISchedulingOptimization _schedulingOptimizationEngine;

        public SchedulingOptimizationController(
            ILogger<SchedulingOptimizationController> logger,
            ISchedulingOptimization schedulingOptimizationEngine)
        {
            _logger = logger;
            _schedulingOptimizationEngine = schedulingOptimizationEngine;
        }

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
        [HttpPost("DemoEmployeeScheduling")]
        public ActionResult<SchedulingResponseDto> SolveEmployeeScheduling()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("SchedulingOptimizationEngine SolveEmployeeScheduling()");
                SchedulingResponseDto result = _schedulingOptimizationEngine.SolveEmployeeScheduling();
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
        [HttpPost("DemoEmployeeSchedulingWithShiftRequests")]
        public ActionResult<SchedulingResponseDto> SolveEmployeeSchedulingWithShiftRequests()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("SchedulingOptimizationEngine SolveEmployeeSchedulingWithShiftRequests()");
                SchedulingResponseDto result = _schedulingOptimizationEngine.SolveEmployeeSchedulingWithShiftRequests();
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
        [HttpPost("DemoJobShopProblem")]
        public ActionResult<AssignedSchedulingResponseDto> SolveJobShopProblem()
        {
            IDisposable? logScope = null;
            try
            {
                logScope = _logger.BeginScope("SchedulingOptimizationEngine SolveJobShopProblem()");
                AssignedSchedulingResponseDto result = _schedulingOptimizationEngine.SolveJobShopProblem();
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