using System.Runtime.Serialization;

namespace IntegerOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "VariableDto", Namespace = "urn:Optimization.Dto")]
    public class VariableDto
    {
        [DataMember(Name = "min", IsRequired = true, EmitDefaultValue = false)]
        public double Min { get; set; }

        [DataMember(Name = "max", IsRequired = true, EmitDefaultValue = false)]
        public double Max { get; set; }

        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = false)]
        public string Name { get; set; } = string.Empty;

        public VariableDto()
        {
        }
    }
}
