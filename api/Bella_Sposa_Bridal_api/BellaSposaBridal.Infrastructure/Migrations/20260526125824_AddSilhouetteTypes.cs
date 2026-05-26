using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSilhouetteTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SilhouetteTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SilhouetteTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SilhouetteTypes",
                columns: new[] { "Id", "Name", "DisplayOrder" },
                values: new object[,]
                {
                    { 0, "Mermaid",    0 },
                    { 1, "Ball Gown",  1 },
                    { 2, "A-Line",     2 },
                    { 3, "Sheath",     3 },
                    { 4, "Empire",     4 },
                    { 5, "Trumpet",    5 },
                    { 6, "Tea Length", 6 },
                    { 7, "Mini",       7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dresses_Silhouette",
                table: "Dresses",
                column: "Silhouette");

            migrationBuilder.AddForeignKey(
                name: "FK_Dresses_SilhouetteTypes_Silhouette",
                table: "Dresses",
                column: "Silhouette",
                principalTable: "SilhouetteTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dresses_SilhouetteTypes_Silhouette",
                table: "Dresses");

            migrationBuilder.DropTable(
                name: "SilhouetteTypes");

            migrationBuilder.DropIndex(
                name: "IX_Dresses_Silhouette",
                table: "Dresses");
        }
    }
}
