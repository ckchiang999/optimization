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

        [DataMember(Name = "time_to_solve", IsRequired = false, EmitDefaultValue = false)]
        public double? TimeToSolve { get; set; }

        [DataMember(Name = "conflicts", IsRequired = false, EmitDefaultValue = false)]
        public long? NumberOfConflicts { get; set; }

        [DataMember(Name = "branches", IsRequired = false, EmitDefaultValue = false)]
        public long? NumberOfBranches { get; set; }
    }
}
