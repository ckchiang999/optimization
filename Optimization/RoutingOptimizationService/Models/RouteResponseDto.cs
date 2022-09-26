using System.Runtime.Serialization;

namespace RoutingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "RouteLocationResponseDto", Namespace = "urn:Optimization.Dto")]
    public class RouteResponseDto
    {
        [DataMember(Name = "locations", IsRequired = true, EmitDefaultValue = false)]
        public ICollection<LocationResponseDto> Locations { get; set; } = new List<LocationResponseDto>();

        [DataMember(Name = "total_distance", IsRequired = false, EmitDefaultValue = true)]
        public long? TotalDistance;

        [DataMember(Name = "total_load", IsRequired = false, EmitDefaultValue = true)]
        public long? TotalLoad;

        [DataMember(Name = "total_time", IsRequired = false, EmitDefaultValue = true)]
        public long? TotalTime;
    }
}
