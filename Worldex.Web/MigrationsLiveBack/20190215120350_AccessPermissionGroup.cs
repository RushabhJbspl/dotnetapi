using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AccessPermissionGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToolName",
                table: "UserAssignToolRights");

            migrationBuilder.DropColumn(
                name: "SubModuleName",
                table: "UserAssignSubModule");

            migrationBuilder.DropColumn(
                name: "ModuleName",
                table: "UserAssignModule");

            migrationBuilder.DropColumn(
                name: "FieldName",
                table: "UserAssignFieldRights");

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "UserAssignToolRights",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "UserAssignSubModule",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "UserAssignModule",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "UserAssignFieldRights",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserAssignToolRights");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserAssignSubModule");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserAssignModule");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserAssignFieldRights");

            migrationBuilder.AddColumn<string>(
                name: "ToolName",
                table: "UserAssignToolRights",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubModuleName",
                table: "UserAssignSubModule",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModuleName",
                table: "UserAssignModule",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldName",
                table: "UserAssignFieldRights",
                nullable: true);
        }
    }
}
