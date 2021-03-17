using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addcolMarginChargePer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<decimal>(
                name: "MarginChargePer",
                table: "LeverageMaster",
                type: "decimal(28, 18)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoleMapping",
                table: "UserRoleMapping",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.CreateTable(
                name: "RoleHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ModificationDetail = table.Column<string>(maxLength: 250, nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Module = table.Column<int>(nullable: false),
                    IPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleMaster",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    RoleDescription = table.Column<string>(maxLength: 250, nullable: true),
                    PermissionGroupID = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMaster", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMapping_BizRoles_RoleId",
                table: "UserRoleMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMapping_BizUser_UserId",
                table: "UserRoleMapping");

            migrationBuilder.DropTable(
                name: "RoleHistory");

            migrationBuilder.DropTable(
                name: "RoleMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoleMapping",
                table: "UserRoleMapping");

            migrationBuilder.DropColumn(
                name: "MarginChargePer",
                table: "LeverageMaster");

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
    }
}
