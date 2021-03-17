using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddNewResponseParam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "BuySellTopUpRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "BuySellTopUpRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bank",
                table: "BuySellTopUpRequest");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "BuySellTopUpRequest");
        }
    }
}
