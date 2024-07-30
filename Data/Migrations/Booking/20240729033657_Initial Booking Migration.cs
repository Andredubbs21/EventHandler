using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventHandler.Api.Data.Migrations.Booking
{
    /// <inheritdoc />
    public partial class InitialBookingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "Amount", "EventId", "Username" },
                values: new object[,]
                {
                    { 1, 2, 1, "andredubbs" },
                    { 2, 1, 2, "johndoe" },
                    { 3, 4, 3, "janedoe" },
                    { 4, 3, 4, "mikeross" },
                    { 5, 2, 5, "sarahconnor" },
                    { 6, 5, 6, "bobsmith" },
                    { 7, 1, 7, "alicesmith" },
                    { 8, 3, 8, "charliebrown" },
                    { 9, 2, 9, "davidjohnson" },
                    { 10, 4, 10, "emilywhite" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}
