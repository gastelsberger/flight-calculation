namespace Importer;

/// <summary>
/// Result of command line parsing
/// </summary>
public record CommandLineArgs(string Command, string CsvFilePath, bool IsDryRun);

/// <summary>
/// Parser for command line arguments
/// </summary>
public class CommandLineParser
{
    public CommandLineArgs Parse(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException(
                "Please provide a command and CSV file path.\n" +
                "Usage:\n" +
                "  Importer import-bookings --file <csv-file-path> [--dry-run]\n" +
                "  Importer <csv-file-path> [--dry-run]  (legacy format)");
        }

        // Check for new command format: import-bookings --file <path>
        if (args[0] == "import-bookings")
        {
            var fileIndex = Array.IndexOf(args, "--file");
            if (fileIndex == -1 || fileIndex + 1 >= args.Length)
            {
                throw new ArgumentException("Missing --file argument. Usage: import-bookings --file <csv-file-path>");
            }

            var csvFilePath = args[fileIndex + 1];
            var isDryRun = args.Any(arg => arg == "--dry-run");

            return new CommandLineArgs("import-bookings", csvFilePath, isDryRun);
        }

        // Legacy format: Importer <csv-file-path> [--dry-run]
        var legacyCsvFilePath = args[0];
        var legacyIsDryRun = args.Any(arg => arg == "--dry-run");

        return new CommandLineArgs("import-legacy", legacyCsvFilePath, legacyIsDryRun);
    }
}
