using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addNewColToThirdPartyArbitrage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Param4Regex",
                table: "ThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param5Regex",
                table: "ThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param6Regex",
                table: "ThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param7Regex",
                table: "ThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdateDate",
                table: "CryptoWatcherArbitrage",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "CryptoWatcherArbitrage",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "APITickerURL",
                table: "ArbitrageThirdPartyAPIConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TickerMethodType",
                table: "ArbitrageThirdPartyAPIConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TickerResponseBody",
                table: "ArbitrageThirdPartyAPIConfiguration",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Param4Regex",
                table: "ThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "Param5Regex",
                table: "ThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "Param6Regex",
                table: "ThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "Param7Regex",
                table: "ThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "CryptoWatcherArbitrage");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CryptoWatcherArbitrage");

            migrationBuilder.DropColumn(
                name: "APITickerURL",
                table: "ArbitrageThirdPartyAPIConfiguration");

            migrationBuilder.DropColumn(
                name: "TickerMethodType",
                table: "ArbitrageThirdPartyAPIConfiguration");

            migrationBuilder.DropColumn(
                name: "TickerResponseBody",
                table: "ArbitrageThirdPartyAPIConfiguration");
        }
    }
}
