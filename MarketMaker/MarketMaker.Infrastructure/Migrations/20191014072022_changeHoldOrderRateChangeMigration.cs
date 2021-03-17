using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketMaker.Infrastructure.Migrations
{
    public partial class changeHoldOrderRateChangeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyHoldOrderRateChange",
                table: "MarketMakerPreferences");

            migrationBuilder.RenameColumn(
                name: "SellHoldOrderRateChange",
                table: "MarketMakerPreferences",
                newName: "HoldOrderRateChange");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HoldOrderRateChange",
                table: "MarketMakerPreferences",
                newName: "SellHoldOrderRateChange");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyHoldOrderRateChange",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
