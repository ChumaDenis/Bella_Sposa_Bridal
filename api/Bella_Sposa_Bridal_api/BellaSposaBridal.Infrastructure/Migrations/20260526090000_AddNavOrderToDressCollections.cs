using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using BellaSposaBridal.Infrastructure.Persistence;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260526090000_AddNavOrderToDressCollections")]
    public partial class AddNavOrderToDressCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NavOrder",
                table: "DressCollections",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NavOrder",
                table: "DressCollections");
        }
    }
}
