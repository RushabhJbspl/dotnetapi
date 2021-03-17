using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addIsPrivateAPITardeColToTrdaeTransactionQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IsPrivateAPITarde",
                table: "TradeTransactionQueueMargin",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsPrivateAPITarde",
                table: "TradeTransactionQueue",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivateAPITarde",
                table: "TradeTransactionQueueMargin");

            migrationBuilder.DropColumn(
                name: "IsPrivateAPITarde",
                table: "TradeTransactionQueue");
        }
    }
}
