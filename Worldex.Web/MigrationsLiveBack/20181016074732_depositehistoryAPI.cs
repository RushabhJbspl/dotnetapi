using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class depositehistoryAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TimeStamp",
                table: "WalletTransactionQueues",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "WalletTransactionOrders",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<long>(
                name: "userId",
                table: "DepositHistorys",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "DepositHistorys");

            migrationBuilder.AlterColumn<string>(
                name: "TimeStamp",
                table: "WalletTransactionQueues",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "WalletTransactionOrders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
