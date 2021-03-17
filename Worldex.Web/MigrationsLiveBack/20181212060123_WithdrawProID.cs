using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class WithdrawProID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Wallet",
                table: "WithdrawHistory",
                newName: "WalletId");

            migrationBuilder.AddColumn<string>(
                name: "ProviderWalletID",
                table: "WithdrawHistory",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "TrnAcBatch",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderWalletID",
                table: "WithdrawHistory");

            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "WithdrawHistory",
                newName: "Wallet");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "TrnAcBatch",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
