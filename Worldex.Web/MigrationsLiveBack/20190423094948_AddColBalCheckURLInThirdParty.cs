using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddColBalCheckURLInThirdParty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BalCheckRequestBody",
                table: "ThirdPartyAPIConfiguration",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalCheckRequestBody",
                table: "ThirdPartyAPIConfiguration");
        }
    }
}
