using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddCol_UtilityTypes_CrudTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CrudTypes",
                table: "SubModuleMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtilityTypes",
                table: "SubModuleMaster",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrudTypes",
                table: "SubModuleMaster");

            migrationBuilder.DropColumn(
                name: "UtilityTypes",
                table: "SubModuleMaster");
        }
    }
}
