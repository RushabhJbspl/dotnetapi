using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class TradeTQAccountIDAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryAccountID",
                table: "TradeTransactionQueue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderAccountID",
                table: "TradeTransactionQueue",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryAccountID",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "OrderAccountID",
                table: "TradeTransactionQueue");
        }
    }
}
