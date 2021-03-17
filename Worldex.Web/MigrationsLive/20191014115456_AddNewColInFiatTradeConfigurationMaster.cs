using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddNewColInFiatTradeConfigurationMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FiatCurrencyName",
                table: "FiatTradeConfigurationMaster",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "FiatCurrencyRate",
                table: "FiatTradeConfigurationMaster",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FiatCurrencyName",
                table: "FiatTradeConfigurationMaster");

            migrationBuilder.DropColumn(
                name: "FiatCurrencyRate",
                table: "FiatTradeConfigurationMaster");
        }
    }
}
