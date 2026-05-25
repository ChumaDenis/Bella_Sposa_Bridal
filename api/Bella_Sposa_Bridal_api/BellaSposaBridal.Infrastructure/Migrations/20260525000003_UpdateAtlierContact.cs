using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BellaSposaBridal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAtlierContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "AtlierInfos" SET
                    "Address"      = '127 Queenstown Road, London SW8 3RH',
                    "Phone"        = '07466728196',
                    "WhatsApp"     = '07466728196',
                    "WorkingHours" = 'Mon – Sun: 10:00 – 19:00',
                    "MaxGuests"    = 1,
                    "UpdatedAt"    = NOW()
                WHERE 1=1;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
