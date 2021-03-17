using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class NewColumnInThirpartyArbitrage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValidateMethodType",
                table: "ArbitrageThirdPartyAPIConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidateRequestBody",
                table: "ArbitrageThirdPartyAPIConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppTypeID",
                table: "AppType",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidateMethodType",
                table: "ArbitrageThirdPartyAPIConfiguration");

            migrationBuilder.DropColumn(
                name: "ValidateRequestBody",
                table: "ArbitrageThirdPartyAPIConfiguration");

            migrationBuilder.DropColumn(
                name: "AppTypeID",
                table: "AppType");
        }
    }
}
