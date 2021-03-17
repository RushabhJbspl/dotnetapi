using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ArbitrageThirdpartyCancellationUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCheckUrl",
                table: "RouteConfigurationArbitrage");

            migrationBuilder.DropColumn(
                name: "TransactionUrl",
                table: "RouteConfigurationArbitrage");

            migrationBuilder.DropColumn(
                name: "ValidationUrl",
                table: "RouteConfigurationArbitrage");

            migrationBuilder.AddColumn<string>(
                name: "APICancellationURL",
                table: "ArbitrageThirdPartyAPIConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationMethodType",
                table: "ArbitrageThirdPartyAPIConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationRequestBody",
                table: "ArbitrageThirdPartyAPIConfiguration",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APICancellationURL",
                table: "ArbitrageThirdPartyAPIConfiguration");

            migrationBuilder.DropColumn(
                name: "CancellationMethodType",
                table: "ArbitrageThirdPartyAPIConfiguration");

            migrationBuilder.DropColumn(
                name: "CancellationRequestBody",
                table: "ArbitrageThirdPartyAPIConfiguration");

            migrationBuilder.AddColumn<string>(
                name: "StatusCheckUrl",
                table: "RouteConfigurationArbitrage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionUrl",
                table: "RouteConfigurationArbitrage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidationUrl",
                table: "RouteConfigurationArbitrage",
                nullable: true);
        }
    }
}
