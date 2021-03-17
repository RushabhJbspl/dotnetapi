using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class UpdateColNameToThirdPartyArbitrage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TickerResponseBody",
                table: "ArbitrageThirdPartyAPIConfiguration",
                newName: "TickerRequestBody");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TickerRequestBody",
                table: "ArbitrageThirdPartyAPIConfiguration",
                newName: "TickerResponseBody");
        }
    }
}
