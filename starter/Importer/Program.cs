using AppServices;
using AppServices.Importer;
using Importer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Build the host with dependency injection
var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// Configure services
ConfigureServices(builder.Services, builder.Configuration);

var host = builder.Build();

try
{
    // Parse command line arguments
    var parser = new CommandLineParser();
    var parsedArgs = parser.Parse(args);

    // Validate file exists
    if (!File.Exists(parsedArgs.CsvFilePath))
    {
        Console.Error.WriteLine($"Error: File '{parsedArgs.CsvFilePath}' not found.");
        return 1;
    }

    // Handle different commands
    if (parsedArgs.Command == "import-bookings")
    {
        var bookingImporter = host.Services.GetRequiredService<IBookingDataImporter>();
        var summary = await bookingImporter.ImportFromCsvAsync(parsedArgs.CsvFilePath);

        Console.WriteLine($"\nProcessing {parsedArgs.CsvFilePath}...");
        Console.WriteLine($"Total rows: {summary.TotalRowsProcessed}");
        Console.WriteLine($"Successful imports: {summary.SuccessfulImports}");
        Console.WriteLine($"Skipped (invalid): {summary.SkippedRows}");

        if (summary.Warnings.Any())
        {
            Console.WriteLine("Warnings:");
            foreach (var warning in summary.Warnings.Take(10))
            {
                Console.WriteLine($"  - {warning}");
            }
            if (summary.Warnings.Count > 10)
            {
                Console.WriteLine($"  ... and {summary.Warnings.Count - 10} more warnings");
            }
        }

        Console.WriteLine("Import complete.");
        return 0;
    }
    else
    {
        // Legacy import (not used in this exercise, kept for compatibility)
        Console.WriteLine("Legacy import not implemented. Use: import-bookings --file <path>");
        return 1;
    }
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
catch (FileNotFoundException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"\nError occurred: {ex.Message}");
    Console.Error.WriteLine("Import failed.");
    return 1;
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Register database context
    var path = configuration["Database:path"] ?? throw new InvalidOperationException("Database path not configured.");
    var fileName = configuration["Database:fileName"] ?? throw new InvalidOperationException("Database file name not configured.");
    var connectionString = $"Data Source={path}/{fileName}";

    services.AddDbContext<ApplicationDataContext>(options =>
        options.UseSqlite(connectionString));

    // Register application services for booking import
    services.AddScoped<IFileReader, FileReader>();
    services.AddScoped<IBookingDataCsvParser, BookingDataCsvParser>();
    services.AddScoped<IBookingImportDatabaseWriter, BookingImportDatabaseWriter>();
    services.AddScoped<IBookingDataImporter, BookingDataImporter>();
}
