using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class GroupRoleMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "UserAPIKeyDetails");

            migrationBuilder.AddColumn<long>(
                name: "WhiteListIPEndPointID",
                table: "UserAPIKeyDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "IsChargeDeducted",
                table: "MarginWalletTopupRequest",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "APIKeyLimit",
                table: "APIPlanDetail",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "GroupRoleMapping",
                columns: table => new
                {
                    PermissionGroupId = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRoleMapping", x => new { x.PermissionGroupId, x.RoleId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupRoleMapping");

            migrationBuilder.DropColumn(
                name: "WhiteListIPEndPointID",
                table: "UserAPIKeyDetails");

            migrationBuilder.DropColumn(
                name: "IsChargeDeducted",
                table: "MarginWalletTopupRequest");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "APIKeyLimit",
                table: "APIPlanDetail");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "UserAPIKeyDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
