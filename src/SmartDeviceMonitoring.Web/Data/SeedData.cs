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
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any devices.
                if (context.Devices.Any())
                {
                    return;   // DB has been seeded
                }

                // Read seeddata.json
                var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "seeddata.json");
                if (!File.Exists(jsonFilePath))
                {
                    // Fallback for when running from the project root directly
                    jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "src", "SmartDeviceMonitoring.Web", "Data", "seeddata.json");
                }

                if (File.Exists(jsonFilePath))
                {
                    var jsonString = await File.ReadAllTextAsync(jsonFilePath);
                    var devices = JsonSerializer.Deserialize<List<Device>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (devices != null && devices.Any())
                    {
                        foreach (var device in devices)
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
                    // If JSON file not found, create some default devices
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
