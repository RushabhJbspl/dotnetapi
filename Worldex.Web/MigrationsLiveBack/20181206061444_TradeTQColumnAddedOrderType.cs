using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class TradeTQColumnAddedOrderType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TakerPer",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "TrnRefNo",
                table: "TradeTransactionQueue");

            migrationBuilder.AddColumn<int>(
                name: "WalletDeductionType",
                table: "WalletTransactionQueues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ordertype",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WalletDeductionType",
                table: "WalletTransactionQueues");

            migrationBuilder.DropColumn(
                name: "ordertype",
                table: "TradeTransactionQueue");

            migrationBuilder.AddColumn<decimal>(
                name: "TakerPer",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "TrnRefNo",
                table: "TradeTransactionQueue",
                nullable: true);
        }
    }
}
