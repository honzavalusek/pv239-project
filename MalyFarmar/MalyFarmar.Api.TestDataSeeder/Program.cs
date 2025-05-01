using MalyFarmar.Api.DAL.Data;
using MalyFarmar.Api.TestDataSeeder.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MalyFarmar.Api.TestDataSeeder;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                Console.WriteLine("Starting data seeding...");
                var seeder = services.GetRequiredService<IDataSeeder>();
                await seeder.SeedAsync();
                Console.WriteLine("Data seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false);
                config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Configure database connection
                var sqliteDbDefaultName = hostContext.Configuration.GetConnectionString("DefaultConnection") ??
                                          throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                var dataFolder = Environment.SpecialFolder.LocalApplicationData;
                var basePath = Environment.GetFolderPath(dataFolder);
                var connectionString = $"Data Source={Path.Combine(basePath, sqliteDbDefaultName)}";

                services.AddDbContext<MalyFarmarDbContext>(options =>
                    options
                        .UseSqlite(
                            connectionString,
                            x => x
                                .MigrationsAssembly("MalyFarmar.Api.DAL")
                        ));

                // Register services
                services.AddTransient<IDataSeeder, Services.TestDataSeeder>();
            });
}
