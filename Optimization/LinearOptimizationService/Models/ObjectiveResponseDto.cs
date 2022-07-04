﻿using System.Runtime.Serialization;

namespace LinearOptimizationService.Models
{
    [Serializable]
    [DataContract(Name = "ObjectiveResponseDto", Namespace = "urn:Optimization.Dto")]
    public class ObjectiveResponseDto
    {
        [DataMember(Name = "value", IsRequired = true, EmitDefaultValue = false)]
        public double? Value { get; set; }

        public ObjectiveResponseDto()
        {
        }
    }
}
