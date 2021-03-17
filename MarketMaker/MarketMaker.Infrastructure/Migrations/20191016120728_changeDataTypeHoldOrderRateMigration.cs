using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketMaker.Infrastructure.Migrations
{
    public partial class changeDataTypeHoldOrderRateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HoldOrderRateChange",
                table: "MarketMakerPreferences",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,18)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "HoldOrderRateChange",
                table: "MarketMakerPreferences",
                type: "decimal(28,18)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true);
        }
    }
}
