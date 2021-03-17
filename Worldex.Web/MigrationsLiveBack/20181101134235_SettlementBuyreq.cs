using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class SettlementBuyreq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MakerPrice",
                table: "TradePoolQueue",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TakerDisc",
                table: "TradePoolQueue",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TakerLoss",
                table: "TradePoolQueue",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TakerPrice",
                table: "TradePoolQueue",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "TradeBuyRequest",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveredQty",
                table: "TradeBuyerList",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MakerPrice",
                table: "TradePoolQueue");

            migrationBuilder.DropColumn(
                name: "TakerDisc",
                table: "TradePoolQueue");

            migrationBuilder.DropColumn(
                name: "TakerLoss",
                table: "TradePoolQueue");

            migrationBuilder.DropColumn(
                name: "TakerPrice",
                table: "TradePoolQueue");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "TradeBuyRequest");

            migrationBuilder.DropColumn(
                name: "DeliveredQty",
                table: "TradeBuyerList");
        }
    }
}
