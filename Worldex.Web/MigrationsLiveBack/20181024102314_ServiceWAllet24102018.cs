using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ServiceWAllet24102018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StckingScheme",
                table: "StckingScheme");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberShadowLimit",
                table: "MemberShadowLimit");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserStacking",
                newName: "SchemeId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "UserStacking",
                newName: "WalletType");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "StckingScheme",
                newName: "WalletType");

            migrationBuilder.RenameColumn(
                name: "LimitAmount",
                table: "StckingScheme",
                newName: "MinLimitAmount");

            migrationBuilder.RenameColumn(
                name: "MemberTypeId",
                table: "StckingScheme",
                newName: "Percent");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "MemberShadowLimit",
                newName: "WalletType");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxLimitAmount",
                table: "StckingScheme",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TimePeriod",
                table: "StckingScheme",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "MemberShadowLimitId",
                table: "MemberShadowBalance",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WalletTypeId",
                table: "MemberShadowBalance",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "DeviceMaster",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StckingScheme",
                table: "StckingScheme",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberShadowLimit",
                table: "MemberShadowLimit",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ServiceTypeMapping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    ServiceId = table.Column<long>(nullable: false),
                    TrnType = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypeMapping", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceTypeMapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StckingScheme",
                table: "StckingScheme");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberShadowLimit",
                table: "MemberShadowLimit");

            migrationBuilder.DropColumn(
                name: "MaxLimitAmount",
                table: "StckingScheme");

            migrationBuilder.DropColumn(
                name: "TimePeriod",
                table: "StckingScheme");

            migrationBuilder.DropColumn(
                name: "MemberShadowLimitId",
                table: "MemberShadowBalance");

            migrationBuilder.DropColumn(
                name: "WalletTypeId",
                table: "MemberShadowBalance");

            migrationBuilder.RenameColumn(
                name: "WalletType",
                table: "UserStacking",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "SchemeId",
                table: "UserStacking",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "WalletType",
                table: "StckingScheme",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Percent",
                table: "StckingScheme",
                newName: "MemberTypeId");

            migrationBuilder.RenameColumn(
                name: "MinLimitAmount",
                table: "StckingScheme",
                newName: "LimitAmount");

            migrationBuilder.RenameColumn(
                name: "WalletType",
                table: "MemberShadowLimit",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "DeviceMaster",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StckingScheme",
                table: "StckingScheme",
                column: "MemberTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberShadowLimit",
                table: "MemberShadowLimit",
                column: "MemberTypeId");
        }
    }
}
