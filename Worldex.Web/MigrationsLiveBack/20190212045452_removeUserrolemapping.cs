using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class removeUserrolemapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMapping_BizRoles_RoleId",
                table: "UserRoleMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMapping_BizUser_UserId",
                table: "UserRoleMapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoleMapping",
                table: "UserRoleMapping");

            migrationBuilder.RenameTable(
                name: "UserRoleMapping",
                newName: "BizUserRole");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoleMapping_RoleId",
                table: "BizUserRole",
                newName: "IX_BizUserRole_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BizUserRole",
                table: "BizUserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BizUserRole_BizRoles_RoleId",
                table: "BizUserRole",
                column: "RoleId",
                principalTable: "BizRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BizUserRole_BizUser_UserId",
                table: "BizUserRole",
                column: "UserId",
                principalTable: "BizUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BizUserRole_BizRoles_RoleId",
                table: "BizUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_BizUserRole_BizUser_UserId",
                table: "BizUserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BizUserRole",
                table: "BizUserRole");

            migrationBuilder.RenameTable(
                name: "BizUserRole",
                newName: "UserRoleMapping");

            migrationBuilder.RenameIndex(
                name: "IX_BizUserRole_RoleId",
                table: "UserRoleMapping",
                newName: "IX_UserRoleMapping_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoleMapping",
                table: "UserRoleMapping",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleMapping_BizRoles_RoleId",
                table: "UserRoleMapping",
                column: "RoleId",
                principalTable: "BizRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleMapping_BizUser_UserId",
                table: "UserRoleMapping",
                column: "UserId",
                principalTable: "BizUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
