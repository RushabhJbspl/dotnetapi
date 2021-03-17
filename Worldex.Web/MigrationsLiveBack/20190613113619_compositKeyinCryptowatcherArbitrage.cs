using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class compositKeyinCryptowatcherArbitrage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CryptoWatcherArbitrage",
                table: "CryptoWatcherArbitrage");

            migrationBuilder.AddColumn<short>(
                name: "IsSmartArbitrage",
                table: "TransactionQueueArbitrage",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<string>(
                name: "SecretKey",
                table: "ServiceProConfigurationArbitrage",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "APIKey",
                table: "ServiceProConfigurationArbitrage",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SecretKey",
                table: "ServiceProConfiguration",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "APIKey",
                table: "ServiceProConfiguration",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CryptoWatcherArbitrage",
                table: "CryptoWatcherArbitrage",
                columns: new[] { "LPType", "PairId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CryptoWatcherArbitrage",
                table: "CryptoWatcherArbitrage");

            migrationBuilder.DropColumn(
                name: "IsSmartArbitrage",
                table: "TransactionQueueArbitrage");

            migrationBuilder.AlterColumn<string>(
                name: "SecretKey",
                table: "ServiceProConfigurationArbitrage",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "APIKey",
                table: "ServiceProConfigurationArbitrage",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "SecretKey",
                table: "ServiceProConfiguration",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "APIKey",
                table: "ServiceProConfiguration",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CryptoWatcherArbitrage",
                table: "CryptoWatcherArbitrage",
                column: "Id");
        }
    }
}
