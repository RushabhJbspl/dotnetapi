using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addremarksinbankfiat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "UserBankRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "UserBankMaster",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "UserBankRequest");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "UserBankMaster");
        }
    }
}
