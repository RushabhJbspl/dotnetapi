using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addpathAndRoundid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BGPath",
                table: "IEORoundMaster",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoundId",
                table: "IEOCurrencyPairMapping",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BGPath",
                table: "IEORoundMaster");

            migrationBuilder.DropColumn(
                name: "RoundId",
                table: "IEOCurrencyPairMapping");
        }
    }
}
