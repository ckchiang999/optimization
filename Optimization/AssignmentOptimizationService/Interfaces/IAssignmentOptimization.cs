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

        /// <summary>
        /// Demo solving an assignment problem with teams of workers 
        /// using the MIP (Mixed Integer Programming) solver.
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
        AssignmentResponseDto SolveMultiTeamAssignmentWithMip();

        /// <summary>
        /// Demo solving an task size assignment problem using the CP-SAT (Constrained Programming - Satisfiability) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// zero or more tasks with a total maximum task size, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost, Task 5 Cost, Task 6 Cost, Task 7 Cost, Task 8 Cost
        /// 1       90           76           75           70           50           74           12           68
        /// 2       35           85           55           65           48           101          70           83
        /// 3       125          95           90           105          59           120          36           73
        /// 4       45           110          95           115          104          83           37           71
        /// 5       60           105          80           75           59           62           93           88
        /// 6       45           65           110          95           47           31           81           34
        /// 7       38           51           107          41           69           99           115          48
        /// 8       47           85           57           71           92           77           109          36
        /// 9       39           63           97           49           118          56           92           61
        /// 10      47           101          71           60           88           109          52           90
        /// 
        ///         Task 1 Size, Task 2 Size, Task 3 Size, Task 4 Size, Task 5 Size, Task 6 Size, Task 7 Size, Task 8 Size
        ///         10           7            3            12           15           4            11           5
        ///
        /// Maximu task size: 15
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        AssignmentResponseDto SolveTaskSizeAssignmentWithCpSat();

        /// <summary>
        /// Demo solving an task size assignment problem using the MIP (Mixed Integer Programming) solver.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// zero or more tasks with a total maximum task size, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost, Task 5 Cost, Task 6 Cost, Task 7 Cost, Task 8 Cost
        /// 1       90           76           75           70           50           74           12           68
        /// 2       35           85           55           65           48           101          70           83
        /// 3       125          95           90           105          59           120          36           73
        /// 4       45           110          95           115          104          83           37           71
        /// 5       60           105          80           75           59           62           93           88
        /// 6       45           65           110          95           47           31           81           34
        /// 7       38           51           107          41           69           99           115          48
        /// 8       47           85           57           71           92           77           109          36
        /// 9       39           63           97           49           118          56           92           61
        /// 10      47           101          71           60           88           109          52           90
        /// 
        ///         Task 1 Size, Task 2 Size, Task 3 Size, Task 4 Size, Task 5 Size, Task 6 Size, Task 7 Size, Task 8 Size
        ///         10           7            3            12           15           4            11           5
        ///
        /// Maximu task size: 15
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        AssignmentResponseDto SolveTaskSizeAssignmentWithMip();

        /// <summary>
        /// Demo solving an allowed groups assignment problem using the CP-SAT (Constraint Programming - Satisfiability) solver.
        /// <para>
        /// This is a problem in which only certain groups of workers can be assigned to the tasks.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost, Task 5 Cost, Task 6 Cost
        /// 1       90           76           75           70           50           74
        /// 2       35           85           55           65           48           101
        /// 3       125          95           90           105          59           120
        /// 4       45           110          95           115          104          83
        /// 5       60           105          80           75           59           62
        /// 6       45           65           110          95           47           31
        /// 7       38           51           107          41           69           99
        /// 8       47           85           57           71           92           77
        /// 9       39           63           97           49           118          56
        /// 10      47           101          71           60           88           109
        /// 11      17           39           103          64           61           92
        /// 12      101          45           83           59           92           27
        /// 
        /// group 1 (subgroups of workers 0 - 3): [2, 3], [1, 3], [1, 2], [0, 1], [0, 2]
        /// group 2 (subgroups of workers 4 - 7): [6, 7], [5, 7], [5, 6], [4, 5], [4, 7]
        /// group 3 (subgroups of workers 8 -11): [10, 11], [9, 11], [9, 10], [8, 10], [8, 11]
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        AssignmentResponseDto SolveAllowedGroupsAssignmentWithCpSat();

        /// <summary>
        /// Demo solving an allowed groups assignment problem using the MIP (Mixed Integer Programming) solver.
        /// <para>
        /// This is a problem in which only certain groups of workers can be assigned to the tasks.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost, Task 5 Cost, Task 6 Cost
        /// 1       90           76           75           70           50           74
        /// 2       35           85           55           65           48           101
        /// 3       125          95           90           105          59           120
        /// 4       45           110          95           115          104          83
        /// 5       60           105          80           75           59           62
        /// 6       45           65           110          95           47           31
        /// 7       38           51           107          41           69           99
        /// 8       47           85           57           71           92           77
        /// 9       39           63           97           49           118          56
        /// 10      47           101          71           60           88           109
        /// 11      17           39           103          64           61           92
        /// 12      101          45           83           59           92           27
        /// 
        /// group 1 (subgroups of workers 0 - 3): [2, 3], [1, 3], [1, 2], [0, 1], [0, 2]
        /// group 2 (subgroups of workers 4 - 7): [6, 7], [5, 7], [5, 6], [4, 5], [4, 7]
        /// group 3 (subgroups of workers 8 -11): [10, 11], [9, 11], [9, 10], [8, 10], [8, 11]
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        AssignmentResponseDto SolveAllowedGroupsAssignmentWithMip();

        /// <summary>
        /// Demo solving an assignment problem using the Linear Sum Assignment solver.
        /// This is a specialized solver for assignment problems and is faster than the MIP and CP-SAT
        /// solvers for the assignment problem.
        /// <para>
        /// This is a problem to minimize the cost of performing a set of tasks by a group of workers,
        /// given there is a cost for assigning a worker to a task, each worker can be assigned to 
        /// at most one task, and no two workers can perform the same task.
        /// </para>
        /// <code>
        /// Worker, Task 1 Cost, Task 2 Cost, Task 3 Cost, Task 4 Cost
        /// 1       90           76           75           70
        /// 2       35           85           55           65
        /// 3       125          95           90           105
        /// 4       45           110          95           115
        /// </code>
        /// </summary>
        /// <returns>
        /// An <see cref="AssignmentOptimizationService.Models.AssignmentResponseDto"/>
        /// </returns>
        AssignmentResponseDto SolveAssignmentWithLinearSumAssignmentSolver();
    }
}
