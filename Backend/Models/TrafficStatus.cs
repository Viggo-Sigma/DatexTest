using System;

namespace Backend.Models
{
    public enum TrafficCondition
    {
        Normal,
        SlowTraffic,
        QueuingTraffic,
        StationaryTraffic
    }
    
    public class TrafficStatus
    {
        public string RampId { get; set; }
        public string RampName { get; set; }
        public DateTime Timestamp { get; set; }
        public TrafficCondition Condition { get; set; }
        public int QueueLength { get; set; } // in meters
        public int Severity { get; set; } // 0-100
        public bool AlertRequired { get; set; }
        
        // Box statistics
        public int BoxAVehicles { get; set; }
        public int BoxBVehicles { get; set; }
        public int BoxCVehicles { get; set; }
        
        public double BoxAAverageStayTimeSeconds { get; set; }
        public double BoxBAverageStayTimeSeconds { get; set; }
        public double BoxCAverageStayTimeSeconds { get; set; }
        
        public int TotalVehicles => BoxAVehicles + BoxBVehicles + BoxCVehicles;
    }
} 