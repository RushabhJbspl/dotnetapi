using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class MarketCapWalletStatasticColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMaker",
                table: "TrnChargeLog");

            migrationBuilder.AddColumn<DateTime>(
                name: "USDLastUpdateDateTime",
                table: "StatasticsDetail",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "USDAmount",
                table: "Statastics",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume_24h",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_7d",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_24h",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_1h",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Market_cap",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "USDLastUpdateDateTime",
                table: "StatasticsDetail");

            migrationBuilder.DropColumn(
                name: "USDAmount",
                table: "Statastics");

            migrationBuilder.AddColumn<short>(
                name: "IsMaker",
                table: "TrnChargeLog",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume_24h",
                table: "CurrencyRateDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CurrencyRateDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_7d",
                table: "CurrencyRateDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_24h",
                table: "CurrencyRateDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_1h",
                table: "CurrencyRateDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Market_cap",
                table: "CurrencyRateDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
