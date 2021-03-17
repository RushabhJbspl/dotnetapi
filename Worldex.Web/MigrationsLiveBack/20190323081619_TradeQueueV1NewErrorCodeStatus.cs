using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class TradeQueueV1NewErrorCodeStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StatusCode",
                table: "TradePoolQueueV1",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "StatusMsg",
                table: "TradePoolQueueV1",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "TradePoolQueueV1");

            migrationBuilder.DropColumn(
                name: "StatusMsg",
                table: "TradePoolQueueV1");
        }
    }
}
