using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutReachToursAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PaidAmountKES",
                table: "HotelBookings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                table: "HotelBookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaystackReference",
                table: "HotelBookings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmountKES",
                table: "HotelBookings");

            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                table: "HotelBookings");

            migrationBuilder.DropColumn(
                name: "PaystackReference",
                table: "HotelBookings");
        }
    }
}
