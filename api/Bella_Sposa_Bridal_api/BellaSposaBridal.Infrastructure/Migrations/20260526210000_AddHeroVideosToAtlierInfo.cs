using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHeroVideosToAtlierInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeroVideoDesktop",
                table: "AtlierInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroVideoMobile",
                table: "AtlierInfos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeroVideoDesktop",
                table: "AtlierInfos");

            migrationBuilder.DropColumn(
                name: "HeroVideoMobile",
                table: "AtlierInfos");
        }
    }
}
