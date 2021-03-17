using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class modifyemailtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveBy",
                table: "AddUserWalletRequest");

            migrationBuilder.DropColumn(
                name: "ApproveDate",
                table: "AddUserWalletRequest");

            migrationBuilder.RenameColumn(
                name: "WalletOwnerUserID",
                table: "AddUserWalletRequest",
                newName: "FromUserId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PhoneMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EmailMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AddUserWalletRequest",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PhoneMaster");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EmailMaster");

            migrationBuilder.RenameColumn(
                name: "FromUserId",
                table: "AddUserWalletRequest",
                newName: "WalletOwnerUserID");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AddUserWalletRequest",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<long>(
                name: "ApproveBy",
                table: "AddUserWalletRequest",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveDate",
                table: "AddUserWalletRequest",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
