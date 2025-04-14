using System;
using System.Collections.Generic;

namespace Backend.DTOs.Datex2
{
    public class MeasuredDataPublicationDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Version { get; set; } = "1.0";
        
        // Header information
        public string SenderName { get; set; } = "TrafficMonitor";
        public DateTime PublicationTime { get; set; } = DateTime.UtcNow;
        
        // Reference to MeasurementSiteTable
        public string MeasurementSiteTableReference { get; set; }
        
        // Measurements
        public List<SiteMeasurementsDto> SiteMeasurements { get; set; } = new List<SiteMeasurementsDto>();
    }
    
    public class SiteMeasurementsDto
    {
        public string Id { get; set; }
        public DateTime MeasurementTimeDefault { get; set; }
        public string MeasurementSiteReference { get; set; }
        
        // The actual traffic status data
        public TrafficStatusDto TrafficStatus { get; set; }
    }
    
    public class TrafficStatusDto
    {
        public string TrafficStatusValue { get; set; }
        
        // Optional - for more advanced implementations
        public int? AverageVehicleSpeed { get; set; }
        public int? TrafficVolume { get; set; }
        public double? OccupancyPercentage { get; set; }
    }
    
    public class MeasurementSiteTableDto
    {
        public string Id { get; set; }
        public string Version { get; set; } = "1.0";
        
        // Collection of measurement sites
        public List<MeasurementSiteDto> MeasurementSites { get; set; } = new List<MeasurementSiteDto>();
    }
    
    public class MeasurementSiteDto
    {
        public string Id { get; set; }
        public string Version { get; set; } = "1.0";
        
        // Characteristics - what is being measured
        public List<MeasurementSpecificCharacteristicsDto> MeasurementSpecificCharacteristics { get; set; } = 
            new List<MeasurementSpecificCharacteristicsDto>();
        
        // Location reference
        public LocationReferenceDto MeasurementSiteLocation { get; set; }
    }
    
    public class MeasurementSpecificCharacteristicsDto
    {
        public int Index { get; set; }
        public string SpecificMeasurementValueType { get; set; } = "trafficStatus";
        
        // Lane information for lane-specific measurements (if needed)
        public LaneInfoDto SpecificLane { get; set; }
    }
    
    public class LaneInfoDto
    {
        public int LaneNumber { get; set; } = 1;
        public string LaneUsage { get; set; } = "mainCarriageway";
    }
    
    public class LocationReferenceDto
    {
        // Basic coordinate reference
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; }
        
        // For lane-specific data if needed
        public string AffectedCarriageway { get; set; } = "mainCarriageway";
    }
} 