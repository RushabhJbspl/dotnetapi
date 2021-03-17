using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketMaker.Infrastructure.Migrations
{
    public partial class ChangeInMarketMakerPreferenceMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RangeType",
                table: "MarketMakerPreferences");

            migrationBuilder.AddColumn<int>(
                name: "BuyLTPRangeType",
                table: "MarketMakerPreferences",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SellLTPRangeType",
                table: "MarketMakerPreferences",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyLTPRangeType",
                table: "MarketMakerPreferences");

            migrationBuilder.DropColumn(
                name: "SellLTPRangeType",
                table: "MarketMakerPreferences");

            migrationBuilder.AddColumn<int>(
                name: "RangeType",
                table: "MarketMakerPreferences",
                nullable: false,
                defaultValue: 0);
        }
    }
}
