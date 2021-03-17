using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addTrnHashCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LPType",
                table: "WalletTransactionQueues",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddColumn<string>(
                name: "TrnHash",
                table: "TransferFeeHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrnHash",
                table: "TokenSupplyHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrnHash",
                table: "DestroyFundRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrnHash",
                table: "TransferFeeHistory");

            migrationBuilder.DropColumn(
                name: "TrnHash",
                table: "TokenSupplyHistory");

            migrationBuilder.DropColumn(
                name: "TrnHash",
                table: "DestroyFundRequest");

            migrationBuilder.AlterColumn<short>(
                name: "LPType",
                table: "WalletTransactionQueues",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
