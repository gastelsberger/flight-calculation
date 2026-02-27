using System.Text.RegularExpressions;

namespace AppServices.Importer;

/// <summary>
/// Data transfer object for booking import summary
/// </summary>
public class ImportSummary
{
    public int TotalRowsProcessed { get; set; }
    public int SuccessfulImports { get; set; }
    public int SkippedRows { get; set; }
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Interface for parsing booking CSV content
/// </summary>
public interface IBookingDataCsvParser
{
    /// <summary>
    /// Parses CSV content into booking records with validation
    /// </summary>
    /// <param name="csvContent">CSV content as string</param>
    /// <returns>Tuple of successfully parsed bookings and import summary</returns>
    (List<BookingImportRecord> bookings, ImportSummary summary) ParseCsv(string csvContent);
}

/// <summary>
/// Temporary record for import (before saving to database)
/// </summary>
public class BookingImportRecord
{
    public string FlightNumber { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public decimal TicketPrice { get; set; }
    public int PassengerCount { get; set; }
    public DateTime FlightDate { get; set; }
}

/// <summary>
/// Implementation for parsing booking CSV files
/// </summary>
public class BookingDataCsvParser : IBookingDataCsvParser
{
    private static readonly Regex FlightNumberPattern = new Regex(@"^[A-Z]{2}\d{4}$", RegexOptions.Compiled);

    public (List<BookingImportRecord> bookings, ImportSummary summary) ParseCsv(string csvContent)
    {
        // TODO: Implement CSV parsing logic
        // Expected format: FlightNumber,BookingDate,TicketPrice,PassengerCount,FlightDate
        // 
        // Validation rules:
        // 1. BookingDate must be before FlightDate
        // 2. TicketPrice must be > 0 and < 1000
        // 3. PassengerCount must be >= 1 and <= 9
        // 4. FlightNumber must match pattern: 2 letters + 4 digits (e.g., FR1234)
        //
        // For invalid rows:
        // - Skip the row
        // - Add to summary.SkippedRows
        // - Add descriptive warning to summary.Warnings
        //
        // Hint: Use FlightNumberPattern.IsMatch() to validate flight numbers
        
        throw new NotImplementedException("Students must implement this method");
    }
}
