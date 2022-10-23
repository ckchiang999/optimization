using System.Runtime.Serialization;

namespace SchedulingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "AssignedSolutionDto", Namespace = "urn:Optimization.Dto")]
    public class AssignedSolutionDto
    {
        [DataMember(Name = "machine", IsRequired = true, EmitDefaultValue = true)]
        public int Machine { get; set; }

        [DataMember(Name = "assigned_tasks", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<AssignedTaskDto> AssignedTasks { get; set; } = new List<AssignedTaskDto>();
    }
}
