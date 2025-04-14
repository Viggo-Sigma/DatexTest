using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    public interface ITrafficSensorService
    {
        // Simulate getting sensor data from virtual boxes
        Task<TrafficRamp> SimulateSensorDataAsync(string rampId);
        
        // Analyze traffic conditions and generate status
        Task<TrafficStatus> AnalyzeTrafficAsync(string rampId);
        
        // Get the current status of a ramp
        Task<TrafficStatus> GetTrafficStatusAsync(string rampId);
    }
} 