using System.Runtime.Serialization;

namespace ConstraintOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "ContraintVariableResponseDto", Namespace = "urn:Optimization.Dto")]
    public class VariableResponseDto
    {
        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = false)]
        public string? Name { get; set; }

        [DataMember(Name = "solution", IsRequired = false, EmitDefaultValue = false)]
        public long? Solution { get; set; }

        public VariableResponseDto()
        {
        }
    }
}
