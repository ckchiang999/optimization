using System.Runtime.Serialization;
using SchedulingOptimizationService.Enums;

namespace SchedulingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "SchedulingResponseDto", Namespace = "urn:Optimization.Dto")]
    public class SchedulingResponseDto
    {
        [DataMember(Name = "solutions", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<SolutionDto> Solutions { get; set; } = new List<SolutionDto>();

        [DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
        public OptimizationStatus Status { get; set; }

        [DataMember(Name = "solution_count", IsRequired = true, EmitDefaultValue = false)]
        public int SolutionCount { get { return Solutions.Count; } }

        [DataMember(Name = "objective_met_count", IsRequired = true, EmitDefaultValue = false)]
        public double? ObjectiveValue { get; set; }

        [DataMember(Name = "time_to_solve", IsRequired = false, EmitDefaultValue = false)]
        public double? TimeToSolve { get; set; }

        [DataMember(Name = "conflicts", IsRequired = false, EmitDefaultValue = false)]
        public long? NumberOfConflicts { get; set; }

        [DataMember(Name = "branches", IsRequired = false, EmitDefaultValue = false)]
        public long? NumberOfBranches { get; set; }
    }
}
