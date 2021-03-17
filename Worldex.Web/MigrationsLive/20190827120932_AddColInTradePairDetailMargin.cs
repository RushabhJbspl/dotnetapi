using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddColInTradePairDetailMargin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmtLength",
                table: "TradePairDetailMargin",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriceLength",
                table: "TradePairDetailMargin",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QtyLength",
                table: "TradePairDetailMargin",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmtLength",
                table: "TradePairDetailMargin");

            migrationBuilder.DropColumn(
                name: "PriceLength",
                table: "TradePairDetailMargin");

            migrationBuilder.DropColumn(
                name: "QtyLength",
                table: "TradePairDetailMargin");
        }
    }
}
