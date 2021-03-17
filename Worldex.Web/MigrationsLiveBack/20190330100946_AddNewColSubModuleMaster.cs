using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddNewColSubModuleMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Controller",
                table: "SubModuleMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MethodName",
                table: "SubModuleMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "SubModuleMaster",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Controller",
                table: "SubModuleMaster");

            migrationBuilder.DropColumn(
                name: "MethodName",
                table: "SubModuleMaster");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "SubModuleMaster");
        }
    }
}
