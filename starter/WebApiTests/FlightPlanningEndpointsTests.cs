using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace WebApiTests;

public class FlightPlanningEndpointsTests : IClassFixture<WebApiTestFixture>
{
    private readonly HttpClient _client;

    public FlightPlanningEndpointsTests(WebApiTestFixture fixture)
    {
        _client = fixture.HttpClient;
    }

    [Fact]
    public async Task ConfigurationRecommend_ValidRoute_Returns200()
    {
        // Arrange
        // TODO: Students seed database with test route and bookings
        var request = new { RouteCode = "VIE-LPA" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/configuration/recommend", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // TODO: Verify response body contains expected fields
        // (recommendedCapacity, expectedLoadFactor, reasoning)
        
        throw new NotImplementedException("Students must complete this test");
    }

    [Fact]
    public async Task ConfigurationRecommend_InvalidRoute_Returns404()
    {
        // Arrange
        var request = new { RouteCode = "XXX-YYY" };  // Non-existent route

        // Act
        var response = await _client.PostAsJsonAsync("/api/configuration/recommend", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ScheduleValidate_ValidSchedule_Returns200WithIsValidTrue()
    {
        // Arrange
        // TODO: Students create valid schedule request
        var request = new
        {
            Flights = new[]
            {
                new { FlightNumber = "FR1234", DepartureTime = "07:00", ArrivalTime = "10:30", FlightDurationMinutes = 210 },
                new { FlightNumber = "FR1235", DepartureTime = "12:00", ArrivalTime = "15:30", FlightDurationMinutes = 210 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/schedule/validate", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // TODO: Verify response.IsValid is true
        // TODO: Verify ValidationErrors is empty
        
        throw new NotImplementedException("Students must complete this test");
    }

    [Fact]
    public async Task ScheduleValidate_InvalidSchedule_Returns200WithErrors()
    {
        // Arrange
        // TODO: Students create invalid schedule (e.g., insufficient turnaround)
        
        // Act
        // TODO: Call endpoint
        
        // Assert
        // TODO: Verify status is 200 OK but IsValid is false
        // TODO: Verify ValidationErrors contains appropriate messages
        
        throw new NotImplementedException("Students must implement this test");
    }

    [Fact]
    public async Task BookingAnalyze_ExistingFlight_Returns200()
    {
        // Arrange
        // TODO: Students seed database with test flight and bookings
        
        // Act
        var response = await _client.GetAsync("/api/bookings/analyze/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // TODO: Verify response contains booking analysis fields
        
        throw new NotImplementedException("Students must complete this test");
    }

    [Fact]
    public async Task BookingAnalyze_NonexistentFlight_Returns404()
    {
        // Arrange
        var nonExistentFlightId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/bookings/analyze/{nonExistentFlightId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RoutesCompare_ReturnsAllRoutesSortedByProfit()
    {
        // Arrange
        // TODO: Students seed database with multiple routes and flights
        
        // Act
        var response = await _client.GetAsync("/api/routes/compare");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // TODO: Verify routes are sorted by averageProfit descending
        // TODO: Verify each route has correct aggregated values
        
        throw new NotImplementedException("Students must complete this test");
    }
}
