using System;
using System.Collections.Generic;

namespace Backend.DTOs.Datex2
{
    public class SituationDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        // Header info
        public string SenderName { get; set; } = "TrafficMonitor";
        public DateTime PublicationTime { get; set; } = DateTime.UtcNow;
        
        // Records
        public List<AbnormalTrafficDto> SituationRecords { get; set; } = new List<AbnormalTrafficDto>();
    }
} 