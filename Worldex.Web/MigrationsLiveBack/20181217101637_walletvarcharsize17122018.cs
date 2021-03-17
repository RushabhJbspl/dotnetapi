using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class walletvarcharsize17122018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserWalletBlockTrnTypeMaster",
                table: "UserWalletBlockTrnTypeMaster");

            migrationBuilder.AlterColumn<string>(
                name: "SiteURL",
                table: "AutorizedApps",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecretKey",
                table: "AutorizedApps",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "AppName",
                table: "AutorizedApps",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserWalletBlockTrnTypeMaster",
                table: "UserWalletBlockTrnTypeMaster",
                columns: new[] { "WalletID", "WTrnTypeMasterID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserWalletBlockTrnTypeMaster",
                table: "UserWalletBlockTrnTypeMaster");

            migrationBuilder.AlterColumn<string>(
                name: "SiteURL",
                table: "AutorizedApps",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SecretKey",
                table: "AutorizedApps",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "AppName",
                table: "AutorizedApps",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserWalletBlockTrnTypeMaster",
                table: "UserWalletBlockTrnTypeMaster",
                column: "Id");
        }
    }
}
