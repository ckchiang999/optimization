using FlowOptimizationService.Models;

namespace FlowOptimizationService.Interfaces
{
    public interface IFlowOptimization
    {
        /// <summary>
        /// Demo solving maximum flow from a source node to a sink node.
        /// <para>
        /// At each node, other than the source or the sink, the total flow of all
        /// lines leading in to the node equals the total flow of all lines leading out of it.
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        FlowResponseDto SolveMaximumFlow();

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
        FlowResponseDto SolveMinimumCostFlow();

        /// <summary>
        /// Demo solving an assignment problem using the minimum flow solver.
        /// <para>
        /// Flow is 1 and capacity is 1.  Flow is from worker to task.
        /// </para>
        /// </summary>
        /// <returns>
        /// <see cref="FlowResponseDto"/>
        /// </returns>
        FlowResponseDto SolveAssignmentAsFlow();

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
        FlowResponseDto SolveTeamAssignmentAsFlow();
    }
}
