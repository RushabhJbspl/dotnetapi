using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class STOPLOSSFollowers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FollowingTo",
                table: "TradeStopLoss",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "ISFollowersReq",
                table: "TradeStopLoss",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowingTo",
                table: "TradeStopLoss");

            migrationBuilder.DropColumn(
                name: "ISFollowersReq",
                table: "TradeStopLoss");
        }
    }
}
