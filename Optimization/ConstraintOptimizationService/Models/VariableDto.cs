using System.Runtime.Serialization;

namespace ConstraintOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "ConstraintVariableDto", Namespace = "urn:Optimization.Dto")]
    public class VariableDto
    {
        [DataMember(Name = "min", IsRequired = true, EmitDefaultValue = false)]
        public long Min { get; set; }

        [DataMember(Name = "max", IsRequired = true, EmitDefaultValue = false)]
        public long Max { get; set; }

        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = false)]
        public string Name { get; set; } = string.Empty;

        public VariableDto()
        {
        }
    }
}
