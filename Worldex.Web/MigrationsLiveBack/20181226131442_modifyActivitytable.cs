using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class modifyActivitytable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Application",
                table: "ActivityRegister");

            migrationBuilder.DropColumn(
                name: "EndPoint",
                table: "ActivityRegister");

            migrationBuilder.RenameColumn(
                name: "Response",
                table: "ActivityRegister",
                newName: "Session");

            migrationBuilder.RenameColumn(
                name: "Request",
                table: "ActivityRegister",
                newName: "AccessToken");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "ActivityRegister",
                newName: "ReturnCode");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ActivityRegister",
                newName: "Remark");

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityTypeId",
                table: "ActivityRegister",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "AliasName",
                table: "ActivityRegister",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationId",
                table: "ActivityRegister",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "ActivityRegister",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ErrorCode",
                table: "ActivityRegister",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "HostURLId",
                table: "ActivityRegister",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "ActivityRegister",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModuleTypeId",
                table: "ActivityRegister",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ReturnMsg",
                table: "ActivityRegister",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActivityRegisterDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ActivityId = table.Column<Guid>(nullable: false),
                    Request = table.Column<string>(maxLength: 8000, nullable: true),
                    Response = table.Column<string>(maxLength: 8000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRegisterDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityType_Master",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    TypeMaster = table.Column<string>(maxLength: 4000, nullable: true),
                    AliasName = table.Column<string>(maxLength: 1000, nullable: true),
                    IsDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityType_Master", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HostURLMaster",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HostURL = table.Column<string>(maxLength: 500, nullable: true),
                    AliasName = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostURLMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityRegisterDetail");

            migrationBuilder.DropTable(
                name: "ActivityType_Master");

            migrationBuilder.DropTable(
                name: "HostURLMaster");

            migrationBuilder.DropColumn(
                name: "ActivityTypeId",
                table: "ActivityRegister");

            migrationBuilder.DropColumn(
                name: "AliasName",
                table: "ActivityRegister");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "ActivityRegister");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "ActivityRegister");

            migrationBuilder.DropColumn(
                name: "ErrorCode",
                table: "ActivityRegister");

            migrationBuilder.DropColumn(
                name: "HostURLId",
                table: "ActivityRegister");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "ActivityRegister");

            migrationBuilder.DropColumn(
                name: "ModuleTypeId",
                table: "ActivityRegister");

            migrationBuilder.DropColumn(
                name: "ReturnMsg",
                table: "ActivityRegister");

            migrationBuilder.RenameColumn(
                name: "Session",
                table: "ActivityRegister",
                newName: "Response");

            migrationBuilder.RenameColumn(
                name: "ReturnCode",
                table: "ActivityRegister",
                newName: "EventId");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "ActivityRegister",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "AccessToken",
                table: "ActivityRegister",
                newName: "Request");

            migrationBuilder.AddColumn<string>(
                name: "Application",
                table: "ActivityRegister",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndPoint",
                table: "ActivityRegister",
                maxLength: 500,
                nullable: true);
        }
    }
}
