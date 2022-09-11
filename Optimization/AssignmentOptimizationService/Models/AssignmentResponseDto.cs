using System.Runtime.Serialization;

namespace AssignmentOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "AssignmentResponseDto", Namespace = "urn:Optimization.Dto")]
    public class AssignmentResponseDto
    {
        [DataMember(Name = "variables", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<VariableResponseDto> Variables { get; set; } = new List<VariableResponseDto>();

        [DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
        public OptimizationStatus Status { get; set; }
    }
}
