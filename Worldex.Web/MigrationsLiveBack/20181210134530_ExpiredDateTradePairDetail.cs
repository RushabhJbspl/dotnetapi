using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ExpiredDateTradePairDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "ChargeType",
                table: "TradePairDetail",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OpenOrderExpiration",
                table: "TradePairDetail",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "ParameterInfo",
            //    table: "TemplateMaster",
            //    maxLength: 200,
            //    nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargeType",
                table: "TradePairDetail");

            migrationBuilder.DropColumn(
                name: "OpenOrderExpiration",
                table: "TradePairDetail");

            //migrationBuilder.DropColumn(
            //    name: "ParameterInfo",
            //    table: "TemplateMaster");
        }
    }
}
