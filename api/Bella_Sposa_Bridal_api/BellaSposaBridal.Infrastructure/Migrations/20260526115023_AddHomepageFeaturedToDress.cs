using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHomepageFeaturedToDress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HomepageFeaturedOrder",
                table: "Dresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsHomepageFeatured",
                table: "Dresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomepageFeaturedOrder",
                table: "Dresses");

            migrationBuilder.DropColumn(
                name: "IsHomepageFeatured",
                table: "Dresses");
        }
    }
}
