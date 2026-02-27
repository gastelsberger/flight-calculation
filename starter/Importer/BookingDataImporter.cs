using AppServices;
using AppServices.Importer;

namespace Importer;

/// <summary>
/// Service for importing booking data from CSV files
/// </summary>
public interface IBookingDataImporter
{
    Task<ImportSummary> ImportFromCsvAsync(string csvFilePath);
}

/// <summary>
/// Implementation of booking data importer
/// </summary>
public class BookingDataImporter : IBookingDataImporter
{
    private readonly IFileReader _fileReader;
    private readonly IBookingDataCsvParser _csvParser;
    private readonly IBookingImportDatabaseWriter _databaseWriter;

    public BookingDataImporter(
        IFileReader fileReader,
        IBookingDataCsvParser csvParser,
        IBookingImportDatabaseWriter databaseWriter)
    {
        _fileReader = fileReader;
        _csvParser = csvParser;
        _databaseWriter = databaseWriter;
    }

    public async Task<ImportSummary> ImportFromCsvAsync(string csvFilePath)
    {
        // TODO: Students implement this method
        // 1. Read CSV file using _fileReader.ReadFileAsync()
        // 2. Parse CSV using _csvParser.ParseCsv()
        // 3. Write to database using _databaseWriter.WriteToDatabase()
        // 4. Return combined import summary
        
        throw new NotImplementedException("Students must implement this method");
    }
}
