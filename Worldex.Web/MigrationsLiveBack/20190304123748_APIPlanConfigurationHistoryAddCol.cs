using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class APIPlanConfigurationHistoryAddCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LastModifyBy",
                table: "APIPlanConfigurationHistory",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifyDate",
                table: "APIPlanConfigurationHistory",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifyDetails",
                table: "APIPlanConfigurationHistory",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PlanID",
                table: "APIPlanConfigurationHistory",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModifyBy",
                table: "APIPlanConfigurationHistory");

            migrationBuilder.DropColumn(
                name: "LastModifyDate",
                table: "APIPlanConfigurationHistory");

            migrationBuilder.DropColumn(
                name: "ModifyDetails",
                table: "APIPlanConfigurationHistory");

            migrationBuilder.DropColumn(
                name: "PlanID",
                table: "APIPlanConfigurationHistory");
        }
    }
}
