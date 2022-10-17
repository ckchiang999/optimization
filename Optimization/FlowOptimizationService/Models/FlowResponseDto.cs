using System.Runtime.Serialization;
using FlowOptimizationService.Enums;

namespace FlowOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "FlowResponseDto", Namespace = "urn:Optimization.Dto")]
    public class FlowResponseDto
    {
        [DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
        public OptimizationStatus Status { get; set; }

        [DataMember(Name = "optimal_value", IsRequired = false, EmitDefaultValue = false)]
        public double? OptimalValue;

        [DataMember(Name = "items", IsRequired = false, EmitDefaultValue = false)]
        public ICollection<FlowDto> Flows { get; set; } = new List<FlowDto>();
    }
}
