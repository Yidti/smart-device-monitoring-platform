using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartDeviceMonitoring.Web.Data;
using SmartDeviceMonitoring.Web.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDeviceMonitoring.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.SetBasePath(Directory.GetCurrentDirectory());
                    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configuration.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddHostedService<SimulatorService>();
                });
    }

    public class SimulatorService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public SimulatorService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Simulator Service running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var devices = await context.Devices.Where(d => d.IsActive).ToListAsync();

                if (!devices.Any())
                {
                    Console.WriteLine("No active devices found to simulate data for.");
                    return;
                }

                foreach (var device in devices)
                {
                    // Simulate data for each device (e.g., temperature, humidity)
                    // For simplicity, let's assume each device has a single sensor for now
                    // In a real scenario, we would fetch sensors associated with the device

                    var random = new Random();
                    var simulatedValue = (decimal)(random.NextDouble() * 100); // Value between 0 and 100

                    var sensor = await context.Sensors.FirstOrDefaultAsync(s => s.DeviceId == device.DeviceId);

                    if (sensor == null)
                    {
                        // Create a dummy sensor if none exists for the device
                        sensor = new Sensor
                        {
                            SensorName = $"{device.DeviceName} - Temp Sensor",
                            DeviceId = device.DeviceId,
                            SensorTypeId = 1, // Assuming SensorTypeId 1 exists (e.g., Temperature)
                            MinThreshold = 20,
                            MaxThreshold = 80,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        context.Sensors.Add(sensor);
                        await context.SaveChangesAsync();
                        Console.WriteLine($"Created dummy sensor for device {device.DeviceName}");
                    }

                    var sensorData = new SensorData
                    {
                        SensorId = sensor.SensorId,
                        Value = simulatedValue,
                        Timestamp = DateTime.UtcNow
                    };
                    context.SensorData.Add(sensorData);

                    // Check for alerts
                    if (simulatedValue < sensor.MinThreshold || simulatedValue > sensor.MaxThreshold)
                    {
                        var alert = new Alert
                        {
                            SensorId = sensor.SensorId,
                            AlertType = simulatedValue < sensor.MinThreshold ? "Low Threshold Exceeded" : "High Threshold Exceeded",
                            TriggerValue = simulatedValue,
                            AlertTime = DateTime.UtcNow,
                            IsResolved = false
                        };
                        context.Alerts.Add(alert);
                        Console.WriteLine($"ALERT! Device: {device.DeviceName}, Sensor: {sensor.SensorName}, Value: {simulatedValue}");
                    }

                    await context.SaveChangesAsync();
                    Console.WriteLine($"Simulated data for Device: {device.DeviceName}, Value: {simulatedValue}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Simulator Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}