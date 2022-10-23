using System.Runtime.Serialization;
using SchedulingOptimizationService.Enums;

namespace SchedulingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "AssignedSchedulingResponseDto", Namespace = "urn:Optimization.Dto")]
    public class AssignedSchedulingResponseDto
    {
        [DataMember(Name = "assigned_solution", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<AssignedSolutionDto> AssignedJobs { get; set; } = new List<AssignedSolutionDto>();

        [DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
        public OptimizationStatus Status { get; set; }

        [DataMember(Name = "optimal_schedule_length", IsRequired = true, EmitDefaultValue = false)]
        public double? ObjectiveValue { get; set; }

        [DataMember(Name = "time_to_solve", IsRequired = false, EmitDefaultValue = false)]
        public double? TimeToSolve { get; set; }

        [DataMember(Name = "conflicts", IsRequired = false, EmitDefaultValue = false)]
        public long? NumberOfConflicts { get; set; }

        [DataMember(Name = "branches", IsRequired = false, EmitDefaultValue = false)]
        public long? NumberOfBranches { get; set; }
    }
}
