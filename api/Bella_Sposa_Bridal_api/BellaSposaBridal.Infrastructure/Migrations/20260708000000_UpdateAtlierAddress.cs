using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using BellaSposaBridal.Infrastructure.Persistence;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260708000000_UpdateAtlierAddress")]
    public partial class UpdateAtlierAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "AtlierInfos" SET
                    "Address"   = '29 Queenstown Road, London SW8 3RE',
                    "UpdatedAt" = NOW()
                WHERE 1=1;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
