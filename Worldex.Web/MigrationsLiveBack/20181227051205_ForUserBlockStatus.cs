using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ForUserBlockStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discription",
                table: "WalletTypeMasters",
                newName: "Description");

            migrationBuilder.AddColumn<long>(
                name: "PairID",
                table: "TradeStopLoss",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "BizUser",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PairID",
                table: "TradeStopLoss");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "BizUser");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WalletTypeMasters",
                newName: "Discription");
        }
    }
}
