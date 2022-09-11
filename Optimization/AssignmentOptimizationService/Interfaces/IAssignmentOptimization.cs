using AssignmentOptimizationService.Models;

namespace AssignmentOptimizationService.Interfaces
{
    public interface IAssignmentOptimization
    {
        /// <summary>
        /// Demo solving an assignment problem using the MIP (Mixed-Integer Programming) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// at most one task, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost
        /// 1       90           80           75           70
        /// 2       35           85           55           65
        /// 3       125          95           90           95
        /// 4       45           110          95           115
        /// 5       50           100          90           100
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        AssignmentResponseDto SolveAssignmentWithMip();

        /// <summary>
        /// Demo solving an assignment problem using the CP-SAT (Constraint Programming - Satisfiability) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// at most one task, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost
        /// 1       90           80           75           70
        /// 2       35           85           55           65
        /// 3       125          95           90           95
        /// 4       45           110          95           115
        /// 5       50           100          90           100
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        AssignmentResponseDto SolveAssignmentWithCpSat();

        /// <summary>
        /// Demo solving an assignment problem with teams of workers 
        /// using the CP-SAT (Constraint Programming - Satisfiability) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by multiple teams of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// at most one task, no two workers can perform the same task, and
        /// each team can perform at most two tasks.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost
        /// 1       90           76           75           70
        /// 2       35           85           55           65
        /// 3       125          95           90           105
        /// 4       45           110          95           115
        /// 5       60           105          80           75
        /// 6       45           65           110          95
        /// 
        /// Team 1: worker 1, 3, 5
        /// Team 2: worker 2, 4, 6
        /// Max number of tasks for any team: 2
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        AssignmentResponseDto SolveMultiTeamAssignmentWithCpSat();
    }
}
