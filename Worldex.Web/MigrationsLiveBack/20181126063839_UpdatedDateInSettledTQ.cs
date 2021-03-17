using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CleanArchitecture.Web.Migrations
{
    public partial class UpdatedDateInSettledTQ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
               name: "UpdatedDate",
               table: "SettledTradeTransactionQueue",
               nullable: true,
               oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
