using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AutoRenew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IsAutoRenew",
                table: "UserSubscribeAPIPlan",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "NextAutoRenewId",
                table: "UserSubscribeAPIPlan",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "PaymentStatus",
                table: "UserSubscribeAPIPlan",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RenewDate",
                table: "UserSubscribeAPIPlan",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "RenewStatus",
                table: "UserSubscribeAPIPlan",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoRenew",
                table: "UserSubscribeAPIPlan");

            migrationBuilder.DropColumn(
                name: "NextAutoRenewId",
                table: "UserSubscribeAPIPlan");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "UserSubscribeAPIPlan");

            migrationBuilder.DropColumn(
                name: "RenewDate",
                table: "UserSubscribeAPIPlan");

            migrationBuilder.DropColumn(
                name: "RenewStatus",
                table: "UserSubscribeAPIPlan");
        }
    }
}
