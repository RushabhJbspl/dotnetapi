using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketMaker.Infrastructure.Migrations
{
    public partial class DataTypeModificationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "RangeMin",
                table: "MarketMakerRangeDetails",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "RangeMax",
                table: "MarketMakerRangeDetails",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<long>(
                name: "PairId",
                table: "MarketMakerPreferences",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "RangeMin",
                table: "MarketMakerRangeDetails",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "RangeMax",
                table: "MarketMakerRangeDetails",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "PairId",
                table: "MarketMakerPreferences",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
