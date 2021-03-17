using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class MinMaxNotional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxNotional",
                table: "TradePairDetailMargin",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinNotional",
                table: "TradePairDetailMargin",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxNotional",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinNotional",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxNotional",
                table: "TradePairDetailMargin");

            migrationBuilder.DropColumn(
                name: "MinNotional",
                table: "TradePairDetailMargin");

            migrationBuilder.DropColumn(
                name: "MaxNotional",
                table: "TradePairDetail");

            migrationBuilder.DropColumn(
                name: "MinNotional",
                table: "TradePairDetail");
        }
    }
}
