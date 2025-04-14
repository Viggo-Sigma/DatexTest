using Backend.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add controllers
builder.Services.AddControllers();

// Register logging
builder.Services.AddLogging();

// Register our services
builder.Services.AddSingleton<ITrafficSensorService, TrafficSensorService>();
builder.Services.AddSingleton<IDatex2ConversionService, Datex2ConversionService>();
builder.Services.AddSingleton<SchemaSetupService>();

// Setup CORS for frontend - Allow any origin for development/testing
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Comment out HTTPS redirection for development testing
// app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Copy Datex2 schema files
try
{
    var schemaService = app.Services.GetRequiredService<SchemaSetupService>();
    
    // As a fallback, let's manually copy schema files if they're not already present
    string destDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datex2");
    if (!Directory.Exists(destDir))
    {
        Directory.CreateDirectory(destDir);
    }
    
    string sourceDir = Path.Combine(
        Directory.GetParent(app.Environment.ContentRootPath).FullName, 
        "frontend", "datex2");
        
    if (Directory.Exists(sourceDir))
    {
        string[] files = Directory.GetFiles(sourceDir, "*.xsd");
        
        // Only copy files if we found any
        if (files.Length > 0)
        {
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, true);
                Console.WriteLine($"Copied schema file: {fileName}");
            }
            Console.WriteLine($"Manually copied {files.Length} schema files");
        }
    }
    
    // Call the service method too for completeness
    schemaService.CopySchemaFiles();
}
catch (Exception ex)
{
    Console.WriteLine($"Error copying schema files: {ex.Message}");
}

// Enable controllers
app.MapControllers();

// Log the URLs that the server is listening on
app.Lifetime.ApplicationStarted.Register(() => 
{
    var urls = app.Urls;
    Console.WriteLine("Server started, listening on:");
    foreach (var url in urls)
    {
        Console.WriteLine($"- {url}");
    }
});

app.Run();
