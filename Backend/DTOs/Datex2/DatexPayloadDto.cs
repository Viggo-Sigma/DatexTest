using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Backend.DTOs.Datex2
{
    [XmlRoot("payload", Namespace = "http://datex2.eu/schema/3/d2Payload")]
    public class DatexPayloadDto
    {
        [XmlElement("payload")]
        public SituationDto Situation { get; set; }
    }
} 