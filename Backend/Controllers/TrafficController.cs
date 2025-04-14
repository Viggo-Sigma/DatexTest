using System;
using System.Threading.Tasks;
using Backend.DTOs.Datex2;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrafficController : ControllerBase
    {
        private readonly ITrafficSensorService _trafficSensorService;
        private readonly IDatex2ConversionService _datex2ConversionService;
        private readonly ILogger<TrafficController> _logger;
        
        public TrafficController(
            ITrafficSensorService trafficSensorService,
            IDatex2ConversionService datex2ConversionService,
            ILogger<TrafficController> logger)
        {
            _trafficSensorService = trafficSensorService;
            _datex2ConversionService = datex2ConversionService;
            _logger = logger;
        }
        
        [HttpGet("status/{rampId}")]
        public async Task<ActionResult<TrafficStatus>> GetTrafficStatus(string rampId)
        {
            try
            {
                var status = await _trafficSensorService.GetTrafficStatusAsync(rampId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting traffic status");
                return StatusCode(500, "An error occurred while getting traffic status");
            }
        }
        
        [HttpGet("datex2/{rampId}")]
        public async Task<ActionResult> GetDatex2Data(string rampId)
        {
            try
            {
                // Get traffic status
                var status = await _trafficSensorService.GetTrafficStatusAsync(rampId);
                
                // Convert to Datex2
                var datex2Dto = _datex2ConversionService.ConvertToDatex2(status);
                
                // Generate XML
                var xml = _datex2ConversionService.GenerateXml(datex2Dto);
                
                bool isValid = false;
                try {
                    // Validate the XML against the schema
                    isValid = await _datex2ConversionService.ValidateXmlAgainstSchema(xml);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "XML validation failed");
                    // Continue without failing the entire request
                    isValid = false;
                }
                
                // Return XML with validation status in a header
                Response.Headers.Add("X-Datex2-Valid", isValid.ToString());
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Datex2-Valid");
                
                return Content(xml, "application/xml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Datex2 data");
                return StatusCode(500, "An error occurred while generating Datex2 data");
            }
        }
        
        [HttpGet("validate/{rampId}")]
        public async Task<ActionResult<object>> ValidateRampData(string rampId)
        {
            try
            {
                // Get traffic status
                var status = await _trafficSensorService.GetTrafficStatusAsync(rampId);
                
                // Convert to Datex2
                var datex2Dto = _datex2ConversionService.ConvertToDatex2(status);
                
                // Generate XML
                var xml = _datex2ConversionService.GenerateXml(datex2Dto);
                
                bool isValid = false;
                string message = "XML validation could not be completed";
                
                try {
                    // Validate the XML against the schema
                    isValid = await _datex2ConversionService.ValidateXmlAgainstSchema(xml);
                    message = isValid ? "XML is valid against Datex2 schema" : "XML failed validation against Datex2 schema";
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "XML validation failed");
                    isValid = false;
                    message = $"XML validation error: {ex.Message}";
                }
                
                return Ok(new { isValid, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Datex2 data");
                return StatusCode(500, "An error occurred while validating Datex2 data");
            }
        }
        
        [HttpGet("simulate/{rampId}")]
        public async Task<ActionResult<TrafficRamp>> SimulateTrafficData(string rampId)
        {
            try
            {
                var ramp = await _trafficSensorService.SimulateSensorDataAsync(rampId);
                return Ok(ramp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error simulating traffic data");
                return StatusCode(500, "An error occurred while simulating traffic data");
            }
        }
    }
} 