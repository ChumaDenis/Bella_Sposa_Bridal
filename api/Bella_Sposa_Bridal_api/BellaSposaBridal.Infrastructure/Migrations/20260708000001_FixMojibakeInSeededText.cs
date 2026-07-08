using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using BellaSposaBridal.Infrastructure.Persistence;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260708000001_FixMojibakeInSeededText")]
    public partial class FixMojibakeInSeededText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seeded text contained double-encoded UTF-8 (mojibake):
            //   a-circumflex + euro + left-quote  -> en dash (U+2013)
            //   a-circumflex + euro + right-quote -> em dash (U+2014)
            //   A-tilde + copyright               -> e-acute (U+00E9)
            // chr() keeps this file pure ASCII so no editor/compiler can re-corrupt it.
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION pg_temp.fix_mojibake(t text) RETURNS text AS $$
                    SELECT replace(replace(replace(t,
                        chr(226) || chr(8364) || chr(8220), chr(8211)),
                        chr(226) || chr(8364) || chr(8221), chr(8212)),
                        chr(195) || chr(169),               chr(233));
                $$ LANGUAGE sql;

                UPDATE "AtlierInfos" SET
                    "WorkingHours" = pg_temp.fix_mojibake("WorkingHours"),
                    "UpdatedAt"    = NOW();

                UPDATE "Dresses" SET
                    "Description"       = pg_temp.fix_mojibake("Description"),
                    "Decoration"        = pg_temp.fix_mojibake("Decoration"),
                    "SleeveDescription" = pg_temp.fix_mojibake("SleeveDescription");

                UPDATE "Collections" SET
                    "Description" = pg_temp.fix_mojibake("Description");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
