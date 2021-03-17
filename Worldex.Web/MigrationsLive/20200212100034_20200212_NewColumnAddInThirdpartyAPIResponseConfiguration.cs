using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class _20200212_NewColumnAddInThirdpartyAPIResponseConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParsingName",
                table: "ThirdPartyAPIResponseConfiguration",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParsingName",
                table: "ThirdPartyAPIResponseConfiguration");
        }
    }
}
