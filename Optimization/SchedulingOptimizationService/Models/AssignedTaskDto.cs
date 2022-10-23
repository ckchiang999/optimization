using System.Runtime.Serialization;

namespace SchedulingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "SchedulingAssignedTaskResponseDto", Namespace = "urn:Optimization.Dto")]
    public class AssignedTaskDto : IComparable
    {
        [DataMember(Name = "job", IsRequired = true, EmitDefaultValue = true)]
        public int Job { get; set; }

        [DataMember(Name = "task", IsRequired = true, EmitDefaultValue = true)]
        public int Task { get; set; }

        [DataMember(Name = "start", IsRequired = true, EmitDefaultValue = true)]
        public int Start { get; set; }

        [DataMember(Name = "duration", IsRequired = true, EmitDefaultValue = true)]
        public int Duration { get; set; }

        public int CompareTo(object? another)
        {
            if (another == null)
                return 1;

            if (another is not AssignedTaskDto otherTask)
                throw new ArgumentException("Object is not an AssignedTask");

            if (this.Start != otherTask.Start)
                return this.Start.CompareTo(otherTask.Start);
            else
                return this.Duration.CompareTo(otherTask.Duration);
        }
    }
}
