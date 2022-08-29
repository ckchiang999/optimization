using System.Runtime.Serialization;

namespace ConstraintOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "SolutionDto", Namespace = "urn:Optimization.Dto")]
    public class SolutionDto
    {
        [DataMember(Name = "variables", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<VariableResponseDto> Variables { get; set;  } = new List<VariableResponseDto>();

        [DataMember(Name = "time_to_solve", IsRequired = true, EmitDefaultValue = false)]
        public double? TimeToSolve { get; set; }
    }
}
