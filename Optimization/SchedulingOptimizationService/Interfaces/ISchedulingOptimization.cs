using SchedulingOptimizationService.Models;

namespace SchedulingOptimizationService.Interfaces
{
    public interface ISchedulingOptimization
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
        SchedulingResponseDto SolveEmployeeScheduling();

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
        SchedulingResponseDto SolveEmployeeSchedulingWithShiftRequests();
    }
}
