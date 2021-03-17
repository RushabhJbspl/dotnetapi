using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class userwallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WalletAuthorizeUserMaster_Id_RoleID_UserID_WalletID",
                table: "WalletAuthorizeUserMaster");

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

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WalletAuthorizeUserMaster_Id_UserID_WalletID",
                table: "WalletAuthorizeUserMaster",
                columns: new[] { "Id", "UserID", "WalletID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WalletAuthorizeUserMaster_Id_UserID_WalletID",
                table: "WalletAuthorizeUserMaster");

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

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AddUserWalletRequest",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WalletAuthorizeUserMaster_Id_RoleID_UserID_WalletID",
                table: "WalletAuthorizeUserMaster",
                columns: new[] { "Id", "RoleID", "UserID", "WalletID" });
        }
    }
}
