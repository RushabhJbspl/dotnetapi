using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addGUIDinwithdrawdeposit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "WithdrawHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "DepositHistory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawHistory_GUID",
                table: "WithdrawHistory",
                column: "GUID");

            migrationBuilder.CreateIndex(
                name: "IX_DepositHistory_GUID",
                table: "DepositHistory",
                column: "GUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WithdrawHistory_GUID",
                table: "WithdrawHistory");

            migrationBuilder.DropIndex(
                name: "IX_DepositHistory_GUID",
                table: "DepositHistory");

            migrationBuilder.DropColumn(
                name: "GUID",
                table: "WithdrawHistory");

            migrationBuilder.DropColumn(
                name: "GUID",
                table: "DepositHistory");
        }
    }
}
