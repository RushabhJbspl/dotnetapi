using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketMaker.Infrastructure.Migrations
{
    public partial class changeEntityDatatype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SellUpPercentage",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "SellThreshold",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "SellDownPercentage",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyUpPercentage",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyThreshold",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyDownPercentage",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SellUpPercentage",
                table: "MarketMakerPreferences",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellThreshold",
                table: "MarketMakerPreferences",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "SellDownPercentage",
                table: "MarketMakerPreferences",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "BuyUpPercentage",
                table: "MarketMakerPreferences",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyThreshold",
                table: "MarketMakerPreferences",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,18)");

            migrationBuilder.AlterColumn<int>(
                name: "BuyDownPercentage",
                table: "MarketMakerPreferences",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,18)");
        }
    }
}
