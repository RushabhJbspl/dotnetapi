using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddColsInTradePairDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmtLength",
                table: "TradePairDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriceLength",
                table: "TradePairDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QtyLength",
                table: "TradePairDetail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmtLength",
                table: "TradePairDetail");

            migrationBuilder.DropColumn(
                name: "PriceLength",
                table: "TradePairDetail");

            migrationBuilder.DropColumn(
                name: "QtyLength",
                table: "TradePairDetail");
        }
    }
}
