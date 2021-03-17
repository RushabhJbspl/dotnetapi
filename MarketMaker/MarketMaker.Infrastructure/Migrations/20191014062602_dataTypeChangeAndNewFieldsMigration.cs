using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketMaker.Infrastructure.Migrations
{
    public partial class dataTypeChangeAndNewFieldsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BuyHoldOrderRateChange",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SellHoldOrderRateChange",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyHoldOrderRateChange",
                table: "MarketMakerPreferences");

            migrationBuilder.DropColumn(
                name: "SellHoldOrderRateChange",
                table: "MarketMakerPreferences");
        }
    }
}
