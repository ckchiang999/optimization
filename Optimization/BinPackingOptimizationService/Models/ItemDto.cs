using System.Runtime.Serialization;

namespace BinPackingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "ItemDto", Namespace = "urn:Optimization.Dto")]
    public class ItemDto
    {
        [DataMember(Name = "bin", IsRequired = false, EmitDefaultValue = false)]
        public int? BinIndex;

        [DataMember(Name = "item", IsRequired = false, EmitDefaultValue = false)]
        public int? ItemIndex;

        [DataMember(Name = "value", IsRequired = false, EmitDefaultValue = false)]
        public double? Value;

        [DataMember(Name = "weight", IsRequired = false, EmitDefaultValue = false)]
        public double? Weight;
    }
}
