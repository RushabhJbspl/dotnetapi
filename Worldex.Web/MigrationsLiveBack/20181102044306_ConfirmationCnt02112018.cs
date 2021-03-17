using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ConfirmationCnt02112018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "ConfirmationCount",
                table: "WalletTypeMasters",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConfirmationCount",
                table: "RouteConfiguration",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationCount",
                table: "WalletTypeMasters");

            migrationBuilder.DropColumn(
                name: "ConfirmationCount",
                table: "RouteConfiguration");
        }
    }
}
