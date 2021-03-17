using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class removeclustorOnDeviceMasterEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropUniqueConstraint(
            //    name: "IX_UserUniqueDevice",
            //    table: "DeviceMaster");

            //migrationBuilder.DropIndex(
            //    name: "UserUniqueDeviceIndex",
            //    table: "DeviceMaster");

            //migrationBuilder.AlterColumn<string>(
            //    name: "DeviceId",
            //    table: "DeviceMaster",
            //    maxLength: 250,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 250);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Device",
            //    table: "DeviceMaster",
            //    maxLength: 250,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 250);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<string>(
            //    name: "DeviceId",
            //    table: "DeviceMaster",
            //    maxLength: 250,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 250,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Device",
            //    table: "DeviceMaster",
            //    maxLength: 250,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 250,
            //    oldNullable: true);

            //migrationBuilder.AddUniqueConstraint(
            //    name: "IX_UserUniqueDevice",
            //    table: "DeviceMaster",
            //    columns: new[] { "Device", "DeviceId", "DeviceOS", "UserId" });

            //migrationBuilder.CreateIndex(
            //    name: "UserUniqueDeviceIndex",
            //    table: "DeviceMaster",
            //    columns: new[] { "Device", "DeviceId", "DeviceOS", "UserId" },
            //    unique: true);
        }
    }
}
