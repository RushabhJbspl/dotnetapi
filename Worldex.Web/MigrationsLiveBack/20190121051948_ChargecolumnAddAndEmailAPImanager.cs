using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ChargecolumnAddAndEmailAPImanager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestID",
                table: "RequestFormatMaster");

            migrationBuilder.DropColumn(
                name: "CommSerproID",
                table: "CommServiceproviderMaster");

            migrationBuilder.DropColumn(
                name: "CommServiceID",
                table: "CommServiceMaster");

            migrationBuilder.DropColumn(
                name: "APID",
                table: "CommAPIServiceMaster");

            migrationBuilder.RenameColumn(
                name: "contentType",
                table: "RequestFormatMaster",
                newName: "ContentType");

            migrationBuilder.AddColumn<decimal>(
                name: "DeductedChargeAmount",
                table: "WalletTransactionQueues",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "ErrorCode",
                table: "WalletTransactionQueues",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HoldChargeAmount",
                table: "WalletTransactionQueues",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ChargeAmount",
                table: "WalletTransactionOrders",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RequestName",
                table: "RequestFormatMaster",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SMSBalURL",
                table: "CommAPIServiceMaster",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeductedChargeAmount",
                table: "WalletTransactionQueues");

            migrationBuilder.DropColumn(
                name: "ErrorCode",
                table: "WalletTransactionQueues");

            migrationBuilder.DropColumn(
                name: "HoldChargeAmount",
                table: "WalletTransactionQueues");

            migrationBuilder.DropColumn(
                name: "ChargeAmount",
                table: "WalletTransactionOrders");

            migrationBuilder.DropColumn(
                name: "RequestName",
                table: "RequestFormatMaster");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "RequestFormatMaster",
                newName: "contentType");

            migrationBuilder.AddColumn<long>(
                name: "RequestID",
                table: "RequestFormatMaster",
                maxLength: 60,
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CommSerproID",
                table: "CommServiceproviderMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CommServiceID",
                table: "CommServiceMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "SMSBalURL",
                table: "CommAPIServiceMaster",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "APID",
                table: "CommAPIServiceMaster",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
