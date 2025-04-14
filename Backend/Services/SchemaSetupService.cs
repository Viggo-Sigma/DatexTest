using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.Services
{
    public class SchemaSetupService
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<SchemaSetupService> _logger;

        public SchemaSetupService(IHostEnvironment hostEnvironment, ILogger<SchemaSetupService> logger = null)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        private void LogInformation(string message, params object[] args)
        {
            _logger?.LogInformation(message, args);
        }

        private void LogError(Exception ex, string message)
        {
            _logger?.LogError(ex, message);
        }

        public void CopySchemaFiles()
        {
            try
            {
                // Source directory (relative to the content root)
                string sourceDir = Path.Combine(
                    Directory.GetParent(_hostEnvironment.ContentRootPath).FullName, 
                    "frontend", "datex2");

                // Destination directory (in the application's output directory)
                string destDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datex2");

                // Create the destination directory if it doesn't exist
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                // Get all XSD files from the source directory
                string[] xsdFiles = Directory.GetFiles(sourceDir, "*.xsd");

                // Copy each file to the destination directory
                foreach (string xsdFile in xsdFiles)
                {
                    string fileName = Path.GetFileName(xsdFile);
                    string destFile = Path.Combine(destDir, fileName);
                    File.Copy(xsdFile, destFile, true); // Overwrite if exists
                    LogInformation($"Copied schema file: {fileName}");
                }

                LogInformation($"Successfully copied {xsdFiles.Length} schema files");
            }
            catch (Exception ex)
            {
                LogError(ex, "Error copying Datex2 schema files");
            }
        }
    }
} 