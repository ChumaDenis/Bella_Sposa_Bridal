using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturedToCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Collections",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FeaturedOrder",
                table: "Collections",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsFeatured",    table: "Collections");
            migrationBuilder.DropColumn(name: "FeaturedOrder", table: "Collections");
        }
    }
}
