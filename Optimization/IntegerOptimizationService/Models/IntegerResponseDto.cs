using System.Runtime.Serialization;

namespace IntegerOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "LinearResponseDto", Namespace = "urn:Optimization.Dto")]
    public class IntegerResponseDto
    {
        [DataMember(Name = "variables", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<VariableResponseDto> Variables { get; set; } = new List<VariableResponseDto>();

        [DataMember(Name = "objective", IsRequired = true, EmitDefaultValue = false)]
        public ObjectiveResponseDto Objective { get; set; } = new ObjectiveResponseDto();

        [DataMember(Name = "iterations", IsRequired = true, EmitDefaultValue = false)]
        public long? Iterations { get; set; }

        [DataMember(Name = "time_to_solve", IsRequired = true, EmitDefaultValue = false)]
        public long? TimeToSolve { get; set; }

        [DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
        public OptimizationStatus Status { get; set; }

        public IntegerResponseDto()
        {
        }
    }
}
