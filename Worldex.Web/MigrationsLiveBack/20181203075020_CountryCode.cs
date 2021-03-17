using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class CountryCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "TempUserRegister",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "BizUser",
                maxLength: 5,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "TempUserRegister");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "BizUser");
        }
    }
}
