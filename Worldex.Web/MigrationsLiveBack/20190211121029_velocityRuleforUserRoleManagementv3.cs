using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class velocityRuleforUserRoleManagementv3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignFieldRights");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights");

            migrationBuilder.RenameColumn(
                name: "UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights",
                newName: "UserAssignSubModuleSubModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignToolRights_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights",
                newName: "IX_UserAssignToolRights_UserAssignSubModuleSubModuleID");

            migrationBuilder.RenameColumn(
                name: "UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule",
                newName: "UserAssignModuleModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignSubModule_UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule",
                newName: "IX_UserAssignSubModule_UserAssignModuleModuleID");

            migrationBuilder.RenameColumn(
                name: "UserAssignSubModule<long>SubModuleID",
                table: "UserAssignFieldRights",
                newName: "UserAssignSubModuleSubModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignFieldRights_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignFieldRights",
                newName: "IX_UserAssignFieldRights_UserAssignSubModuleSubModuleID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleSubModuleID",
                table: "UserAssignFieldRights",
                column: "UserAssignSubModuleSubModuleID",
                principalTable: "UserAssignSubModule",
                principalColumn: "SubModuleID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleModuleID",
                table: "UserAssignSubModule",
                column: "UserAssignModuleModuleID",
                principalTable: "UserAssignModule",
                principalColumn: "ModuleID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleSubModuleID",
                table: "UserAssignToolRights",
                column: "UserAssignSubModuleSubModuleID",
                principalTable: "UserAssignSubModule",
                principalColumn: "SubModuleID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleSubModuleID",
                table: "UserAssignFieldRights");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleModuleID",
                table: "UserAssignSubModule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleSubModuleID",
                table: "UserAssignToolRights");

            migrationBuilder.RenameColumn(
                name: "UserAssignSubModuleSubModuleID",
                table: "UserAssignToolRights",
                newName: "UserAssignSubModule<long>SubModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignToolRights_UserAssignSubModuleSubModuleID",
                table: "UserAssignToolRights",
                newName: "IX_UserAssignToolRights_UserAssignSubModule<long>SubModuleID");

            migrationBuilder.RenameColumn(
                name: "UserAssignModuleModuleID",
                table: "UserAssignSubModule",
                newName: "UserAssignModule<long>ModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignSubModule_UserAssignModuleModuleID",
                table: "UserAssignSubModule",
                newName: "IX_UserAssignSubModule_UserAssignModule<long>ModuleID");

            migrationBuilder.RenameColumn(
                name: "UserAssignSubModuleSubModuleID",
                table: "UserAssignFieldRights",
                newName: "UserAssignSubModule<long>SubModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignFieldRights_UserAssignSubModuleSubModuleID",
                table: "UserAssignFieldRights",
                newName: "IX_UserAssignFieldRights_UserAssignSubModule<long>SubModuleID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignFieldRights",
                column: "UserAssignSubModule<long>SubModuleID",
                principalTable: "UserAssignSubModule",
                principalColumn: "SubModuleID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule",
                column: "UserAssignModule<long>ModuleID",
                principalTable: "UserAssignModule",
                principalColumn: "ModuleID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights",
                column: "UserAssignSubModule<long>SubModuleID",
                principalTable: "UserAssignSubModule",
                principalColumn: "SubModuleID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
