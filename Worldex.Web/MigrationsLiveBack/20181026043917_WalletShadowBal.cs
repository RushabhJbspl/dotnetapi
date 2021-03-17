using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class WalletShadowBal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerProConfigurationID",
                table: "ThirdPartyAPIConfiguration");

            migrationBuilder.AddColumn<int>(
                name: "WalletTrnType",
                table: "WalletTransactionQueues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "WalletLimitConfiguration",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "WalletLimitConfiguration",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "TimeStamp",
                table: "ThirdPartyAPIConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConvertAmount",
                table: "RouteConfiguration",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WalletTrnType",
                table: "WalletTransactionQueues");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "WalletLimitConfiguration");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "WalletLimitConfiguration");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "ThirdPartyAPIConfiguration");

            migrationBuilder.DropColumn(
                name: "ConvertAmount",
                table: "RouteConfiguration");

            migrationBuilder.AddColumn<long>(
                name: "SerProConfigurationID",
                table: "ThirdPartyAPIConfiguration",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
