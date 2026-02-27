using AppServices;
using Xunit;

namespace AppServicesTests;

public class ConfigurationOptimizerTests
{
    [Fact]
    public void RecommendConfiguration_LowDemand_Recommends220()
    {
        // Arrange
        // TODO: Students create test data with average demand of 180 PAX
        // TODO: Create ConfigurationOptimizer instance
        
        // Act
        // TODO: Call RecommendConfiguration
        
        // Assert
        // TODO: Verify recommendedCapacity is 220
        // TODO: Verify expectedLoadFactor is approximately 0.818 (180/220)
        
        throw new NotImplementedException("Students must implement this test");
    }

    [Fact]
    public void RecommendConfiguration_MediumDemand_Recommends220()
    {
        // Arrange
        // TODO: Students create test data with average demand of 210 PAX
        
        // Act
        // TODO: Call RecommendConfiguration
        
        // Assert
        // TODO: Verify recommendedCapacity is 220
        // TODO: Verify expectedLoadFactor is approximately 0.955 (210/220)
        
        throw new NotImplementedException("Students must implement this test");
    }

    [Fact]
    public void RecommendConfiguration_HighDemand_Recommends240()
    {
        // Arrange
        // TODO: Students create test data with average demand of 230 PAX
        
        // Act
        // TODO: Call RecommendConfiguration
        
        // Assert
        // TODO: Verify recommendedCapacity is 240
        // TODO: Verify expectedLoadFactor is approximately 0.958 (230/240)
        
        throw new NotImplementedException("Students must implement this test");
    }

    [Fact]
    public void RecommendConfiguration_NoHistoricalData_ReturnsDefault220()
    {
        // Arrange
        // TODO: Students create empty booking list
        
        // Act
        // TODO: Call RecommendConfiguration
        
        // Assert
        // TODO: Verify recommendedCapacity is 220 (conservative default)
        
        throw new NotImplementedException("Students must implement this test");
    }

    [Fact]
    public void RecommendConfiguration_HighVariance_IncludesWarningInReasoning()
    {
        // Arrange
        // TODO: Students create test data with high variance
        // Example: bookings with sold seats: 150, 180, 240, 160, 220 (std dev > 25% of mean)
        
        // Act
        // TODO: Call RecommendConfiguration
        
        // Assert
        // TODO: Verify reasoning contains "variance" or "inconsistent"
        
        throw new NotImplementedException("Students must implement this test");
    }
}
