using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketMaker.Infrastructure.Migrations
{
    public partial class MarketMakerPreferenceRemoveFieldsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyDownThreshold",
                table: "MarketMakerPreferences");

            migrationBuilder.DropColumn(
                name: "BuyUpThreshold",
                table: "MarketMakerPreferences");

            migrationBuilder.DropColumn(
                name: "SellDownThreshold",
                table: "MarketMakerPreferences");

            migrationBuilder.DropColumn(
                name: "SellUpThreshold",
                table: "MarketMakerPreferences");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyThreshold",
                table: "MarketMakerPreferences",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SellThreshold",
                table: "MarketMakerPreferences",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyThreshold",
                table: "MarketMakerPreferences");

            migrationBuilder.DropColumn(
                name: "SellThreshold",
                table: "MarketMakerPreferences");

            migrationBuilder.AddColumn<int>(
                name: "BuyDownThreshold",
                table: "MarketMakerPreferences",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BuyUpThreshold",
                table: "MarketMakerPreferences",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SellDownThreshold",
                table: "MarketMakerPreferences",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SellUpThreshold",
                table: "MarketMakerPreferences",
                nullable: false,
                defaultValue: 0);
        }
    }
}
