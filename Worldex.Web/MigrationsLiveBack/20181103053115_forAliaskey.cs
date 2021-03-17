using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class forAliaskey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_TradeTransactionQueue",
            //    table: "TradeTransactionQueue");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TradeBuyerList_TrnNo",
                table: "TradeBuyerList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeBuyerList",
                table: "TradeBuyerList");

            migrationBuilder.AddColumn<string>(
                name: "AliasKey",
                table: "BizUser",
                nullable: true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_TradeTransactionQueue",
            //    table: "TradeTransactionQueue",
            //    column: "TrnNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeBuyerList",
                table: "TradeBuyerList",
                column: "TrnNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeTransactionQueue",
                table: "TradeTransactionQueue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeBuyerList",
                table: "TradeBuyerList");

            migrationBuilder.DropColumn(
                name: "AliasKey",
                table: "BizUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeTransactionQueue",
                table: "TradeTransactionQueue",
                columns: new[] { "Id", "TrnNo" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TradeBuyerList_TrnNo",
                table: "TradeBuyerList",
                column: "TrnNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeBuyerList",
                table: "TradeBuyerList",
                columns: new[] { "Id", "TrnNo" });
        }
    }
}
