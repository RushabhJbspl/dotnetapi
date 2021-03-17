using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class settledQueueChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SettledTradeTransactionQueue",
                table: "SettledTradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "ProductID",
                table: "SettledTradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "RoutID",
                table: "SettledTradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "SerProID",
                table: "SettledTradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "ServiceID",
                table: "SettledTradeTransactionQueue");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerPer",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SettledTradeTransactionQueue",
                table: "SettledTradeTransactionQueue",
                column: "TrnNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SettledTradeTransactionQueue",
                table: "SettledTradeTransactionQueue");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerPer",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AddColumn<long>(
                name: "ProductID",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "RoutID",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "SerProID",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ServiceID",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SettledTradeTransactionQueue",
                table: "SettledTradeTransactionQueue",
                column: "Id");
        }
    }
}
