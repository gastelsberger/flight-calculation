using Importer;

namespace ImporterTests;

public class CommandLineParserTests
{
    private readonly CommandLineParser parser = new();

    [Fact]
    public void Parse_ImportBookingsCommand_ReturnsCorrectResult()
    {
        // Arrange
        var args = new[] { "import-bookings", "--file", "test.csv" };

        // Act
        var result = parser.Parse(args);

        // Assert
        Assert.Equal("import-bookings", result.Command);
        Assert.Equal("test.csv", result.CsvFilePath);
        Assert.False(result.IsDryRun);
    }

    [Fact]
    public void Parse_ImportBookingsWithDryRun_ReturnsDryRunTrue()
    {
        // Arrange
        var args = new[] { "import-bookings", "--file", "test.csv", "--dry-run" };

        // Act
        var result = parser.Parse(args);

        // Assert
        Assert.Equal("import-bookings", result.Command);
        Assert.Equal("test.csv", result.CsvFilePath);
        Assert.True(result.IsDryRun);
    }

    [Fact]
    public void Parse_LegacyFormat_ReturnsLegacyCommand()
    {
        // Arrange
        var args = new[] { "test.csv" };

        // Act
        var result = parser.Parse(args);

        // Assert
        Assert.Equal("import-legacy", result.Command);
        Assert.Equal("test.csv", result.CsvFilePath);
        Assert.False(result.IsDryRun);
    }

    [Fact]
    public void Parse_LegacyFormatWithDryRun_ReturnsDryRunTrue()
    {
        // Arrange
        var args = new[] { "test.csv", "--dry-run" };

        // Act
        var result = parser.Parse(args);

        // Assert
        Assert.Equal("import-legacy", result.Command);
        Assert.Equal("test.csv", result.CsvFilePath);
        Assert.True(result.IsDryRun);
    }

    [Fact]
    public void Parse_NoArguments_ThrowsArgumentException()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.Parse(args));
        Assert.Contains("command", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_ImportBookingsMissingFileArg_ThrowsException()
    {
        // Arrange
        var args = new[] { "import-bookings" };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.Parse(args));
        Assert.Contains("--file", exception.Message);
    }
}
