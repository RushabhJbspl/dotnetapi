using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class DeviceMasterWithIndex_NJ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "DeviceMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: true),
            //        Status = table.Column<short>(nullable: false),
            //        UserId = table.Column<int>(nullable: false),
            //        Device = table.Column<string>(maxLength: 250, nullable: false),
            //        DeviceOS = table.Column<string>(maxLength: 250, nullable: false),
            //        DeviceId = table.Column<string>(maxLength: 250, nullable: false),
            //        IsEnable = table.Column<bool>(nullable: false),
            //        IsDeleted = table.Column<bool>(nullable: false),
            //        Guid = table.Column<Guid>(nullable: false),
            //        ExpiryTime = table.Column<DateTime>(nullable: false),
            //        Location = table.Column<string>(nullable: true),
            //        IPAddress = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DeviceMaster", x => x.Id);
            //        table.UniqueConstraint("IX_UserUniqueDevice", x => new { x.Device, x.DeviceId, x.DeviceOS, x.UserId });
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "UserUniqueDeviceIndex",
            //    table: "DeviceMaster",
            //    columns: new[] { "Device", "DeviceId", "DeviceOS", "UserId" },
            //    unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "DeviceMaster");
        }
    }
}
