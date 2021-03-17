using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class UpdateDeviceMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "DeviceMaster",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "Device",
                table: "DeviceMaster",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceOS",
                table: "DeviceMaster",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Device",
                table: "DeviceMaster");

            migrationBuilder.DropColumn(
                name: "DeviceOS",
                table: "DeviceMaster");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "DeviceMaster",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);
        }
    }
}
