using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class velocityRuleforUserRoleManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeildMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    FeildName = table.Column<string>(nullable: true),
                    SubModuleID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeildMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ModuleName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubModuleMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    SubModuleName = table.Column<string>(nullable: true),
                    ModuleID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubModuleMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ToolMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ToolName = table.Column<string>(nullable: true),
                    SubModuleID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessRights",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UserID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessRights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignModule<long>",
                columns: table => new
                {
                    ModuleID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ModuleName = table.Column<string>(nullable: true),
                    UserAccessRightsId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignModule<long>", x => x.ModuleID);
                    table.ForeignKey(
                        name: "FK_UserAssignModule<long>_UserAccessRights_UserAccessRightsId",
                        column: x => x.UserAccessRightsId,
                        principalTable: "UserAccessRights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignSubModule<long>",
                columns: table => new
                {
                    SubModuleID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubModuleName = table.Column<string>(nullable: true),
                    View = table.Column<short>(nullable: false),
                    Create = table.Column<short>(nullable: false),
                    Edit = table.Column<short>(nullable: false),
                    Delete = table.Column<short>(nullable: false),
                    UserAssignModulelongModuleID = table.Column<long>(name: "UserAssignModule<long>ModuleID", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignSubModule<long>", x => x.SubModuleID);
                    table.ForeignKey(
                        name: "FK_UserAssignSubModule<long>_UserAssignModule<long>_UserAssignModule<long>ModuleID",
                        column: x => x.UserAssignModulelongModuleID,
                        principalTable: "UserAssignModule<long>",
                        principalColumn: "ModuleID",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "UserAssignToolRights<long>",
                columns: table => new
                {
                    ToolID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ToolName = table.Column<string>(nullable: true),
                    UserAssignSubModulelongSubModuleID = table.Column<long>(name: "UserAssignSubModule<long>SubModuleID", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignToolRights<long>", x => x.ToolID);
                    table.ForeignKey(
                        name: "FK_UserAssignToolRights<long>_UserAssignSubModule<long>_UserAssignSubModule<long>SubModuleID",
                        column: x => x.UserAssignSubModulelongSubModuleID,
                        principalTable: "UserAssignSubModule<long>",
                        principalColumn: "SubModuleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignFeildRights<long>_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignFeildRights<long>",
                column: "UserAssignSubModule<long>SubModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignModule<long>_UserAccessRightsId",
                table: "UserAssignModule<long>",
                column: "UserAccessRightsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignSubModule<long>_UserAssignModule<long>ModuleID",
                table: "UserAssignSubModule<long>",
                column: "UserAssignModule<long>ModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignToolRights<long>_UserAssignSubModule<long>SubModuleID",
                table: "UserAssignToolRights<long>",
                column: "UserAssignSubModule<long>SubModuleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeildMaster");

            migrationBuilder.DropTable(
                name: "ModuleMaster");

            migrationBuilder.DropTable(
                name: "SubModuleMaster");

            migrationBuilder.DropTable(
                name: "ToolMaster");

            migrationBuilder.DropTable(
                name: "UserAssignFeildRights<long>");

            migrationBuilder.DropTable(
                name: "UserAssignToolRights<long>");

            migrationBuilder.DropTable(
                name: "UserAssignSubModule<long>");

            migrationBuilder.DropTable(
                name: "UserAssignModule<long>");

            migrationBuilder.DropTable(
                name: "UserAccessRights");
        }
    }
}
