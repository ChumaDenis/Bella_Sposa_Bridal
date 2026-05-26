using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToDressesAndCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Dresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Dresses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Collections",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Collections",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Dresses");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Dresses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Collections");
        }
    }
}
