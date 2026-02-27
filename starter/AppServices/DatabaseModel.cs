namespace AppServices;

// Entity representing a flight operation
public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;  // e.g., "FR1234"
    public string Route { get; set; } = string.Empty;          // e.g., "VIE-LPA"
    public DateTime FlightDate { get; set; }
    public int SeatCapacity { get; set; }     // 220 or 240
    public int SoldSeats { get; set; }
    public decimal BaseFare { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }    // From turnaround calculation
    
    // Navigation property
    public List<BookingRecord> Bookings { get; set; } = new();
}

// Entity representing individual ticket purchases
public class BookingRecord
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public DateTime BookingDate { get; set; }  // When the ticket was purchased
    public decimal TicketPrice { get; set; }
    public int PassengerCount { get; set; }    // Usually 1, but can be group bookings
    
    // Navigation property
    public Flight Flight { get; set; } = null!;
}

// Entity representing route metadata
public class Route
{
    public int Id { get; set; }
    public string RouteCode { get; set; } = string.Empty;      // e.g., "VIE-LPA"
    public string Origin { get; set; } = string.Empty;         // e.g., "VIE"
    public string Destination { get; set; } = string.Empty;    // e.g., "LPA"
    public int TypicalDemand { get; set; }     // Historical average PAX (0-300)
    public decimal AverageFare { get; set; }   // Historical average ticket price
}
