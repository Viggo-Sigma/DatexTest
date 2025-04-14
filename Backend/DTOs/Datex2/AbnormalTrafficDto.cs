using System;

namespace Backend.DTOs.Datex2
{
    public enum AbnormalTrafficTypeEnum
    {
        StationaryTraffic,
        QueuingTraffic,
        SlowTraffic
    }
    
    public class AbnormalTrafficDto
    {
        public string Id { get; set; }
        public string Version { get; set; } = "1.0";
        
        // SituationRecord properties
        public DateTime SituationRecordCreationTime { get; set; }
        public DateTime SituationRecordVersionTime { get; set; }
        public string ProbabilityOfOccurrence { get; set; } = "certain";
        
        // Validity
        public DateTime ValidityStartTime { get; set; }
        public DateTime? ValidityEndTime { get; set; }
        
        // Location
        public string LocationName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        // AbnormalTraffic specific
        public AbnormalTrafficTypeEnum AbnormalTrafficType { get; set; }
        public int QueueLength { get; set; }
    }
} 