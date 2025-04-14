using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    public class TrafficSensorService : ITrafficSensorService
    {
        // Store the current simulated ramp state
        private readonly Dictionary<string, TrafficRamp> _ramps = new Dictionary<string, TrafficRamp>();
        private readonly Random _random = new Random();
        
        public TrafficSensorService()
        {
            // Initialize with a default ramp
            var defaultRamp = new TrafficRamp
            {
                Id = "ramp-1",
                Name = "Highway Exit 42",
                LastUpdated = DateTime.UtcNow,
                Boxes = new List<TrafficBox>
                {
                    new TrafficBox { Id = "box-a", Name = "A", LastUpdated = DateTime.UtcNow },
                    new TrafficBox { Id = "box-b", Name = "B", LastUpdated = DateTime.UtcNow },
                    new TrafficBox { Id = "box-c", Name = "C", LastUpdated = DateTime.UtcNow }
                }
            };
            
            _ramps.Add(defaultRamp.Id, defaultRamp);
        }
        
        public async Task<TrafficRamp> SimulateSensorDataAsync(string rampId)
        {
            // Ensure ramp exists
            if (!_ramps.TryGetValue(rampId, out var ramp))
            {
                throw new KeyNotFoundException($"Ramp with ID {rampId} not found");
            }
            
            // Generate realistic traffic data based on a random scenario
            int scenario = _random.Next(5); // 0-4 different traffic scenarios
            
            // Reset counts
            foreach (var box in ramp.Boxes)
            {
                box.CarCount = 0;
                box.TruckCount = 0;
                box.BusCount = 0;
                box.AverageVehicleStayTime = TimeSpan.Zero;
            }
            
            switch (scenario)
            {
                case 0: // Light traffic - few vehicles, moving quickly
                    SimulateLightTraffic(ramp);
                    break;
                case 1: // Medium traffic - moderate vehicles, some slowdown
                    SimulateMediumTraffic(ramp);
                    break;
                case 2: // Heavy traffic - many vehicles, significant slowdown
                    SimulateHeavyTraffic(ramp);
                    break;
                case 3: // Queue forming - many vehicles in box C, slowdown
                    SimulateQueueForming(ramp);
                    break;
                case 4: // Queue extended - vehicles in all boxes, very slow
                    SimulateExtendedQueue(ramp);
                    break;
            }
            
            ramp.LastUpdated = DateTime.UtcNow;
            return await Task.FromResult(ramp);
        }
        
        public async Task<TrafficStatus> AnalyzeTrafficAsync(string rampId)
        {
            // First, simulate new sensor data
            var ramp = await SimulateSensorDataAsync(rampId);
            
            // Analyze the data to create a traffic status
            var status = new TrafficStatus
            {
                RampId = ramp.Id,
                RampName = ramp.Name,
                Timestamp = ramp.LastUpdated,
                QueueLength = ramp.GetQueueLength(),
                Severity = ramp.GetTrafficSeverity()
            };
            
            // Set vehicle counts from boxes
            if (ramp.BoxA != null)
            {
                status.BoxAVehicles = ramp.BoxA.TotalVehicleCount;
                status.BoxAAverageStayTimeSeconds = ramp.BoxA.AverageVehicleStayTime.TotalSeconds;
            }
            
            if (ramp.BoxB != null)
            {
                status.BoxBVehicles = ramp.BoxB.TotalVehicleCount;
                status.BoxBAverageStayTimeSeconds = ramp.BoxB.AverageVehicleStayTime.TotalSeconds;
            }
            
            if (ramp.BoxC != null)
            {
                status.BoxCVehicles = ramp.BoxC.TotalVehicleCount;
                status.BoxCAverageStayTimeSeconds = ramp.BoxC.AverageVehicleStayTime.TotalSeconds;
            }
            
            // Determine traffic condition based on severity
            if (status.Severity < 30)
            {
                status.Condition = TrafficCondition.Normal;
                status.AlertRequired = false;
            }
            else if (status.Severity < 60)
            {
                status.Condition = TrafficCondition.SlowTraffic;
                status.AlertRequired = false;
            }
            else if (status.Severity < 80)
            {
                status.Condition = TrafficCondition.QueuingTraffic;
                status.AlertRequired = true;
            }
            else
            {
                status.Condition = TrafficCondition.StationaryTraffic;
                status.AlertRequired = true;
            }
            
            return status;
        }
        
        public async Task<TrafficStatus> GetTrafficStatusAsync(string rampId)
        {
            return await AnalyzeTrafficAsync(rampId);
        }
        
        #region Simulation Helper Methods
        
        private void SimulateLightTraffic(TrafficRamp ramp)
        {
            // Box A (furthest from highway) - few cars, moving quickly
            ramp.BoxA.CarCount = _random.Next(0, 3);
            ramp.BoxA.TruckCount = _random.Next(0, 1);
            ramp.BoxA.BusCount = 0;
            ramp.BoxA.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(5, 15));
            
            // Box B (middle) - very few cars, moving quickly
            ramp.BoxB.CarCount = _random.Next(0, 2);
            ramp.BoxB.TruckCount = 0;
            ramp.BoxB.BusCount = 0;
            ramp.BoxB.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(5, 15));
            
            // Box C (closest to highway) - almost no cars
            ramp.BoxC.CarCount = _random.Next(0, 1);
            ramp.BoxC.TruckCount = 0;
            ramp.BoxC.BusCount = 0;
            ramp.BoxC.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(5, 10));
        }
        
        private void SimulateMediumTraffic(TrafficRamp ramp)
        {
            // Box A - moderate cars
            ramp.BoxA.CarCount = _random.Next(2, 6);
            ramp.BoxA.TruckCount = _random.Next(0, 2);
            ramp.BoxA.BusCount = _random.Next(0, 1);
            ramp.BoxA.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(10, 25));
            
            // Box B - some cars
            ramp.BoxB.CarCount = _random.Next(1, 4);
            ramp.BoxB.TruckCount = _random.Next(0, 1);
            ramp.BoxB.BusCount = 0;
            ramp.BoxB.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(10, 20));
            
            // Box C - few cars
            ramp.BoxC.CarCount = _random.Next(1, 3);
            ramp.BoxC.TruckCount = 0;
            ramp.BoxC.BusCount = 0;
            ramp.BoxC.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(10, 15));
        }
        
        private void SimulateHeavyTraffic(TrafficRamp ramp)
        {
            // Box A - many cars
            ramp.BoxA.CarCount = _random.Next(5, 10);
            ramp.BoxA.TruckCount = _random.Next(1, 3);
            ramp.BoxA.BusCount = _random.Next(0, 2);
            ramp.BoxA.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(20, 40));
            
            // Box B - substantial cars
            ramp.BoxB.CarCount = _random.Next(4, 8);
            ramp.BoxB.TruckCount = _random.Next(1, 2);
            ramp.BoxB.BusCount = _random.Next(0, 1);
            ramp.BoxB.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(20, 35));
            
            // Box C - moderate cars, slowing down
            ramp.BoxC.CarCount = _random.Next(3, 6);
            ramp.BoxC.TruckCount = _random.Next(0, 2);
            ramp.BoxC.BusCount = 0;
            ramp.BoxC.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(15, 30));
        }
        
        private void SimulateQueueForming(TrafficRamp ramp)
        {
            // Box A - many cars, slow moving
            ramp.BoxA.CarCount = _random.Next(6, 12);
            ramp.BoxA.TruckCount = _random.Next(1, 4);
            ramp.BoxA.BusCount = _random.Next(0, 2);
            ramp.BoxA.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(30, 60));
            
            // Box B - many cars, slow moving
            ramp.BoxB.CarCount = _random.Next(5, 10);
            ramp.BoxB.TruckCount = _random.Next(1, 3);
            ramp.BoxB.BusCount = _random.Next(0, 1);
            ramp.BoxB.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(25, 55));
            
            // Box C - heavy congestion, cars staying long
            ramp.BoxC.CarCount = _random.Next(8, 15);
            ramp.BoxC.TruckCount = _random.Next(2, 5);
            ramp.BoxC.BusCount = _random.Next(0, 2);
            ramp.BoxC.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(40, 90));
        }
        
        private void SimulateExtendedQueue(TrafficRamp ramp)
        {
            // Box A - heavy congestion, very slow
            ramp.BoxA.CarCount = _random.Next(10, 20);
            ramp.BoxA.TruckCount = _random.Next(3, 6);
            ramp.BoxA.BusCount = _random.Next(1, 3);
            ramp.BoxA.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(50, 120));
            
            // Box B - heavy congestion, very slow
            ramp.BoxB.CarCount = _random.Next(8, 15);
            ramp.BoxB.TruckCount = _random.Next(2, 5);
            ramp.BoxB.BusCount = _random.Next(0, 2);
            ramp.BoxB.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(45, 100));
            
            // Box C - complete congestion, nearly standstill
            ramp.BoxC.CarCount = _random.Next(12, 25);
            ramp.BoxC.TruckCount = _random.Next(3, 8);
            ramp.BoxC.BusCount = _random.Next(1, 3);
            ramp.BoxC.AverageVehicleStayTime = TimeSpan.FromSeconds(_random.Next(60, 180));
        }
        
        #endregion
    }
} 