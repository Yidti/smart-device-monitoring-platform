using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

namespace SmartDeviceMonitoring.Web.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Find the project's content root directory
            var currentDirectory = Directory.GetCurrentDirectory();
            var projectDirectory = currentDirectory;

            // Traverse up the directory tree until we find the .csproj file
            while (!Directory.GetFiles(projectDirectory, "*.csproj").Any())
            {
                projectDirectory = Directory.GetParent(projectDirectory)?.FullName;
                if (projectDirectory == null)
                {
                    throw new InvalidOperationException("Project root could not be located.");
                }
            }

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectDirectory) // Set base path to the project root
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }
}