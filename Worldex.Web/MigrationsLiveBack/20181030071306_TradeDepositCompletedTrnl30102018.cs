using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class TradeDepositCompletedTrnl30102018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductID",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "RoutID",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "SerProID",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "ServiceID",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "MemberID",
                table: "TradeBuyRequest");

            migrationBuilder.DropColumn(
                name: "TrnDate",
                table: "TradeBuyRequest");

            migrationBuilder.DropColumn(
                name: "AlertRec",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "CashChargePer",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "CashChargeRs",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "CouponNo",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "DAccountNo",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "DBankID",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "DMemberID",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "DeliveryGivenDate",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "IsChargeAccepted",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "IsDebited",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "OAccountNo",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "OBankID",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "OBranchName",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "OChequeDate",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "PGId",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "WalletAmt",
                table: "PoolOrder");

            migrationBuilder.RenameColumn(
                name: "WalletID",
                table: "PoolOrder",
                newName: "UserWalletID");

            migrationBuilder.RenameColumn(
                name: "OMemberID",
                table: "PoolOrder",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "OChequeNo",
                table: "PoolOrder",
                newName: "UserWalletAccID");

            migrationBuilder.RenameColumn(
                name: "DeliveryGivenBy",
                table: "PoolOrder",
                newName: "PoolID");

            migrationBuilder.AlterColumn<short>(
                name: "TrnMode",
                table: "PoolOrder",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AlterColumn<short>(
                name: "PayMode",
                table: "PoolOrder",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.CreateTable(
                name: "TradeDepositCompletedTrn",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnID = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeDepositCompletedTrn", x => new { x.Address, x.TrnID });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeDepositCompletedTrn");

            migrationBuilder.RenameColumn(
                name: "UserWalletID",
                table: "PoolOrder",
                newName: "WalletID");

            migrationBuilder.RenameColumn(
                name: "UserWalletAccID",
                table: "PoolOrder",
                newName: "OChequeNo");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "PoolOrder",
                newName: "OMemberID");

            migrationBuilder.RenameColumn(
                name: "PoolID",
                table: "PoolOrder",
                newName: "DeliveryGivenBy");

            migrationBuilder.AddColumn<long>(
                name: "ProductID",
                table: "TradeTransactionQueue",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "RoutID",
                table: "TradeTransactionQueue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "SerProID",
                table: "TradeTransactionQueue",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ServiceID",
                table: "TradeTransactionQueue",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MemberID",
                table: "TradeBuyRequest",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrnDate",
                table: "TradeBuyRequest",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<byte>(
                name: "TrnMode",
                table: "PoolOrder",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<byte>(
                name: "PayMode",
                table: "PoolOrder",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddColumn<byte>(
                name: "AlertRec",
                table: "PoolOrder",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<double>(
                name: "CashChargePer",
                table: "PoolOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "CashChargeRs",
                table: "PoolOrder",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "CouponNo",
                table: "PoolOrder",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DAccountNo",
                table: "PoolOrder",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "DBankID",
                table: "PoolOrder",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "DMemberID",
                table: "PoolOrder",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryGivenDate",
                table: "PoolOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsChargeAccepted",
                table: "PoolOrder",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDebited",
                table: "PoolOrder",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OAccountNo",
                table: "PoolOrder",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "OBankID",
                table: "PoolOrder",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "OBranchName",
                table: "PoolOrder",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OChequeDate",
                table: "PoolOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "PoolOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PGId",
                table: "PoolOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "WalletAmt",
                table: "PoolOrder",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
