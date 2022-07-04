using System.Diagnostics;
using Google.OrTools.LinearSolver;
using LinearOptimizationService.Models;

namespace LinearOptimizationService
{
    public class LinearOptimizationEngine : ILinearOptimization
    {
        public LinearOptimizationEngine()
        {
        }

        /// <summary>
        /// An example solving a simple linear function.
        /// Objective function: 3x + y
        /// Maximize 3x + y subject to the following constraints:
        ///   0 <= x <= 1
        ///   0 <= y <= 2
        ///   x + y <= 2
        /// </summary>
        /// <returns></returns>
        public LinearResponseDto SolveDemo()
        {
            // Create the linear solver with the GLOP backend
            Solver solver = Solver.CreateSolver("GLOP");

            // Create the variables x and y with constraints
            Variable x = solver.MakeNumVar(0.0, 1.0, "x");
            Variable y = solver.MakeNumVar(0.0, 2.0, "y");

            Debug.WriteLine("Number of variables = {0}", solver.NumVariables());

            // Create a linear constraint, 0 <= x + y <= 2
            Constraint ct = solver.MakeConstraint(0.0, 2.0, "ct");
            ct.SetCoefficient(x, 1);
            ct.SetCoefficient(y, 1);

            Debug.WriteLine("Number of constraints = {0}", solver.NumConstraints());

            // Create the objective function, 3 * x + y
            Objective objective = solver.Objective();
            objective.SetCoefficient(x, 3);
            objective.SetCoefficient(y, 1);
            objective.SetMaximization();

            // Invoke the solver
            solver.Solve();

            Debug.WriteLine("Solution:");
            Debug.WriteLine("Objective value = {0}", solver.Objective().Value());
            Debug.WriteLine("x = {0}", x.SolutionValue());
            Debug.WriteLine("y = {0}", y.SolutionValue());

            LinearResponseDto result = new();
            result.Variables.Add(new VariableResponseDto() { Name = "x", Solution = x.SolutionValue() });
            result.Variables.Add(new VariableResponseDto() { Name = "y", Solution = y.SolutionValue() });
            result.Objective.Value = solver.Objective().Value();

            return result;
        }

        /// <summary>
        /// Solves a linear optimization problem
        /// </summary>
        /// <param name="problem"></param>
        /// <returns>
        /// A <see cref="LinearOptimizationService.Models.LinearProblemDto"/> with the solution
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public LinearResponseDto Solve(LinearProblemDto problem)
        {
            // Create the linear solver with the GLOP backend
            Solver solver = Solver.CreateSolver("GLOP");

            // Create the variables x and y with constraints
            Dictionary<string, Variable> variables = new();
            foreach (VariableDto variable in problem.Variables)
            {
                Variable numberVariable = solver.MakeNumVar(variable.Min, variable.Max, variable.Name);
                variables.Add(variable.Name, numberVariable);
            }

            Debug.WriteLine("Number of variables = {0}", solver.NumVariables());

            // Create a linear constraints
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

            // Invoke the solver
            solver.Solve();

            Debug.WriteLine("Solution:");
            Debug.WriteLine("Objective value = {0}", solver.Objective().Value());
            foreach (KeyValuePair<string, Variable> variable in variables)
            {
                Debug.WriteLine("{0} = {1}", variable.Key, variable.Value.SolutionValue());
            }

            LinearResponseDto result = new();
            foreach (KeyValuePair<string, Variable> variable in variables)
            {
                result.Variables.Add(new VariableResponseDto() { Name = variable.Key, Solution = variable.Value.SolutionValue() });
            }
            result.Objective.Value = solver.Objective().Value();
            return result;
        }
    }
}
