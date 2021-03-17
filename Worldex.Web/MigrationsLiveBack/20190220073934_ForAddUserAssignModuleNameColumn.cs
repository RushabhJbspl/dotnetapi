using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ForAddUserAssignModuleNameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "RoleMaster");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "UserAccessRights",
                newName: "GroupID");

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

            //migrationBuilder.AddColumn<long>(
            //    name: "CreatedBy",
            //    table: "BizRoles",
            //    nullable: false,
            //    defaultValue: 0L);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "CreatedDate",
            //    table: "BizRoles",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddColumn<long>(
            //    name: "PermissionGroupID",
            //    table: "BizRoles",
            //    nullable: false,
            //    defaultValue: 0L);

            //migrationBuilder.AddColumn<short>(
            //    name: "Status",
            //    table: "BizRoles",
            //    nullable: false,
            //    defaultValue: (short)0);

            //migrationBuilder.AddColumn<long>(
            //    name: "UpdatedBy",
            //    table: "BizRoles",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "UpdatedDate",
            //    table: "BizRoles",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "PermissionGroupMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    GroupName = table.Column<string>(maxLength: 50, nullable: false),
                    GroupDescription = table.Column<string>(maxLength: 100, nullable: true),
                    AccessRightId = table.Column<long>(nullable: false),
                    IPAddress = table.Column<string>(nullable: true),
                    LinkedRoles = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGroupMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionGroupMaster");

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

            //migrationBuilder.DropColumn(
            //    name: "CreatedBy",
            //    table: "BizRoles");

            //migrationBuilder.DropColumn(
            //    name: "CreatedDate",
            //    table: "BizRoles");

            //migrationBuilder.DropColumn(
            //    name: "PermissionGroupID",
            //    table: "BizRoles");

            //migrationBuilder.DropColumn(
            //    name: "Status",
            //    table: "BizRoles");

            //migrationBuilder.DropColumn(
            //    name: "UpdatedBy",
            //    table: "BizRoles");

            //migrationBuilder.DropColumn(
            //    name: "UpdatedDate",
            //    table: "BizRoles");

            migrationBuilder.RenameColumn(
                name: "GroupID",
                table: "UserAccessRights",
                newName: "UserID");

            //migrationBuilder.CreateTable(
            //    name: "RoleMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        ConcurrencyStamp = table.Column<string>(nullable: true),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        Name = table.Column<string>(nullable: true),
            //        NormalizedName = table.Column<string>(nullable: true),
            //        PermissionGroupID = table.Column<long>(nullable: false),
            //        RoleDescription = table.Column<string>(maxLength: 250, nullable: true),
            //        Status = table.Column<short>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RoleMaster", x => x.Id);
            //    });
        }
    }
}
