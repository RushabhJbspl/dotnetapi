using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddNewChargeCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PerMinCount",
                table: "UserAPILimitCount",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "PerMinUpdatedDate",
                table: "UserAPILimitCount",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalAmount",
                table: "MarginTrnChargeLog",
                type: "decimal(28, 18)",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DeductChargetType",
                table: "MarginChargeConfigurationDetail",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "IsCurrencyConverted",
                table: "MarginChargeConfigurationDetail",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerMinCount",
                table: "UserAPILimitCount");

            migrationBuilder.DropColumn(
                name: "PerMinUpdatedDate",
                table: "UserAPILimitCount");

            migrationBuilder.DropColumn(
                name: "OriginalAmount",
                table: "MarginTrnChargeLog");

            migrationBuilder.DropColumn(
                name: "DeductChargetType",
                table: "MarginChargeConfigurationDetail");

            migrationBuilder.DropColumn(
                name: "IsCurrencyConverted",
                table: "MarginChargeConfigurationDetail");
        }
    }
}
