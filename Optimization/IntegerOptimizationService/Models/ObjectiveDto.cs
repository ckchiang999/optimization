using System.Runtime.Serialization;

namespace IntegerOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "VariableDto", Namespace = "urn:Optimization.Dto")]
    public class ObjectiveDto
    {
        [DataMember(Name = "coefficients", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<CoefficientDto> Coefficients { get; set; } = new List<CoefficientDto>();

        [DataMember(Name = "goal", IsRequired = true, EmitDefaultValue = false)]
        public OptimizationGoal Goal { get; set; } = OptimizationGoal.Maximization;
 
        public ObjectiveDto()
        {
        }
    }
}
