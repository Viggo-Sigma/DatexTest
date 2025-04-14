using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Models
{
    public class TrafficRamp
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<TrafficBox> Boxes { get; set; } = new List<TrafficBox>();
        public DateTime LastUpdated { get; set; }
        
        // C is closest to highway, A is furthest from highway (deeper into the ramp)
        public TrafficBox BoxA => Boxes.FirstOrDefault(b => b.Name == "A");
        public TrafficBox BoxB => Boxes.FirstOrDefault(b => b.Name == "B");
        public TrafficBox BoxC => Boxes.FirstOrDefault(b => b.Name == "C");
        
        // Determine if there is queueing traffic based on the occupancy of boxes
        public bool HasQueueingTraffic()
        {
            // If box C (closest to highway) is occupied with vehicles that stay longer than threshold
            // and at least one of the other boxes is also occupied, we have queueing
            bool boxCBusy = BoxC != null && BoxC.IsOccupied && BoxC.AverageVehicleStayTime.TotalSeconds > 30;
            bool otherBoxesOccupied = (BoxB != null && BoxB.IsOccupied) || (BoxA != null && BoxA.IsOccupied);
            
            return boxCBusy && otherBoxesOccupied;
        }
        
        // Returns the severity of the traffic situation (0-100)
        public int GetTrafficSeverity()
        {
            if (Boxes.Count == 0 || Boxes.All(b => !b.IsOccupied))
                return 0;
                
            // Calculate severity based on:
            // 1. How many boxes are occupied
            // 2. Vehicle counts
            // 3. Average stay time
            
            int occupiedBoxes = Boxes.Count(b => b.IsOccupied);
            int totalVehicles = Boxes.Sum(b => b.TotalVehicleCount);
            double avgStayTimeSeconds = Boxes.Where(b => b.IsOccupied)
                .Average(b => b.AverageVehicleStayTime.TotalSeconds);
                
            // More boxes occupied, more vehicles and longer stay times = higher severity
            int severityScore = (int)(
                (occupiedBoxes / 3.0 * 30) +
                (Math.Min(totalVehicles, 50) / 50.0 * 40) +
                (Math.Min(avgStayTimeSeconds, 120) / 120.0 * 30)
            );
            
            return Math.Min(severityScore, 100);
        }
        
        // Get the queue length in meters (rough estimate)
        public int GetQueueLength()
        {
            int occupiedBoxes = Boxes.Count(b => b.IsOccupied);
            int totalVehicles = Boxes.Sum(b => b.TotalVehicleCount);
            
            // Rough calculation - assuming each vehicle takes about 7 meters
            // and the boxes are 100 meters long each
            int occupiedLength = occupiedBoxes * 100;
            int vehicleContribution = totalVehicles * 7;
            
            // Average of the two approaches
            return (occupiedLength + vehicleContribution) / 2;
        }
    }
} 