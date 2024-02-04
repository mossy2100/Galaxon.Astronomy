using Galaxon.Astronomy.Data;
using Galaxon.Astronomy.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Galaxon.Astronomy.DataImport;

public class Program
{
    public static async Task Main()
    {
        // Setup DI container
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddDbContext<AstroDbContext>()
            .AddScoped<DataImportService>()
            .AddScoped<AstroObjectRepository>()
            .AddScoped<AstroObjectGroupRepository>()
            .AddScoped<LeapSecondRepository>()
            // Register any other dependencies your DataImportService requires
            // Example: .AddScoped<IService, Service>()
            .BuildServiceProvider();

        // Resolve DataImportService from the DI container
        // var dataImportService = serviceProvider.GetRequiredService<DataImportService>();
        // dataImportService.ImportData();

        // Parse leap seconds and copy into database.
        var leapSecondRepository = serviceProvider.GetRequiredService<LeapSecondRepository>();
        await leapSecondRepository.ParseNistWebPage();
        await leapSecondRepository.ImportIersBulletins();

        // Dispose the service provider to clean up resources
        await serviceProvider.DisposeAsync();
    }
}
