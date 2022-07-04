using System.Runtime.Serialization;

namespace LinearOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "LinearResponseDto", Namespace = "urn:Optimization.Dto")]
    public class LinearResponseDto
    {
        [DataMember(Name = "variables", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<VariableResponseDto> Variables { get; set; } = new List<VariableResponseDto>();

        [DataMember(Name = "objective", IsRequired = true, EmitDefaultValue = false)]
        public ObjectiveResponseDto Objective { get; set; } = new ObjectiveResponseDto();

        public LinearResponseDto()
        {
        }
    }
}
