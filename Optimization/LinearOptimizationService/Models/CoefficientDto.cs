using System.Runtime.Serialization;

namespace LinearOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "CoefficientDto", Namespace = "urn:Optimization.Dto")]
    public class CoefficientDto
    {
        [DataMember(Name = "variable_name", IsRequired = true, EmitDefaultValue = false)]
        public string VariableName { get; set; } = string.Empty;

        [DataMember(Name = "value", IsRequired = true, EmitDefaultValue = false)]
        public double Value { get; set; }

        public CoefficientDto()
        {
        }
    }
}
