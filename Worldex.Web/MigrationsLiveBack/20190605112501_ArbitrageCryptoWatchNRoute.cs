using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ArbitrageCryptoWatchNRoute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArbitrageLPWalletMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Walletname = table.Column<string>(maxLength: 50, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false),
                    AccWalletID = table.Column<Guid>(nullable: false),
                    SerProID = table.Column<long>(nullable: false),
                    IsDefaultWallet = table.Column<byte>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    OrgID = table.Column<long>(nullable: true),
                    WalletUsageType = table.Column<short>(nullable: false),
                    OutBoundBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    InBoundBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageLPWalletMaster", x => new { x.WalletTypeID, x.SerProID });
                    table.UniqueConstraint("AK_ArbitrageLPWalletMaster_SerProID_WalletTypeID", x => new { x.SerProID, x.WalletTypeID });
                });

            migrationBuilder.CreateTable(
                name: "ArbitrageTransactionAccount",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    BatchNo = table.Column<long>(nullable: false),
                    RefNo = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    CrAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DrAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Remarks = table.Column<string>(maxLength: 150, nullable: false),
                    IsSettled = table.Column<short>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageTransactionAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArbitrageWalletLedger",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletId = table.Column<long>(nullable: false),
                    ToWalletId = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    ServiceTypeID = table.Column<int>(nullable: false),
                    TrnType = table.Column<int>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    CrAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DrAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    PreBal = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    PostBal = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Remarks = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageWalletLedger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArbitrageWalletMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletName = table.Column<string>(maxLength: 50, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false),
                    AccWalletID = table.Column<string>(maxLength: 50, nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    IsDefaultWallet = table.Column<byte>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    OrgID = table.Column<long>(nullable: true),
                    WalletUsageType = table.Column<short>(nullable: false),
                    OutBoundBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    InBoundBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageWalletMaster", x => x.AccWalletID);
                });

            migrationBuilder.CreateTable(
                name: "ArbitrageWalletTransactionOrder",
                columns: table => new
                {
                    OrderID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    OWalletID = table.Column<long>(nullable: false),
                    DWalletID = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    WalletType = table.Column<string>(maxLength: 10, nullable: false),
                    OTrnNo = table.Column<long>(nullable: false),
                    DTrnNo = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StatusMsg = table.Column<string>(maxLength: 50, nullable: false),
                    ChargeAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageWalletTransactionOrder", x => x.OrderID);
                });

            migrationBuilder.CreateTable(
                name: "ArbitrageWalletTransactionQueue",
                columns: table => new
                {
                    TrnNo = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(maxLength: 50, nullable: false),
                    TrnType = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TrnRefNo = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    WalletID = table.Column<long>(nullable: false),
                    WalletType = table.Column<string>(maxLength: 10, nullable: false),
                    MemberID = table.Column<long>(nullable: false),
                    TimeStamp = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StatusMsg = table.Column<string>(maxLength: 100, nullable: false),
                    SettedAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    AllowedChannelID = table.Column<long>(nullable: false),
                    WalletTrnType = table.Column<int>(nullable: false),
                    WalletDeductionType = table.Column<int>(nullable: false),
                    HoldChargeAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DeductedChargeAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ErrorCode = table.Column<long>(nullable: true),
                    IsProcessing = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageWalletTransactionQueue", x => x.TrnNo);
                });

            migrationBuilder.CreateTable(
                name: "ArbitrageWalletTypeMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletTypeName = table.Column<string>(maxLength: 7, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    IsDepositionAllow = table.Column<short>(nullable: false),
                    IsWithdrawalAllow = table.Column<short>(nullable: false),
                    IsTransactionWallet = table.Column<short>(nullable: false),
                    IsDefaultWallet = table.Column<short>(nullable: true),
                    ConfirmationCount = table.Column<short>(nullable: true),
                    IsLocal = table.Column<short>(nullable: true),
                    CurrencyTypeID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageWalletTypeMaster", x => x.WalletTypeName);
                });

            migrationBuilder.CreateTable(
                name: "CryptoWatcherArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LTP = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Pair = table.Column<string>(nullable: false),
                    LPType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoWatcherArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LPArbitrageLPTransactionAccount",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    BatchNo = table.Column<long>(nullable: false),
                    RefNo = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    CrAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DrAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Remarks = table.Column<string>(maxLength: 150, nullable: false),
                    IsSettled = table.Column<short>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LPArbitrageLPTransactionAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LPArbitrageWalletLedger",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletId = table.Column<long>(nullable: false),
                    ToWalletId = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    ServiceTypeID = table.Column<int>(nullable: false),
                    TrnType = table.Column<int>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    CrAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DrAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    PreBal = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    PostBal = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Remarks = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LPArbitrageWalletLedger", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbitrageLPWalletMaster");

            migrationBuilder.DropTable(
                name: "ArbitrageTransactionAccount");

            migrationBuilder.DropTable(
                name: "ArbitrageWalletLedger");

            migrationBuilder.DropTable(
                name: "ArbitrageWalletMaster");

            migrationBuilder.DropTable(
                name: "ArbitrageWalletTransactionOrder");

            migrationBuilder.DropTable(
                name: "ArbitrageWalletTransactionQueue");

            migrationBuilder.DropTable(
                name: "ArbitrageWalletTypeMaster");

            migrationBuilder.DropTable(
                name: "CryptoWatcherArbitrage");

            migrationBuilder.DropTable(
                name: "LPArbitrageLPTransactionAccount");

            migrationBuilder.DropTable(
                name: "LPArbitrageWalletLedger");
        }
    }
}
