using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Backend.DTOs.Datex2;
using Backend.Models;
using Microsoft.Extensions.Logging;

namespace Backend.Services
{
    public class MeasuredDataService : IMeasuredDataService
    {
        private readonly ITrafficSensorService _trafficSensorService;
        private readonly ILogger<MeasuredDataService> _logger;
        
        // Cache for the measurement site table (doesn't change often)
        private MeasurementSiteTableDto _measurementSiteTable;
        
        public MeasuredDataService(
            ITrafficSensorService trafficSensorService,
            ILogger<MeasuredDataService> logger = null)
        {
            _trafficSensorService = trafficSensorService;
            _logger = logger;
            
            // Initialize measurement site table
            InitializeMeasurementSiteTable();
        }
        
        private void InitializeMeasurementSiteTable()
        {
            // Create a basic measurement site table for our ramp
            _measurementSiteTable = new MeasurementSiteTableDto
            {
                Id = "measurement-site-table-1",
                Version = "1.0",
                MeasurementSites = new List<MeasurementSiteDto>
                {
                    new MeasurementSiteDto
                    {
                        Id = "site-ramp-1",
                        Version = "1.0",
                        MeasurementSpecificCharacteristics = new List<MeasurementSpecificCharacteristicsDto>
                        {
                            new MeasurementSpecificCharacteristicsDto
                            {
                                Index = 1,
                                SpecificMeasurementValueType = "trafficStatus",
                                SpecificLane = new LaneInfoDto
                                {
                                    LaneNumber = 1,
                                    LaneUsage = "mainCarriageway"
                                }
                            }
                        },
                        MeasurementSiteLocation = new LocationReferenceDto
                        {
                            Latitude = 59.3293,
                            Longitude = 18.0686,
                            LocationName = "Highway Exit 42",
                            AffectedCarriageway = "mainCarriageway"
                        }
                    }
                }
            };
        }
        
        public async Task<MeasurementSiteTableDto> GetMeasurementSiteTableAsync()
        {
            return await Task.FromResult(_measurementSiteTable);
        }
        
        public async Task<MeasuredDataPublicationDto> GenerateMeasuredDataAsync(string rampId)
        {
            // Get the current traffic status
            var trafficStatus = await _trafficSensorService.GetTrafficStatusAsync(rampId);
            
            // Convert to MeasuredData format
            return ConvertToMeasuredData(trafficStatus);
        }
        
        public MeasuredDataPublicationDto ConvertToMeasuredData(TrafficStatus status)
        {
            // Create MeasuredDataPublication
            var publication = new MeasuredDataPublicationDto
            {
                Id = $"measured-data-{status.RampId}-{DateTime.UtcNow.Ticks}",
                Version = "1.0",
                PublicationTime = DateTime.UtcNow,
                SenderName = "TrafficMonitor",
                MeasurementSiteTableReference = _measurementSiteTable.Id,
                SiteMeasurements = new List<SiteMeasurementsDto>
                {
                    new SiteMeasurementsDto
                    {
                        Id = $"site-measurement-{status.RampId}-{DateTime.UtcNow.Ticks}",
                        MeasurementTimeDefault = status.Timestamp,
                        MeasurementSiteReference = "site-ramp-1",
                        TrafficStatus = new TrafficStatusDto
                        {
                            // Map the traffic condition to appropriate value
                            TrafficStatusValue = MapTrafficConditionToStatus(status.Condition),
                            
                            // Additional metrics
                            TrafficVolume = status.TotalVehicles,
                            AverageVehicleSpeed = CalculateAverageSpeed(status),
                            OccupancyPercentage = CalculateOccupancy(status)
                        }
                    }
                }
            };
            
            return publication;
        }
        
        public string GenerateXml(MeasuredDataPublicationDto measuredDataDto)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MeasuredDataPublicationDto));
            
            // Add namespaces
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("roa", "http://datex2.eu/schema/3/roadTrafficData");
            namespaces.Add("com", "http://datex2.eu/schema/3/common");
            namespaces.Add("loc", "http://datex2.eu/schema/3/locationReferencing");
            namespaces.Add("d2", "http://datex2.eu/schema/3/d2Payload");
            
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings 
                { 
                    Indent = true,
                    Encoding = Encoding.UTF8
                }))
                {
                    serializer.Serialize(xmlWriter, measuredDataDto, namespaces);
                }
                
                return stringWriter.ToString();
            }
        }
        
        #region Helper Methods
        
        private string MapTrafficConditionToStatus(TrafficCondition condition)
        {
            switch (condition)
            {
                case TrafficCondition.Normal:
                    return "free";
                case TrafficCondition.SlowTraffic:
                    return "heavy";
                case TrafficCondition.QueuingTraffic:
                    return "congested";
                case TrafficCondition.StationaryTraffic:
                    return "impossible";
                default:
                    return "unknown";
            }
        }
        
        private int CalculateAverageSpeed(TrafficStatus status)
        {
            // Calculate approximate speed based on stay times
            // Longer stay times = slower speeds
            double avgStayTime = (status.BoxAAverageStayTimeSeconds + 
                                 status.BoxBAverageStayTimeSeconds + 
                                 status.BoxCAverageStayTimeSeconds) / 3.0;
            
            // Very rough approximation - in a real system this would use actual speed measurements
            if (avgStayTime < 10) return 60; // km/h
            if (avgStayTime < 30) return 40;
            if (avgStayTime < 60) return 20;
            if (avgStayTime < 120) return 10;
            return 5; // very slow
        }
        
        private double CalculateOccupancy(TrafficStatus status)
        {
            // Estimate occupancy as a percentage
            int totalCapacity = 50; // Arbitrary capacity for this ramp
            double occupancyRate = Math.Min(status.TotalVehicles / (double)totalCapacity, 1.0);
            return Math.Round(occupancyRate * 100, 1); // percentage
        }
        
        #endregion
    }
} 