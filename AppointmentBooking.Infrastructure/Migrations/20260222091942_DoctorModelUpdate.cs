using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DoctorModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Doctors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId",
                table: "Doctors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Doctors",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
