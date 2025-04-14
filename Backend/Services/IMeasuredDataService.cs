using System.Threading.Tasks;
using Backend.DTOs.Datex2;
using Backend.Models;

namespace Backend.Services
{
    public interface IMeasuredDataService
    {
        // Generate a MeasurementSiteTable for this installation
        Task<MeasurementSiteTableDto> GetMeasurementSiteTableAsync();
        
        // Generate measured data (regular interval reporting)
        Task<MeasuredDataPublicationDto> GenerateMeasuredDataAsync(string rampId);
        
        // Convert TrafficStatus to Datex2 MeasuredData format
        MeasuredDataPublicationDto ConvertToMeasuredData(TrafficStatus status);
        
        // Generate XML from MeasuredData
        string GenerateXml(MeasuredDataPublicationDto measuredDataDto);
    }
} 