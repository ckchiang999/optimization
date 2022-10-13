using System.Runtime.Serialization;

namespace BinPackingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "BinPackingResponseDto", Namespace = "urn:Optimization.Dto")]
    public class BinPackingResponseDto
    {
        [DataMember(Name = "capacities", IsRequired = false, EmitDefaultValue = false)]
        public ICollection<double?> Capacities { get; set; } = new List<double?>();

        [DataMember(Name = "optimal_value", IsRequired = false, EmitDefaultValue = false)]
        public double? OptimalValue;

        [DataMember(Name = "items", IsRequired = false, EmitDefaultValue = false)]
        public ICollection<ItemDto> Items { get; set; } = new List<ItemDto>();

    }
}
