using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class VelocityruleRemovePrimarykeyv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "UserAssignModule",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ModuleID = table.Column<long>(nullable: false),
                    ModuleName = table.Column<string>(nullable: true),
                    UserAccessRightsId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignModule", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserAssignModule_UserAccessRights_UserAccessRightsId",
                        column: x => x.UserAccessRightsId,
                        principalTable: "UserAccessRights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignSubModule",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubModuleID = table.Column<long>(nullable: false),
                    SubModuleName = table.Column<string>(nullable: true),
                    View = table.Column<short>(nullable: false),
                    Create = table.Column<short>(nullable: false),
                    Edit = table.Column<short>(nullable: false),
                    Delete = table.Column<short>(nullable: false),
                    UserAssignModuleID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignSubModule", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleID",
                        column: x => x.UserAssignModuleID,
                        principalTable: "UserAssignModule",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignFieldRights",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FieldID = table.Column<long>(nullable: false),
                    FieldName = table.Column<string>(nullable: true),
                    UserAssignSubModuleID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignFieldRights", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
                        column: x => x.UserAssignSubModuleID,
                        principalTable: "UserAssignSubModule",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignToolRights",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ToolID = table.Column<long>(nullable: false),
                    ToolName = table.Column<string>(nullable: true),
                    UserAssignSubModuleID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignToolRights", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleID",
                        column: x => x.UserAssignSubModuleID,
                        principalTable: "UserAssignSubModule",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignFieldRights_UserAssignSubModuleID",
                table: "UserAssignFieldRights",
                column: "UserAssignSubModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignModule_UserAccessRightsId",
                table: "UserAssignModule",
                column: "UserAccessRightsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignSubModule_UserAssignModuleID",
                table: "UserAssignSubModule",
                column: "UserAssignModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignToolRights_UserAssignSubModuleID",
                table: "UserAssignToolRights",
                column: "UserAssignSubModuleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAssignFieldRights");

            migrationBuilder.DropTable(
                name: "UserAssignToolRights");

            migrationBuilder.DropTable(
                name: "UserAssignSubModule");

            migrationBuilder.DropTable(
                name: "UserAssignModule");

            migrationBuilder.DropTable(
                name: "UserAccessRights");
        }
    }
}
