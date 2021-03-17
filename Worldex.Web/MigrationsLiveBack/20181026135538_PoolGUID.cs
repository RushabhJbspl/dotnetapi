using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class PoolGUID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "WalletLimitConfiguration",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "WalletLimitConfiguration",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AddColumn<Guid>(
                name: "GUID",
                table: "TradePoolMaster",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "PairId",
                table: "TradePoolMaster",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GUID",
                table: "TradePoolMaster");

            migrationBuilder.DropColumn(
                name: "PairId",
                table: "TradePoolMaster");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "WalletLimitConfiguration",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "WalletLimitConfiguration",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);
        }
    }
}
