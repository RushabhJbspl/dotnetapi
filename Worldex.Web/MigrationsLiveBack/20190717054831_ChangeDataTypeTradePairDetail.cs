using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ChangeDataTypeTradePairDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "QtyLength",
                table: "TradePairDetailArbitrage",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<int>(
                name: "PriceLength",
                table: "TradePairDetailArbitrage",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<int>(
                name: "AmtLength",
                table: "TradePairDetailArbitrage",
                nullable: false,
                oldClrType: typeof(short));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "QtyLength",
                table: "TradePairDetailArbitrage",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<short>(
                name: "PriceLength",
                table: "TradePairDetailArbitrage",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<short>(
                name: "AmtLength",
                table: "TradePairDetailArbitrage",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
