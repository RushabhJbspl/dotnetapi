using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class BaseEntityModification_NJ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "OtpMaster");

            migrationBuilder.DropColumn(
                name: "EnableStatus",
                table: "OtpMaster");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "OtpMaster",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoginType",
                table: "BizUser",
                nullable: false,
                defaultValue: 101);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "OtpMaster");

            migrationBuilder.DropColumn(
                name: "LoginType",
                table: "BizUser");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "OtpMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "EnableStatus",
                table: "OtpMaster",
                nullable: false,
                defaultValue: false);
        }
    }
}
