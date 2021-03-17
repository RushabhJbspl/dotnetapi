using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddEntityVisibility_FieldRequirer_AccessRights : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Visibility",
                table: "FieldMaster",
                newName: "Required");

            migrationBuilder.AddColumn<short>(
                name: "Visibility",
                table: "SubModuleFormMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "AccressRight",
                table: "FieldMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "ModuleAccessRightsMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    AccessRightName = table.Column<string>(maxLength: 20, nullable: true),
                    AccessRightValue = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleAccessRightsMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleFieldRequirerMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: true),
                    Value = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleFieldRequirerMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleVisibilityMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    VisibilityName = table.Column<string>(maxLength: 20, nullable: true),
                    VisibilityValue = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleVisibilityMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleAccessRightsMaster");

            migrationBuilder.DropTable(
                name: "ModuleFieldRequirerMaster");

            migrationBuilder.DropTable(
                name: "ModuleVisibilityMaster");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "SubModuleFormMaster");

            migrationBuilder.DropColumn(
                name: "AccressRight",
                table: "FieldMaster");

            migrationBuilder.RenameColumn(
                name: "Required",
                table: "FieldMaster",
                newName: "Visibility");
        }
    }
}
