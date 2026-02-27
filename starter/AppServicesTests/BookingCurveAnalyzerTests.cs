using AppServices;
using Microsoft.EntityFrameworkCore;
using TestInfrastructure;
using Xunit;

namespace AppServicesTests;

public class BookingCurveAnalyzerTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{

    [Fact]
    public void AnalyzeBookingPattern_OptimalDistribution_HighScore()
    {
        // Arrange
        // TODO: Students create test flight and bookings with optimal distribution:
        // - 30% early bird (>14 days before flight)
        // - 45% standard (8-14 days)
        // - 25% last minute (≤7 days)
        
        // Act
        // TODO: Create analyzer and call AnalyzeBookingPattern
        
        // Assert
        // TODO: Verify revenueOptimizationScore is 1.00 (perfect)
        // TODO: Verify percentages match expected distribution
        
        throw new NotImplementedException("Students must implement this test");
    }

    [Fact]
    public void AnalyzeBookingPattern_SkewedDistribution_LowerScore()
    {
        // Arrange
        // TODO: Students create test flight with skewed distribution:
        // - 60% early bird
        // - 30% standard
        // - 10% last minute
        
        // Act
        // TODO: Create analyzer and call AnalyzeBookingPattern
        
        // Assert
        // TODO: Verify revenueOptimizationScore is < 0.85
        // TODO: Verify analysis text mentions the imbalance
        
        throw new NotImplementedException("Students must implement this test");
    }

    [Fact]
    public void AnalyzeBookingPattern_CalculatesAverageTicketPrice()
    {
        // Arrange
        // TODO: Students create bookings with known ticket prices
        // Example: 3 bookings at 30€, 2 bookings at 50€, 1 booking at 100€
        // Expected average: (3*30 + 2*50 + 1*100) / 6 = 50€
        
        // Act
        // TODO: Create analyzer and call AnalyzeBookingPattern
        
        // Assert
        // TODO: Verify averageTicketPrice matches expected weighted average
        
        throw new NotImplementedException("Students must implement this test");
    }
}
