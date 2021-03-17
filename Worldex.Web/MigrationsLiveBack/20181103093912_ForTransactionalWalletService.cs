using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ForTransactionalWalletService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WalletTypeName",
                table: "WalletTypeMasters",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SMSCode",
                table: "ServiceMaster",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WalletTypeName",
                table: "WalletTypeMasters",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 7);

            migrationBuilder.AlterColumn<string>(
                name: "SMSCode",
                table: "ServiceMaster",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 6);
        }
    }
}
