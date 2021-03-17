using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddParentIDForMenuRights : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignToolRights");

            //migrationBuilder.DropIndex(
            //    name: "IX_UserAssignToolRights_UserAssignSubModuleID",
            //    table: "UserAssignToolRights");

            //migrationBuilder.DropIndex(
            //    name: "IX_UserAssignFieldRights_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights");

            migrationBuilder.AddColumn<long>(
                name: "ParentID",
                table: "UserAssignSubModule",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ParentID",
                table: "UserAssignModule",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ParentID",
                table: "SubModuleMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ParentID",
                table: "ModuleMaster",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "UserAssignSubModule");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "UserAssignModule");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "SubModuleMaster");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "ModuleMaster");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserAssignToolRights_UserAssignSubModuleID",
            //    table: "UserAssignToolRights",
            //    column: "UserAssignSubModuleID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserAssignFieldRights_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights",
            //    column: "UserAssignSubModuleID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights",
            //    column: "UserAssignSubModuleID",
            //    principalTable: "UserAssignSubModule",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignToolRights",
            //    column: "UserAssignSubModuleID",
            //    principalTable: "UserAssignSubModule",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
