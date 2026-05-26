using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMainDescriptionToAppointmentTypeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MainDescription",
                table: "AppointmentTypeConfigs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainDescription",
                table: "AppointmentTypeConfigs");
        }
    }
}
