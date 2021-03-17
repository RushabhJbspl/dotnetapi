using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ComplainmasterPriorityandmodifycomplaintmaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_coinListRequests",
                table: "coinListRequests");

            migrationBuilder.RenameTable(
                name: "coinListRequests",
                newName: "CoinListRequest");

            migrationBuilder.AlterColumn<decimal>(
                name: "ConvertAmount",
                table: "RouteConfiguration",
                type: "decimal(22, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AddColumn<long>(
                name: "ComplaintPriorityId",
                table: "Complainmaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoinListRequest",
                table: "CoinListRequest",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CoinListRequest",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "ComplaintPriorityId",
                table: "Complainmaster");

            migrationBuilder.RenameTable(
                name: "CoinListRequest",
                newName: "coinListRequests");

            migrationBuilder.AlterColumn<decimal>(
                name: "ConvertAmount",
                table: "RouteConfiguration",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22, 2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_coinListRequests",
                table: "coinListRequests",
                column: "Id");
        }
    }
}
