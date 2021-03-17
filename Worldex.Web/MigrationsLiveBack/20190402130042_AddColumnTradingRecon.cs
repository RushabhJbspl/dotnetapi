using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddColumnTradingRecon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StatusCode",
                table: "TradingRecon",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "StatusMsg",
                table: "TradingRecon",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "ISOrderBySystem",
                table: "TradeTransactionQueueMargin",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsWithoutAmtHold",
                table: "TradeTransactionQueueMargin",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "ISOrderBySystem",
                table: "TradeSellerListMarginV1",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsWithoutAmtHold",
                table: "TradeSellerListMarginV1",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "TradingRecon");

            migrationBuilder.DropColumn(
                name: "StatusMsg",
                table: "TradingRecon");

            migrationBuilder.DropColumn(
                name: "ISOrderBySystem",
                table: "TradeTransactionQueueMargin");

            migrationBuilder.DropColumn(
                name: "IsWithoutAmtHold",
                table: "TradeTransactionQueueMargin");

            migrationBuilder.DropColumn(
                name: "ISOrderBySystem",
                table: "TradeSellerListMarginV1");

            migrationBuilder.DropColumn(
                name: "IsWithoutAmtHold",
                table: "TradeSellerListMarginV1");
        }
    }
}
