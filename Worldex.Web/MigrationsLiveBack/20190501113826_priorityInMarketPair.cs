using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class priorityInMarketPair : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Priority",
                table: "TradePairMasterMargin",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Priority",
                table: "TradePairMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Priority",
                table: "MarketMargin",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Priority",
                table: "Market",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TradePairMasterMargin");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TradePairMaster");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MarketMargin");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Market");
        }
    }
}
