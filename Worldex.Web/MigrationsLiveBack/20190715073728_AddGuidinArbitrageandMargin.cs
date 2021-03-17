using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddGuidinArbitrageandMargin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "WithdrawLoanMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "UserLoanMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefGuid",
                table: "MarginWalletTransactionQueue",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "MarginWalletLedger",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "MarginTransactionAccount",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "MarginLoanRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "MarginDepositHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "LPArbitrageWalletMismatch",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "LPArbitrageWalletLedger",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "RefGuid",
                table: "ArbitrageWalletTransactionQueue",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "ArbitrageTransactionAccount",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "ArbitrageDepositFund",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawLoanMaster_Guid",
                table: "WithdrawLoanMaster",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoanMaster_Guid",
                table: "UserLoanMaster",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_MarginWalletTransactionQueue_RefGuid",
                table: "MarginWalletTransactionQueue",
                column: "RefGuid");

            migrationBuilder.CreateIndex(
                name: "IX_MarginLoanRequest_Guid",
                table: "MarginLoanRequest",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_MarginDepositHistory_GUID",
                table: "MarginDepositHistory",
                column: "GUID");

            migrationBuilder.CreateIndex(
                name: "IX_LPArbitrageWalletMismatch_Guid",
                table: "LPArbitrageWalletMismatch",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_ArbitrageWalletTransactionQueue_RefGuid",
                table: "ArbitrageWalletTransactionQueue",
                column: "RefGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ArbitrageDepositFund_GUID",
                table: "ArbitrageDepositFund",
                column: "GUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WithdrawLoanMaster_Guid",
                table: "WithdrawLoanMaster");

            migrationBuilder.DropIndex(
                name: "IX_UserLoanMaster_Guid",
                table: "UserLoanMaster");

            migrationBuilder.DropIndex(
                name: "IX_MarginWalletTransactionQueue_RefGuid",
                table: "MarginWalletTransactionQueue");

            migrationBuilder.DropIndex(
                name: "IX_MarginLoanRequest_Guid",
                table: "MarginLoanRequest");

            migrationBuilder.DropIndex(
                name: "IX_MarginDepositHistory_GUID",
                table: "MarginDepositHistory");

            migrationBuilder.DropIndex(
                name: "IX_LPArbitrageWalletMismatch_Guid",
                table: "LPArbitrageWalletMismatch");

            migrationBuilder.DropIndex(
                name: "IX_ArbitrageWalletTransactionQueue_RefGuid",
                table: "ArbitrageWalletTransactionQueue");

            migrationBuilder.DropIndex(
                name: "IX_ArbitrageDepositFund_GUID",
                table: "ArbitrageDepositFund");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "WithdrawLoanMaster");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "UserLoanMaster");

            migrationBuilder.DropColumn(
                name: "RefGuid",
                table: "MarginWalletTransactionQueue");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "MarginLoanRequest");

            migrationBuilder.DropColumn(
                name: "GUID",
                table: "MarginDepositHistory");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "LPArbitrageWalletMismatch");

            migrationBuilder.DropColumn(
                name: "RefGuid",
                table: "ArbitrageWalletTransactionQueue");

            migrationBuilder.DropColumn(
                name: "GUID",
                table: "ArbitrageDepositFund");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "MarginWalletLedger",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "MarginTransactionAccount",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "LPArbitrageWalletLedger",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "ArbitrageTransactionAccount",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1000);
        }
    }
}
