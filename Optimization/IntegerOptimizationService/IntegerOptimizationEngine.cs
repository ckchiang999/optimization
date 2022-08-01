using System.Diagnostics;
using Google.OrTools.LinearSolver;
using IntegerOptimizationService.Interfaces;
using IntegerOptimizationService.Models;

namespace IntegerOptimizationService
{
    public class IntegerOptimizationEngine : IIntegerOptimization
    {
        /// <summary>
        /// Solves a linear optimization problem that needs an integer solution
        /// </summary>
        /// <param name="problem"></param>
        /// <returns>
        /// A <see cref="LinearOptimizationService.Models.LinearProblemDto"/> with the solution
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IntegerResponseDto Solve(IntegerProblemDto problem)
        {
            // Create the linear solver with the SCIP backend
            Solver solver = Solver.CreateSolver("SCIP");

            // Create the variables with constraints
            Dictionary<string, Variable> variables = new();
            foreach (VariableDto variable in problem.Variables)
            {
                Variable numberVariable = solver.MakeIntVar(variable.Min, variable.Max, variable.Name);
                variables.Add(variable.Name, numberVariable);
            }

            // Create the linear constraints
            foreach (ConstraintDto constraint in problem.Constraints)
            {
                Constraint ct = solver.MakeConstraint(constraint.Min, constraint.Max, constraint.Name);
                foreach (CoefficientDto coefficient in constraint.Coefficients)
                {
                    Variable coefficientVariable = variables[coefficient.VariableName];
                    ct.SetCoefficient(coefficientVariable, coefficient.Value);
                }
            }

            Debug.WriteLine("Number of constraints = {0}", solver.NumConstraints());

            // Create the objective function
            Objective objective = solver.Objective();
            foreach (CoefficientDto coefficient in problem.Objective.Coefficients)
            {
                Variable coefficientVariable = variables[coefficient.VariableName];
                objective.SetCoefficient(coefficientVariable, coefficient.Value);
            }
            switch (problem.Objective.Goal)
            {
                case OptimizationGoal.Maximization:
                    objective.SetMaximization();
                    break;
                case OptimizationGoal.Minimization:
                    objective.SetMinimization();
                    break;
                default:
                    throw new InvalidOperationException("Invalid optimization goal.");
            }

            if (problem.TimeLimit.HasValue)
            {
                solver.SetTimeLimit(problem.TimeLimit.Value);
            }

            // Invoke the solver
            Solver.ResultStatus status = solver.Solve();

            Debug.WriteLine("Solution:");
            Debug.WriteLine("Objective value = {0}", solver.Objective().Value());
            foreach (KeyValuePair<string, Variable> variable in variables)
            {
                Debug.WriteLine("{0} = {1}", variable.Key, variable.Value.SolutionValue());
            }

            IntegerResponseDto result = new();
            foreach (KeyValuePair<string, Variable> variable in variables)
            {
                result.Variables.Add(new VariableResponseDto() { Name = variable.Key, Solution = variable.Value.SolutionValue() });
            }
            result.Objective.Value = solver.Objective().Value();
            result.Iterations = solver.Iterations();
            result.TimeToSolve = solver.WallTime();
            result.Status = (OptimizationStatus)status;
            return result;
        }
    }
}
