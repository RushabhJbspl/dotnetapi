using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class currencytype_wallettype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrencyTypeID",
                table: "WalletTypeMasters",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyTypeID",
                table: "MarginWalletTypeMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanProfit",
                table: "MarginChargeOrder",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyTypeID",
                table: "WalletTypeMasters");

            migrationBuilder.DropColumn(
                name: "CurrencyTypeID",
                table: "MarginWalletTypeMaster");

            migrationBuilder.DropColumn(
                name: "LoanProfit",
                table: "MarginChargeOrder");
        }
    }
}
