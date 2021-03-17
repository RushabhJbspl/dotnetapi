using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class APIKeyWhitelistIPConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessRightId",
                table: "PermissionGroupMaster");

            migrationBuilder.DropColumn(
                name: "LinkedRoles",
                table: "PermissionGroupMaster");

            migrationBuilder.AddColumn<long>(
                name: "APIPlanID",
                table: "WhiteListIPEndPoint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalAmount",
                table: "TrnChargeLog",
                type: "decimal(28, 18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.CreateTable(
                name: "APIKeyWhitelistIPConfig",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    APIKeyID = table.Column<long>(nullable: false),
                    IPId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIKeyWhitelistIPConfig", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIKeyWhitelistIPConfig");

            migrationBuilder.DropColumn(
                name: "APIPlanID",
                table: "WhiteListIPEndPoint");

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalAmount",
                table: "TrnChargeLog",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AccessRightId",
                table: "PermissionGroupMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LinkedRoles",
                table: "PermissionGroupMaster",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
