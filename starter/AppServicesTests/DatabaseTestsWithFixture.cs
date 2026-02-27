using AppServices;
using Microsoft.EntityFrameworkCore;
using TestInfrastructure;

namespace AppServicesTests;

/// <summary>
/// Example tests demonstrating database CRUD operations using the DatabaseFixture.
/// These tests use the in-memory SQLite database and verify the data model works correctly.
/// </summary>
public class DatabaseTestsWithClassFixture(DatabaseFixture fixture)
    : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task CanAddAndRetrieveFlight()
    {
        // Arrange & Act
        int flightId;
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var flight = new Flight 
            { 
                FlightNumber = "FR1234",
                Route = "VIE-LPA",
                FlightDate = new DateTime(2026, 2, 1),
                SeatCapacity = 220,
                SoldSeats = 0,
                BaseFare = 29.99m,
                TotalRevenue = 0m,
                TotalCost = 0m
            };
            context.Flights.Add(flight);
            await context.SaveChangesAsync();
            flightId = flight.Id;
        }

        // Assert
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var flight = await context.Flights.FindAsync(flightId);
            Assert.NotNull(flight);
            Assert.Equal("FR1234", flight.FlightNumber);
            Assert.Equal("VIE-LPA", flight.Route);
            Assert.Equal(220, flight.SeatCapacity);
        }
    }

    [Fact]
    public async Task CanAddBookingRecordWithFlight()
    {
        // Arrange & Act
        int bookingId;
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var flight = new Flight
            {
                FlightNumber = "FR5678",
                Route = "VIE-AGP",
                FlightDate = new DateTime(2026, 3, 1),
                SeatCapacity = 240,
                SoldSeats = 0,
                BaseFare = 34.99m,
                TotalRevenue = 0m,
                TotalCost = 0m
            };
            context.Flights.Add(flight);
            await context.SaveChangesAsync();

            var booking = new BookingRecord
            {
                FlightId = flight.Id,
                BookingDate = new DateTime(2026, 2, 15),
                TicketPrice = 34.99m,
                PassengerCount = 1
            };
            context.BookingRecords.Add(booking);
            await context.SaveChangesAsync();
            bookingId = booking.Id;
        }

        // Assert
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var booking = await context.BookingRecords
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            
            Assert.NotNull(booking);
            Assert.Equal(34.99m, booking.TicketPrice);
            Assert.Equal(1, booking.PassengerCount);
            Assert.NotNull(booking.Flight);
            Assert.Equal("FR5678", booking.Flight.FlightNumber);
        }
    }

    [Fact]
    public async Task CanUpdateFlightRevenue()
    {
        // Arrange
        int flightId;
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var flight = new Flight 
            { 
                FlightNumber = "FR9999",
                Route = "VIE-BCN",
                FlightDate = new DateTime(2026, 4, 1),
                SeatCapacity = 220,
                SoldSeats = 0,
                BaseFare = 29.99m,
                TotalRevenue = 0m,
                TotalCost = 0m
            };
            context.Flights.Add(flight);
            await context.SaveChangesAsync();
            flightId = flight.Id;
        }

        // Act - Simulate adding bookings and updating revenue
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var flight = await context.Flights.FindAsync(flightId);
            Assert.NotNull(flight);
            
            flight.SoldSeats = 150;
            flight.TotalRevenue = 6500.75m;
            await context.SaveChangesAsync();
        }

        // Assert
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var flight = await context.Flights.FindAsync(flightId);
            Assert.NotNull(flight);
            Assert.Equal(150, flight.SoldSeats);
            Assert.Equal(6500.75m, flight.TotalRevenue);
        }
    }

    [Fact]
    public async Task CanDeleteFlightCascadesBookings()
    {
        // Arrange
        int flightId;
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var flight = new Flight
            {
                FlightNumber = "FR0001",
                Route = "VIE-OPO",
                FlightDate = new DateTime(2026, 5, 1),
                SeatCapacity = 220,
                SoldSeats = 0,
                BaseFare = 29.99m,
                TotalRevenue = 0m,
                TotalCost = 0m
            };
            context.Flights.Add(flight);
            await context.SaveChangesAsync();
            flightId = flight.Id;

            // Add a booking
            var booking = new BookingRecord
            {
                FlightId = flight.Id,
                BookingDate = new DateTime(2026, 4, 15),
                TicketPrice = 29.99m,
                PassengerCount = 1
            };
            context.BookingRecords.Add(booking);
            await context.SaveChangesAsync();
        }

        // Act - Delete the flight
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var flight = await context.Flights.FindAsync(flightId);
            Assert.NotNull(flight);
            context.Flights.Remove(flight);
            await context.SaveChangesAsync();
        }

        // Assert - Flight and bookings should be deleted (cascade)
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var flight = await context.Flights.FindAsync(flightId);
            Assert.Null(flight);

            var bookings = await context.BookingRecords
                .Where(b => b.FlightId == flightId)
                .ToListAsync();
            Assert.Empty(bookings);
        }
    }

    [Fact]
    public async Task CanAddAndRetrieveRoute()
    {
        // Arrange & Act
        int routeId;
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var route = new Route
            {
                RouteCode = "VIE-LPA",
                Origin = "VIE",
                Destination = "LPA",
                TypicalDemand = 215,
                AverageFare = 67.50m
            };
            context.Routes.Add(route);
            await context.SaveChangesAsync();
            routeId = route.Id;
        }

        // Assert
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var route = await context.Routes.FindAsync(routeId);
            Assert.NotNull(route);
            Assert.Equal("VIE-LPA", route.RouteCode);
            Assert.Equal("VIE", route.Origin);
            Assert.Equal("LPA", route.Destination);
            Assert.Equal(215, route.TypicalDemand);
            Assert.Equal(67.50m, route.AverageFare);
        }
    }
}
