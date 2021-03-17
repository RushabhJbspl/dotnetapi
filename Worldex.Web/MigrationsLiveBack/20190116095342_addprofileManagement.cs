using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addprofileManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableStatus",
                table: "Typemaster");

            migrationBuilder.DropColumn(
                name: "ActiveStatus",
                table: "SubscriptionMaster");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "SubscriptionMaster");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "SubscriptionMaster");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ProfileMaster",
                newName: "SubscriptionAmount");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "ProfileMaster",
                newName: "KYCLevel");

            migrationBuilder.RenameColumn(
                name: "EnableStatus",
                table: "ProfileMaster",
                newName: "IsRecursive");

            migrationBuilder.AddColumn<string>(
                name: "AccessibleFeatures",
                table: "SubscriptionMaster",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WithdrawalLimit",
                table: "ProfileMaster",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AddColumn<string>(
                name: "DepositLimit",
                table: "ProfileMaster",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProfileExpiry",
                table: "ProfileMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ProfileFree",
                table: "ProfileMaster",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "Profilelevel",
                table: "ProfileMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "TradeLimit",
                table: "ProfileMaster",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionLimit",
                table: "ProfileMaster",
                maxLength: 2000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessibleFeatures",
                table: "SubscriptionMaster");

            migrationBuilder.DropColumn(
                name: "DepositLimit",
                table: "ProfileMaster");

            migrationBuilder.DropColumn(
                name: "IsProfileExpiry",
                table: "ProfileMaster");

            migrationBuilder.DropColumn(
                name: "ProfileFree",
                table: "ProfileMaster");

            migrationBuilder.DropColumn(
                name: "Profilelevel",
                table: "ProfileMaster");

            migrationBuilder.DropColumn(
                name: "TradeLimit",
                table: "ProfileMaster");

            migrationBuilder.DropColumn(
                name: "TransactionLimit",
                table: "ProfileMaster");

            migrationBuilder.RenameColumn(
                name: "SubscriptionAmount",
                table: "ProfileMaster",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "KYCLevel",
                table: "ProfileMaster",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "IsRecursive",
                table: "ProfileMaster",
                newName: "EnableStatus");

            migrationBuilder.AddColumn<bool>(
                name: "EnableStatus",
                table: "Typemaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ActiveStatus",
                table: "SubscriptionMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "SubscriptionMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "SubscriptionMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "WithdrawalLimit",
                table: "ProfileMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
