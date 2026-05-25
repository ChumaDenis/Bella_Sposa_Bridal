using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSizesToUK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "DressSizes";
                INSERT INTO "DressSizes" ("Id", "DressId", "Size")
                SELECT gen_random_uuid(), d."Id", s."Size"
                FROM "Dresses" d
                CROSS JOIN (
                    VALUES ('UK6'), ('UK8'), ('UK10'), ('UK12'), ('UK14'), ('UK16'), ('UK18'), ('UK20')
                ) AS s("Size");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "DressSizes";
                INSERT INTO "DressSizes" ("Id", "DressId", "Size")
                SELECT gen_random_uuid(), d."Id", s."Size"
                FROM "Dresses" d
                CROSS JOIN (VALUES ('XS'), ('S'), ('M'), ('L'), ('XL')) AS s("Size");
                """);
        }
    }
}
