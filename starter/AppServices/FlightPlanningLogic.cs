namespace AppServices;

// ===== Configuration Optimizer =====

public interface IConfigurationOptimizer
{
    ConfigurationRecommendation RecommendConfiguration(string routeCode, List<BookingRecord> historicalBookings);
}

public class ConfigurationRecommendation
{
    public int RecommendedCapacity { get; set; }  // 220 or 240
    public decimal ExpectedLoadFactor { get; set; }  // 0.00 to 1.00
    public string Reasoning { get; set; } = string.Empty;
}

public class ConfigurationOptimizer : IConfigurationOptimizer
{
    public ConfigurationRecommendation RecommendConfiguration(string routeCode, List<BookingRecord> historicalBookings)
    {
        // TODO: Implement configuration optimization logic
        // 1. Calculate average sold seats from historical bookings
        // 2. Determine optimal configuration based on business rules
        // 3. Calculate expected load factor
        // 4. Generate reasoning text
        
        throw new NotImplementedException("Students must implement this method");
    }
}

// ===== Schedule Validator =====

public interface IScheduleValidator
{
    ScheduleValidationResult ValidateSchedule(List<FlightScheduleEntry> proposedFlights);
}

public class FlightScheduleEntry
{
    public string FlightNumber { get; set; } = string.Empty;
    public TimeOnly DepartureTime { get; set; }
    public TimeOnly ArrivalTime { get; set; }  // Back at VIE
    public int FlightDurationMinutes { get; set; }
}

public class ScheduleValidationResult
{
    public bool IsValid { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}

public class ScheduleValidator : IScheduleValidator
{
    public ScheduleValidationResult ValidateSchedule(List<FlightScheduleEntry> proposedFlights)
    {
        // TODO: Implement schedule validation logic
        // 1. Check minimum turnaround time (60 minutes)
        // 2. Check night curfew (no departures 23:00-06:00)
        // 3. Verify chronological order
        // 4. Validate flight durations are positive
        
        throw new NotImplementedException("Students must implement this method");
    }
}

// ===== Booking Curve Analyzer =====

public interface IBookingCurveAnalyzer
{
    BookingCurveAnalysis AnalyzeBookingPattern(int flightId, List<BookingRecord> bookings);
}

public class BookingCurveAnalysis
{
    public int TotalBookings { get; set; }
    public decimal EarlyBirdPercentage { get; set; }  // Booked >14 days before flight
    public decimal LastMinutePercentage { get; set; } // Booked ≤7 days before flight
    public decimal AverageTicketPrice { get; set; }
    public decimal RevenueOptimizationScore { get; set; }  // 0.00 to 1.00
    public string Analysis { get; set; } = string.Empty;
}

public class BookingCurveAnalyzer : IBookingCurveAnalyzer
{
    private readonly ApplicationDataContext _db;

    public BookingCurveAnalyzer(ApplicationDataContext db)
    {
        _db = db;
    }

    public BookingCurveAnalysis AnalyzeBookingPattern(int flightId, List<BookingRecord> bookings)
    {
        // TODO: Implement booking curve analysis
        // 1. Categorize bookings as Early Bird (>14 days), Standard (8-14), Last Minute (≤7)
        // 2. Calculate percentages for each category
        // 3. Calculate revenue optimization score
        // 4. Generate analysis text
        
        throw new NotImplementedException("Students must implement this method");
    }
}
