using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutReachToursAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelNotificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HotelContactEmail",
                table: "HotelBookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HotelNotifiedAt",
                table: "HotelBookings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelContactEmail",
                table: "HotelBookings");

            migrationBuilder.DropColumn(
                name: "HotelNotifiedAt",
                table: "HotelBookings");
        }
    }
}
