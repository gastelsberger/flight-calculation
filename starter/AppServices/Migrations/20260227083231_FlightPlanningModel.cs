using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppServices.Migrations
{
    /// <inheritdoc />
    public partial class FlightPlanningModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dummies");

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlightNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Route = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FlightDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SeatCapacity = table.Column<int>(type: "INTEGER", nullable: false),
                    SoldSeats = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseFare = table.Column<double>(type: "REAL", nullable: false),
                    TotalRevenue = table.Column<double>(type: "REAL", nullable: false),
                    TotalCost = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RouteCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Origin = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Destination = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    TypicalDemand = table.Column<int>(type: "INTEGER", nullable: false),
                    AverageFare = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlightId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TicketPrice = table.Column<double>(type: "REAL", nullable: false),
                    PassengerCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingRecords_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecords_FlightId",
                table: "BookingRecords",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RouteCode",
                table: "Routes",
                column: "RouteCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingRecords");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.CreateTable(
                name: "Dummies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DecimalProperty = table.Column<double>(type: "REAL", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dummies", x => x.Id);
                });
        }
    }
}
