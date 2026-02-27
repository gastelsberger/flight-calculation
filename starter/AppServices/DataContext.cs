using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AppServices;

public partial class ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : DbContext(options)
{
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<BookingRecord> BookingRecords => Set<BookingRecord>();
    public DbSet<Route> Routes => Set<Route>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Flight entity
        modelBuilder.Entity<Flight>(entity =>
        {
            entity.Property(e => e.FlightNumber).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Route).IsRequired().HasMaxLength(20);
            entity.Property(e => e.BaseFare).HasConversion<double>().HasColumnType("REAL");
            entity.Property(e => e.TotalRevenue).HasConversion<double>().HasColumnType("REAL");
            entity.Property(e => e.TotalCost).HasConversion<double>().HasColumnType("REAL");
            
            entity.HasMany(f => f.Bookings)
                .WithOne(b => b.Flight)
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure BookingRecord entity
        modelBuilder.Entity<BookingRecord>(entity =>
        {
            entity.Property(e => e.TicketPrice).HasConversion<double>().HasColumnType("REAL");
        });

        // Configure Route entity
        modelBuilder.Entity<Route>(entity =>
        {
            entity.Property(e => e.RouteCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Origin).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Destination).IsRequired().HasMaxLength(3);
            entity.Property(e => e.AverageFare).HasConversion<double>().HasColumnType("REAL");
            
            entity.HasIndex(e => e.RouteCode).IsUnique();
        });
    }
}

public class ApplicationDataContextFactory : IDesignTimeDbContextFactory<ApplicationDataContext>
{
    public ApplicationDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var path = configuration["Database:path"] ?? throw new InvalidOperationException("Database path not configured.");
        var fileName = configuration["Database:fileName"] ?? throw new InvalidOperationException("Database file name not configured.");
        optionsBuilder.UseSqlite($"Data Source={path}/{fileName}");

        return new ApplicationDataContext(optionsBuilder.Options);
    }
}