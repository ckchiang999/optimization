using System.Runtime.Serialization;

namespace RoutingOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "RoutingLocationResponseDto", Namespace = "urn:Optimization.Dto")]
    public class LocationResponseDto
    {
        [DataMember(Name = "from_location", IsRequired = true, EmitDefaultValue = false)]
        public string? FromLocation { get; set; }

        [DataMember(Name = "to_location", IsRequired = true, EmitDefaultValue = false)]
        public string? ToLocation { get; set; }

        [DataMember(Name = "distance", IsRequired = true, EmitDefaultValue = true)]
        public long Distance { get; set; }

        public LocationResponseDto()
        {
        }
    }
}
