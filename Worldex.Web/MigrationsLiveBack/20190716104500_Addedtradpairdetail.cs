using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class Addedtradpairdetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "AmtLength",
                table: "TradePairDetailArbitrage",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "PriceLength",
                table: "TradePairDetailArbitrage",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "QtyLength",
                table: "TradePairDetailArbitrage",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmtLength",
                table: "TradePairDetailArbitrage");

            migrationBuilder.DropColumn(
                name: "PriceLength",
                table: "TradePairDetailArbitrage");

            migrationBuilder.DropColumn(
                name: "QtyLength",
                table: "TradePairDetailArbitrage");
        }
    }
}
