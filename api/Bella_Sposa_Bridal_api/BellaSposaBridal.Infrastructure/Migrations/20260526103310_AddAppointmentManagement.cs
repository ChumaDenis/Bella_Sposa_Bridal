using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentTypeConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentTypeConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DaySchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    CustomSlots = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaySchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlotConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Time = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlotConfigs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DaySchedules_Date",
                table: "DaySchedules",
                column: "Date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentTypeConfigs");

            migrationBuilder.DropTable(
                name: "DaySchedules");

            migrationBuilder.DropTable(
                name: "TimeSlotConfigs");
        }
    }
}
