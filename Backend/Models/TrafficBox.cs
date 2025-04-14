using System;

namespace Backend.Models
{
    public class TrafficBox
    {
        public string Id { get; set; }
        public string Name { get; set; } // A, B, or C
        public int CarCount { get; set; }
        public int TruckCount { get; set; }
        public int BusCount { get; set; }
        public DateTime LastUpdated { get; set; }
        public TimeSpan AverageVehicleStayTime { get; set; } // How long vehicles stay in this box
        public bool IsOccupied => CarCount + TruckCount + BusCount > 0;
        
        // Calculate total vehicle count
        public int TotalVehicleCount => CarCount + TruckCount + BusCount;
    }
} 