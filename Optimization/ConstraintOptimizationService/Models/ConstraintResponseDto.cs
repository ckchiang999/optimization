using System.Runtime.Serialization;

namespace ConstraintOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "ConstraintResponseDto", Namespace = "urn:Optimization.Dto")]
    public class ConstraintResponseDto
    {
        [DataMember(Name = "variables", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<SolutionDto> Solutions { get; set; } = new List<SolutionDto>();

        [DataMember(Name = "status", IsRequired = true, EmitDefaultValue = false)]
        public OptimizationStatus Status { get; set; }

        [DataMember(Name = "solution_count", IsRequired = true, EmitDefaultValue = false)]
        public int SolutionCount { get { return Solutions.Count; } }

        [DataMember(Name = "time_to_solve", IsRequired = false, EmitDefaultValue = false)]
        public double? TimeToSolve { get; set; }

        [DataMember(Name = "conflicts", IsRequired = false, EmitDefaultValue = false)]
        public long? NumberOfConflicts { get; set; }

        [DataMember(Name = "branches", IsRequired = false, EmitDefaultValue = false)]
        public long? NumberOfBranches { get; set; }

        public ConstraintResponseDto()
        {
        }
    }
}
