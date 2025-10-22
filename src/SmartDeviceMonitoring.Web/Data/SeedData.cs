using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartDeviceMonitoring.Web.Models;

namespace SmartDeviceMonitoring.Web.Data
{
    public static class SeedData
    {
        private class SeedDataContent
        {
            public List<Device> Devices { get; set; } = new List<Device>(); // Initialized
            public List<SensorType> SensorTypes { get; set; } = new List<SensorType>(); // Initialized
        }

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                // Seed SensorTypes
                if (!context.SensorTypes.Any())
                {
                    var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "seeddata.json");
                    if (!File.Exists(jsonFilePath))
                    {
                        jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "src", "SmartDeviceMonitoring.Web", "Data", "seeddata.json");
                    }

                    if (File.Exists(jsonFilePath))
                    {
                        var jsonString = await File.ReadAllTextAsync(jsonFilePath);
                        var seedContent = JsonSerializer.Deserialize<SeedDataContent>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (seedContent?.SensorTypes != null && seedContent.SensorTypes.Any())
                        {
                            foreach (var sensorType in seedContent.SensorTypes)
                            {
                                context.SensorTypes.Add(sensorType);
                            }
                            await context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        // Fallback: If JSON file not found, create some default sensor types
                        context.SensorTypes.AddRange(
                            new SensorType { TypeName = "Temperature", Unit = "°C", Description = "Measures temperature" },
                            new SensorType { TypeName = "Humidity", Unit = "%", Description = "Measures relative humidity" },
                            new SensorType { TypeName = "Pressure", Unit = "psi", Description = "Measures pressure" }
                        );
                        await context.SaveChangesAsync();
                    }
                }

                // Seed Devices
                if (!context.Devices.Any())
                {
                    var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "seeddata.json");
                    if (!File.Exists(jsonFilePath))
                    {
                        jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "src", "SmartDeviceMonitoring.Web", "Data", "seeddata.json");
                    }

                    if (File.Exists(jsonFilePath))
                    {
                        var jsonString = await File.ReadAllTextAsync(jsonFilePath);
                        var seedContent = JsonSerializer.Deserialize<SeedDataContent>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (seedContent?.Devices != null && seedContent.Devices.Any())
                        {
                            foreach (var device in seedContent.Devices)
                            {
                                device.CreatedAt = DateTime.UtcNow;
                                device.UpdatedAt = DateTime.UtcNow;
                                context.Devices.Add(device);
                            }
                            await context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        // Fallback: If JSON file not found, create some default devices
                        context.Devices.AddRange(
                            new Device
                            {
                                DeviceName = "Default Device 1",
                                Location = "Lab",
                                Description = "First default device",
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            },
                            new Device
                            {
                                DeviceName = "Default Device 2",
                                Location = "Warehouse",
                                Description = "Second default device",
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            }
                        );
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
