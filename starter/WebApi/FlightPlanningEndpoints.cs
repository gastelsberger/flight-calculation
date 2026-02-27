using AppServices;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public static class FlightPlanningEndpoints
{
    public static IEndpointRouteBuilder MapFlightPlanningEndpoints(this IEndpointRouteBuilder app)
    {
        // ===== Configuration Recommendation Endpoint =====
        app.MapPost("/api/configuration/recommend", RecommendConfiguration)
            .Produces<ConfigurationRecommendationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDescription("Get aircraft configuration recommendation for a route");

        // ===== Schedule Validation Endpoint =====
        app.MapPost("/api/schedule/validate", ValidateSchedule)
            .Produces<ScheduleValidationResultDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDescription("Validate a proposed flight schedule");

        // ===== Booking Analysis Endpoint =====
        app.MapGet("/api/bookings/analyze/{flightId:int}", AnalyzeBookings)
            .Produces<BookingCurveAnalysisDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent)
            .WithDescription("Analyze booking patterns for a flight");

        // ===== Route Comparison Endpoint =====
        app.MapGet("/api/routes/compare", CompareRoutes)
            .Produces<List<RouteComparisonDto>>(StatusCodes.Status200OK)
            .WithDescription("Compare profitability across all routes");

        return app;
    }

    // ===== Endpoint Implementations (Students must complete these) =====

    private static async Task<IResult> RecommendConfiguration(
        ApplicationDataContext db,
        IConfigurationOptimizer optimizer,
        ConfigurationRequestDto request)
    {
        // TODO: Students implement this
        // 1. Validate request.RouteCode (not empty, valid format)
        // 2. Query historical bookings for this route from database
        // 3. Call optimizer.RecommendConfiguration()
        // 4. Return 404 if route not found, 200 with recommendation otherwise
        
        throw new NotImplementedException("Students must implement this endpoint");
    }

    private static IResult ValidateSchedule(
        IScheduleValidator validator,
        ScheduleValidationRequestDto request)
    {
        // TODO: Students implement this
        // 1. Validate request has flights
        // 2. Convert request DTOs to domain models
        // 3. Call validator.ValidateSchedule()
        // 4. Return validation result
        
        throw new NotImplementedException("Students must implement this endpoint");
    }

    private static async Task<IResult> AnalyzeBookings(
        ApplicationDataContext db,
        IBookingCurveAnalyzer analyzer,
        int flightId)
    {
        // TODO: Students implement this
        // 1. Check if flight exists (return 404 if not)
        // 2. Get all bookings for this flight
        // 3. Return 204 if no bookings found
        // 4. Call analyzer.AnalyzeBookingPattern()
        // 5. Return analysis result
        
        throw new NotImplementedException("Students must implement this endpoint");
    }

    private static async Task<IResult> CompareRoutes(
        ApplicationDataContext db,
        IConfigurationOptimizer optimizer)
    {
        // TODO: Students implement this
        // 1. Group all flights by Route
        // 2. For each route, calculate:
        //    - Total flights
        //    - Average load factor
        //    - Average revenue, cost, profit
        //    - Get configuration recommendation
        // 3. Sort by average profit (descending)
        // 4. Return list of route comparisons
        
        throw new NotImplementedException("Students must implement this endpoint");
    }
}

// ===== DTOs (Data Transfer Objects) =====

public record ConfigurationRequestDto(string RouteCode);

public record ConfigurationRecommendationDto(
    int RecommendedCapacity,
    decimal ExpectedLoadFactor,
    string Reasoning
);

public record ScheduleValidationRequestDto(List<FlightScheduleEntryDto> Flights);

public record FlightScheduleEntryDto(
    string FlightNumber,
    string DepartureTime,  // Format: "HH:mm"
    string ArrivalTime,    // Format: "HH:mm"
    int FlightDurationMinutes
);

public record ScheduleValidationResultDto(
    bool IsValid,
    List<string> ValidationErrors
);

public record BookingCurveAnalysisDto(
    int TotalBookings,
    decimal EarlyBirdPercentage,
    decimal LastMinutePercentage,
    decimal AverageTicketPrice,
    decimal RevenueOptimizationScore,
    string Analysis
);

public record RouteComparisonDto(
    string RouteCode,
    int TotalFlights,
    decimal AverageLoadFactor,
    decimal AverageRevenue,
    decimal AverageCost,
    decimal AverageProfit,
    int RecommendedConfiguration
);
