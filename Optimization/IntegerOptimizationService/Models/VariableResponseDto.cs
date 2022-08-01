using System.Runtime.Serialization;

namespace IntegerOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "VariableResponseDto", Namespace = "urn:Optimization.Dto")]
    public class VariableResponseDto
    {
        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = false)]
        public string? Name { get; set; }

        [DataMember(Name = "solution", IsRequired = false, EmitDefaultValue = false)]
        public double? Solution { get; set; }

        public VariableResponseDto()
        {
        }
    }
}
