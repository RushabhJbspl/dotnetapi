using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddNewChargeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TradingChargeType",
                table: "WalletTransactionQueues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "TradingChargeTypeMaster",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddColumn<string>(
                name: "DeductCurrency",
                table: "TradingChargeTypeMaster",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "TradingChargeTypeMaster",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "IsChargeFreeMarketEnabled",
                table: "TradingChargeTypeMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsCommonCurrencyDeductEnable",
                table: "TradingChargeTypeMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsDeductChargeMarketCurrency",
                table: "TradingChargeTypeMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "ChargeFreeMarketCurrencyMaster",
                columns: table => new
                {
                    MarketCurrency = table.Column<string>(maxLength: 7, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeFreeMarketCurrencyMaster", x => x.MarketCurrency);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargeFreeMarketCurrencyMaster");

            migrationBuilder.DropColumn(
                name: "TradingChargeType",
                table: "WalletTransactionQueues");

            migrationBuilder.DropColumn(
                name: "DeductCurrency",
                table: "TradingChargeTypeMaster");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "TradingChargeTypeMaster");

            migrationBuilder.DropColumn(
                name: "IsChargeFreeMarketEnabled",
                table: "TradingChargeTypeMaster");

            migrationBuilder.DropColumn(
                name: "IsCommonCurrencyDeductEnable",
                table: "TradingChargeTypeMaster");

            migrationBuilder.DropColumn(
                name: "IsDeductChargeMarketCurrency",
                table: "TradingChargeTypeMaster");

            migrationBuilder.AlterColumn<short>(
                name: "Type",
                table: "TradingChargeTypeMaster",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
