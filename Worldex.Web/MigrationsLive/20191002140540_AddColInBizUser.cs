using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddColInBizUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolvedDate",
                table: "LPWalletMismatch",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<long>(
                name: "ResolvedBy",
                table: "LPWalletMismatch",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "LPWalletMismatch",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettledAmount",
                table: "LPWalletMismatch",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "StatusMsg",
                table: "LPWalletMismatch",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "IsDeviceAuthEnable",
                table: "BizUser",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "LPWalletMismatch");

            migrationBuilder.DropColumn(
                name: "SettledAmount",
                table: "LPWalletMismatch");

            migrationBuilder.DropColumn(
                name: "StatusMsg",
                table: "LPWalletMismatch");

            migrationBuilder.DropColumn(
                name: "IsDeviceAuthEnable",
                table: "BizUser");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolvedDate",
                table: "LPWalletMismatch",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ResolvedBy",
                table: "LPWalletMismatch",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
