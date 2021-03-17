using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addnewparamforFiat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "BuySellTopUpRequest",
                newName: "CurrencyName");

            migrationBuilder.RenameColumn(
                name: "Bank",
                table: "BuySellTopUpRequest",
                newName: "CurrencyId");

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "BuySellTopUpRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "BuySellTopUpRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankId",
                table: "BuySellTopUpRequest");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "BuySellTopUpRequest");

            migrationBuilder.RenameColumn(
                name: "CurrencyName",
                table: "BuySellTopUpRequest",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                table: "BuySellTopUpRequest",
                newName: "Bank");
        }
    }
}
