using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class myaccountserviceForBirjubhai : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTrnNo",
                table: "TradePairDetail");

            //migrationBuilder.RenameColumn(
            //    name: "SerProID",
            //    table: "RouteConfiguration",
            //    newName: "SerProDetailID");

            migrationBuilder.RenameColumn(
                name: "StateID",
                table: "ProductConfiguration",
                newName: "CountryID");

            migrationBuilder.AlterColumn<long>(
                name: "StatusCode",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "CountryMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    CountryName = table.Column<string>(maxLength: 30, nullable: false),
                    CountryCode = table.Column<string>(maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomPassword",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    EnableStatus = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPassword", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    DeviceId = table.Column<string>(maxLength: 250, nullable: false),
                    IsEnable = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoolOrder",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    TrnMode = table.Column<byte>(nullable: false),
                    OMemberID = table.Column<long>(nullable: false),
                    PayMode = table.Column<byte>(nullable: false),
                    OrderAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    DiscPer = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    DiscRs = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    OBankID = table.Column<short>(nullable: false),
                    OBranchName = table.Column<string>(nullable: true),
                    OAccountNo = table.Column<string>(nullable: true),
                    OChequeNo = table.Column<string>(nullable: true),
                    OChequeDate = table.Column<DateTime>(nullable: false),
                    DMemberID = table.Column<long>(nullable: false),
                    DBankID = table.Column<short>(nullable: false),
                    DAccountNo = table.Column<string>(nullable: false),
                    ORemarks = table.Column<string>(nullable: true),
                    DeliveryAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    DRemarks = table.Column<string>(nullable: true),
                    DeliveryGivenBy = table.Column<long>(nullable: false),
                    DeliveryGivenDate = table.Column<DateTime>(nullable: false),
                    AlertRec = table.Column<byte>(nullable: false),
                    CashChargePer = table.Column<double>(nullable: false),
                    CashChargeRs = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    WalletAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    PGId = table.Column<int>(nullable: false),
                    CouponNo = table.Column<long>(nullable: false),
                    IsChargeAccepted = table.Column<bool>(nullable: false),
                    IsDebited = table.Column<bool>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    CancelID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StateMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    StateName = table.Column<string>(maxLength: 30, nullable: false),
                    StateCode = table.Column<string>(maxLength: 2, nullable: false),
                    CountryID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeTransactionStatus",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    SettledQty = table.Column<decimal>(nullable: false),
                    TotalQty = table.Column<decimal>(nullable: false),
                    DeliveredQty = table.Column<decimal>(nullable: false),
                    PendingQty = table.Column<decimal>(nullable: false),
                    SoldPrice = table.Column<decimal>(nullable: false),
                    BidPrice = table.Column<decimal>(nullable: false),
                    OrderID = table.Column<long>(nullable: false),
                    StockID = table.Column<long>(nullable: false),
                    SellStockID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeTransactionStatus", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryMaster");

            migrationBuilder.DropTable(
                name: "CustomPassword");

            migrationBuilder.DropTable(
                name: "DeviceMaster");

            migrationBuilder.DropTable(
                name: "PoolOrder");

            migrationBuilder.DropTable(
                name: "StateMaster");

            migrationBuilder.DropTable(
                name: "TradeTransactionStatus");

            migrationBuilder.RenameColumn(
                name: "SerProDetailID",
                table: "RouteConfiguration",
                newName: "SerProID");

            migrationBuilder.RenameColumn(
                name: "CountryID",
                table: "ProductConfiguration",
                newName: "StateID");

            migrationBuilder.AlterColumn<int>(
                name: "StatusCode",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "LastTrnNo",
                table: "TradePairDetail",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
