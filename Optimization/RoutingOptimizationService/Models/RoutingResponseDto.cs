using System.Runtime.Serialization;

namespace RoutingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "RoutingResponseDto", Namespace = "urn:Optimization.Dto")]
    public class RoutingResponseDto
    {
        [DataMember(Name = "routes", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<RouteResponseDto> Routes { get; set; } = new List<RouteResponseDto>();

        [DataMember(Name = "max_distance", IsRequired = true, EmitDefaultValue = true)]
        public long MaximumDistance;

        [DataMember(Name = "total_distance", IsRequired = true, EmitDefaultValue = true)]
        public long TotalDistance;
    }
}
