using System.Runtime.Serialization;

namespace SchedulingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "SchedulingVariableResponseDto", Namespace = "urn:Optimization.Dto")]
    public class VariableResponseDto
    {
        [DataMember(Name = "day", IsRequired = true, EmitDefaultValue = true)]
        public int? Day { get; set; }

        [DataMember(Name = "employee", IsRequired = true, EmitDefaultValue = true)]
        public int? Employee { get; set; }

        [DataMember(Name = "shift", IsRequired = true, EmitDefaultValue = true)]
        public int? Shift { get; set; }

        [DataMember(Name = "requested", IsRequired = true, EmitDefaultValue = false)]
        public bool? Requested { get; set; }

        [DataMember(Name = "assigned", IsRequired = true, EmitDefaultValue = false)]
        public bool? Assigned { get; set; }
    }
}
