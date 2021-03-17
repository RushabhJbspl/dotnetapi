using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddedColInTokenUnstakingHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StatusCode",
                table: "TradePoolQueueMarginV1",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "StatusMsg",
                table: "TradePoolQueueMarginV1",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DegradeStakingAmount",
                table: "TokenUnStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "TradePoolQueueMarginV1");

            migrationBuilder.DropColumn(
                name: "StatusMsg",
                table: "TradePoolQueueMarginV1");

            migrationBuilder.DropColumn(
                name: "DegradeStakingAmount",
                table: "TokenUnStakingHistory");
        }
    }
}
