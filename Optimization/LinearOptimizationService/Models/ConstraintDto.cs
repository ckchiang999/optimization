using System.Runtime.Serialization;

namespace LinearOptimizationService.Models
{
    public class ConstraintDto
    {
        [DataMember(Name = "min", IsRequired = true, EmitDefaultValue = false)]
        public double Min { get; set; }

        [DataMember(Name = "max", IsRequired = true, EmitDefaultValue = false)]
        public double Max { get; set; }

        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = false)]
        public string Name { get; set; } = string.Empty;

        public ICollection<CoefficientDto> Coefficients { get; set; } = new List<CoefficientDto>();

        public ConstraintDto()
        {
        }
    }
}
