using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class UpdateWalletServiceV21510 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StatusMsg",
                table: "WalletTransactionQueues",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "WalletTransactionQueues",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<string>(
                name: "StatusMsg",
                table: "WalletTransactionOrders",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "WalletTransactionOrders",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StatusMsg",
                table: "WalletTransactionQueues",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "WalletTransactionQueues",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<string>(
                name: "StatusMsg",
                table: "WalletTransactionOrders",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "WalletTransactionOrders",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");
        }
    }
}
