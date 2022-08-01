using System.Runtime.Serialization;

namespace LinearOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "LinearProblemDto", Namespace = "urn:Optimization.Dto")]
    public class LinearProblemDto
    {
        [DataMember(Name = "variables", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<VariableDto> Variables { get; set; }

        [DataMember(Name = "constraints", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<ConstraintDto> Constraints { get; set; }

        [DataMember(Name = "objective", IsRequired = true, EmitDefaultValue = false)]
        public ObjectiveDto Objective { get; set; }

        [DataMember(Name = "time_limit", IsRequired = false, EmitDefaultValue = false)]
        public long? TimeLimit { get; set; }

        public LinearProblemDto()
        {
            Variables = new List<VariableDto>();
            Constraints = new List<ConstraintDto>();
            Objective = new ObjectiveDto();
        }
    }
}
