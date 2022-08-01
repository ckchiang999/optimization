using System.Runtime.Serialization;

namespace IntegerOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "ObjectiveResponseDto", Namespace = "urn:Optimization.Dto")]
    public class ObjectiveResponseDto
    {
        [DataMember(Name = "value", IsRequired = true, EmitDefaultValue = false)]
        public double? Value { get; set; }

        public ObjectiveResponseDto()
        {
        }
    }
}
