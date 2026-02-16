using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TimeSlotTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "TimeSlots");

            migrationBuilder.AddColumn<int>(
                name: "BookedCount",
                table: "TimeSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxPatients",
                table: "TimeSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookedCount",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "MaxPatients",
                table: "TimeSlots");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "TimeSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
