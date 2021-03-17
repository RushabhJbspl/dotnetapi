using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class Add2NewColInFiatTradeConfigurationMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxLimit",
                table: "FiatTradeConfigurationMaster",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinLimit",
                table: "FiatTradeConfigurationMaster",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxLimit",
                table: "FiatTradeConfigurationMaster");

            migrationBuilder.DropColumn(
                name: "MinLimit",
                table: "FiatTradeConfigurationMaster");
        }
    }
}
