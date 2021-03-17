using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class DecimalPointChangeV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellMinQty",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellMinPrice",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellMaxQty",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellMaxPrice",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellFees",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyPrice",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyMinQty",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyMinPrice",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyMaxQty",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyMaxPrice",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyFees",
                table: "TradePairDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellMinQty",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellMinPrice",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellMaxQty",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellMaxPrice",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellFees",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyPrice",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyMinQty",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyMinPrice",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyMaxQty",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyMaxPrice",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyFees",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");
        }
    }
}
