using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class velocityRuleforUserRoleManagementv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignModule<long>_UserAccessRights_UserAccessRightsId",
                table: "UserAssignModule<long>");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignSubModule<long>_UserAssignModule<long>_UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule<long>");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignToolRights<long>_UserAssignSubModule<long>_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights<long>");

            migrationBuilder.DropTable(
                name: "FeildMaster");

            migrationBuilder.DropTable(
                name: "UserAssignFeildRights<long>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignToolRights<long>",
                table: "UserAssignToolRights<long>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignSubModule<long>",
                table: "UserAssignSubModule<long>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignModule<long>",
                table: "UserAssignModule<long>");

            migrationBuilder.RenameTable(
                name: "UserAssignToolRights<long>",
                newName: "UserAssignToolRights");

            migrationBuilder.RenameTable(
                name: "UserAssignSubModule<long>",
                newName: "UserAssignSubModule");

            migrationBuilder.RenameTable(
                name: "UserAssignModule<long>",
                newName: "UserAssignModule");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignToolRights<long>_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights",
                newName: "IX_UserAssignToolRights_UserAssignSubModule<long>SubModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignSubModule<long>_UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule",
                newName: "IX_UserAssignSubModule_UserAssignModule<long>ModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignModule<long>_UserAccessRightsId",
                table: "UserAssignModule",
                newName: "IX_UserAssignModule_UserAccessRightsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignToolRights",
                table: "UserAssignToolRights",
                column: "ToolID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignSubModule",
                table: "UserAssignSubModule",
                column: "SubModuleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignModule",
                table: "UserAssignModule",
                column: "ModuleID");

            migrationBuilder.CreateTable(
                name: "FieldMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    FieldName = table.Column<string>(nullable: true),
                    SubModuleID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignFieldRights",
                columns: table => new
                {
                    FieldID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FieldName = table.Column<string>(nullable: true),
                    UserAssignSubModulelongSubModuleID = table.Column<long>(name: "UserAssignSubModule<long>SubModuleID", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignFieldRights", x => x.FieldID);
                    table.ForeignKey(
                        name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModule<long>SubModuleID",
                        column: x => x.UserAssignSubModulelongSubModuleID,
                        principalTable: "UserAssignSubModule",
                        principalColumn: "SubModuleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignFieldRights_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignFieldRights",
                column: "UserAssignSubModule<long>SubModuleID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignModule_UserAccessRights_UserAccessRightsId",
                table: "UserAssignModule",
                column: "UserAccessRightsId",
                principalTable: "UserAccessRights",
                principalColumn: "Id",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignModule_UserAccessRights_UserAccessRightsId",
                table: "UserAssignModule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights");

            migrationBuilder.DropTable(
                name: "FieldMaster");

            migrationBuilder.DropTable(
                name: "UserAssignFieldRights");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignToolRights",
                table: "UserAssignToolRights");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignSubModule",
                table: "UserAssignSubModule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignModule",
                table: "UserAssignModule");

            migrationBuilder.RenameTable(
                name: "UserAssignToolRights",
                newName: "UserAssignToolRights<long>");

            migrationBuilder.RenameTable(
                name: "UserAssignSubModule",
                newName: "UserAssignSubModule<long>");

            migrationBuilder.RenameTable(
                name: "UserAssignModule",
                newName: "UserAssignModule<long>");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignToolRights_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights<long>",
                newName: "IX_UserAssignToolRights<long>_UserAssignSubModule<long>SubModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignSubModule_UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule<long>",
                newName: "IX_UserAssignSubModule<long>_UserAssignModule<long>ModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignModule_UserAccessRightsId",
                table: "UserAssignModule<long>",
                newName: "IX_UserAssignModule<long>_UserAccessRightsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignToolRights<long>",
                table: "UserAssignToolRights<long>",
                column: "ToolID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignSubModule<long>",
                table: "UserAssignSubModule<long>",
                column: "SubModuleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignModule<long>",
                table: "UserAssignModule<long>",
                column: "ModuleID");

            migrationBuilder.CreateTable(
                name: "FeildMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    FeildName = table.Column<string>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    SubModuleID = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeildMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignFeildRights<long>",
                columns: table => new
                {
                    FeildID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FeildName = table.Column<string>(nullable: true),
                    UserAssignSubModulelongSubModuleID = table.Column<long>(name: "UserAssignSubModule<long>SubModuleID", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignFeildRights<long>", x => x.FeildID);
                    table.ForeignKey(
                        name: "FK_UserAssignFeildRights<long>_UserAssignSubModule<long>_UserAssignSubModule<long>SubModuleID",
                        column: x => x.UserAssignSubModulelongSubModuleID,
                        principalTable: "UserAssignSubModule<long>",
                        principalColumn: "SubModuleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignFeildRights<long>_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignFeildRights<long>",
                column: "UserAssignSubModule<long>SubModuleID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignModule<long>_UserAccessRights_UserAccessRightsId",
                table: "UserAssignModule<long>",
                column: "UserAccessRightsId",
                principalTable: "UserAccessRights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignSubModule<long>_UserAssignModule<long>_UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule<long>",
                column: "UserAssignModule<long>ModuleID",
                principalTable: "UserAssignModule<long>",
                principalColumn: "ModuleID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignToolRights<long>_UserAssignSubModule<long>_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights<long>",
                column: "UserAssignSubModule<long>SubModuleID",
                principalTable: "UserAssignSubModule<long>",
                principalColumn: "SubModuleID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
