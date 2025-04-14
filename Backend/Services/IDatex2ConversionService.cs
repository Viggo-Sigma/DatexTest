using System.Threading.Tasks;
using Backend.DTOs.Datex2;
using Backend.Models;

namespace Backend.Services
{
    public interface IDatex2ConversionService
    {
        // Convert TrafficStatus to Datex2 DTO
        SituationDto ConvertToDatex2(TrafficStatus status);
        
        // Generate XML from Datex2 DTO
        string GenerateXml(SituationDto situationDto);
        
        // Validate XML against schema
        Task<bool> ValidateXmlAgainstSchema(string xml);
    }
} 