using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addTransactionTypeinFiat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FiatCoinConfiguration",
                table: "FiatCoinConfiguration");

            migrationBuilder.AddColumn<short>(
                name: "TransactionType",
                table: "FiatCoinConfiguration",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FiatCoinConfiguration",
                table: "FiatCoinConfiguration",
                columns: new[] { "FromCurrencyId", "ToCurrencyId", "TransactionType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FiatCoinConfiguration",
                table: "FiatCoinConfiguration");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "FiatCoinConfiguration");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FiatCoinConfiguration",
                table: "FiatCoinConfiguration",
                columns: new[] { "FromCurrencyId", "ToCurrencyId" });
        }
    }
}
