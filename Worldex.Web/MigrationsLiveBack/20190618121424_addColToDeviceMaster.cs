using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addColToDeviceMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryTime",
                table: "DeviceMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "DeviceMaster",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "DeviceMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "DeviceMaster",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryTime",
                table: "DeviceMaster");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "DeviceMaster");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "DeviceMaster");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "DeviceMaster");
        }
    }
}
