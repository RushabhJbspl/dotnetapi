using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class SettledTQNewColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryAccountID",
                table: "SettledTradeTransactionQueue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderAccountID",
                table: "SettledTradeTransactionQueue",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryAccountID",
                table: "SettledTradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "OrderAccountID",
                table: "SettledTradeTransactionQueue");
        }
    }
}
