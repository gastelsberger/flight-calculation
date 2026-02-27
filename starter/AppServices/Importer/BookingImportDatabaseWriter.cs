using Microsoft.EntityFrameworkCore;

namespace AppServices.Importer;

/// <summary>
/// Interface for writing imported booking data to database
/// </summary>
public interface IBookingImportDatabaseWriter
{
    /// <summary>
    /// Writes booking records to database and updates flight aggregates
    /// </summary>
    /// <param name="bookings">Parsed booking records</param>
    /// <returns>Import summary with final statistics</returns>
    Task<ImportSummary> WriteToDatabase(List<BookingImportRecord> bookings);
}

/// <summary>
/// Implementation for writing booking data to database
/// </summary>
public class BookingImportDatabaseWriter : IBookingImportDatabaseWriter
{
    private readonly ApplicationDataContext _db;

    public BookingImportDatabaseWriter(ApplicationDataContext db)
    {
        _db = db;
    }

    public async Task<ImportSummary> WriteToDatabase(List<BookingImportRecord> bookings)
    {
        // TODO: Implement database writing logic
        //
        // Requirements:
        // 1. For each booking record:
        //    - Find or create corresponding Flight entity
        //    - If flight doesn't exist, create with default values:
        //      - SeatCapacity = 220
        //      - BaseFare = lowest ticket price for this flight
        //      - Route = "UNKNOWN"
        //    - Create BookingRecord entity
        //    - Detect duplicates (same flight + booking date + price + passenger count)
        //
        // 2. After all bookings imported, update Flight aggregates:
        //    - SoldSeats = sum of all PassengerCount
        //    - TotalRevenue = sum of all (TicketPrice × PassengerCount)
        //
        // 3. Track statistics:
        //    - SuccessfulImports
        //    - SkippedRows (duplicates)
        //    - Warnings for duplicates
        //
        // Hint: Group bookings by FlightNumber+FlightDate first, then process each group
        
        throw new NotImplementedException("Students must implement this method");
    }
}
