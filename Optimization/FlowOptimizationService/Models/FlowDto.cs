using System.Runtime.Serialization;

namespace FlowOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "FlowDto", Namespace = "urn:Optimization.Dto")]
    public class FlowDto
    {
        [DataMember(Name = "tail", IsRequired = false, EmitDefaultValue = false)]
        public double? Tail;

        [DataMember(Name = "head", IsRequired = false, EmitDefaultValue = false)]
        public double? Head;

        [DataMember(Name = "flow", IsRequired = false, EmitDefaultValue = false)]
        public double? Flow;

        [DataMember(Name = "capacity", IsRequired = false, EmitDefaultValue = false)]
        public double? Capacity;

        [DataMember(Name = "cost", IsRequired = false, EmitDefaultValue = false)]
        public double? Cost;
    }
}
