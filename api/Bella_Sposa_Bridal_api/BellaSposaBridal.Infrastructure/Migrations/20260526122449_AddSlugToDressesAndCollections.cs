using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToDressesAndCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Dresses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Collections",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Backfill: generate slugs from existing names
            migrationBuilder.Sql(@"
                UPDATE ""Dresses""
                SET ""Slug"" = TRIM('-' FROM regexp_replace(
                    regexp_replace(
                        lower(trim(""Name"")),
                        '[^a-z0-9\s-]', '', 'g'),
                    '\s+', '-', 'g'));
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Collections""
                SET ""Slug"" = TRIM('-' FROM regexp_replace(
                    regexp_replace(
                        lower(trim(""Name"")),
                        '[^a-z0-9\s-]', '', 'g'),
                    '\s+', '-', 'g'));
            ");

            // Deduplicate dress slugs by appending ID suffix for 2nd+ duplicates
            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT ""Id"", ROW_NUMBER() OVER (PARTITION BY ""Slug"" ORDER BY ""CreatedAt"") AS rn
                    FROM ""Dresses""
                )
                UPDATE ""Dresses"" d
                SET ""Slug"" = d.""Slug"" || '-' || SUBSTRING(CAST(d.""Id"" AS text), 1, 8)
                FROM ranked r
                WHERE d.""Id"" = r.""Id"" AND r.rn > 1;
            ");

            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT ""Id"", ROW_NUMBER() OVER (PARTITION BY ""Slug"" ORDER BY ""CreatedAt"") AS rn
                    FROM ""Collections""
                )
                UPDATE ""Collections"" c
                SET ""Slug"" = c.""Slug"" || '-' || SUBSTRING(CAST(c.""Id"" AS text), 1, 8)
                FROM ranked r
                WHERE c.""Id"" = r.""Id"" AND r.rn > 1;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Dresses_Slug",
                table: "Dresses",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Collections_Slug",
                table: "Collections",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Dresses_Slug", table: "Dresses");
            migrationBuilder.DropIndex(name: "IX_Collections_Slug", table: "Collections");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Dresses");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Collections");
        }
    }
}
