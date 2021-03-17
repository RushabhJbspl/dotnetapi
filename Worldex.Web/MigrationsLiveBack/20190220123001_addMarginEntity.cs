using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addMarginEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarginBlockWalletTrnTypeMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletTypeID = table.Column<long>(nullable: false),
                    TrnTypeID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginBlockWalletTrnTypeMaster", x => new { x.WalletTypeID, x.TrnTypeID });
                    table.UniqueConstraint("AK_MarginBlockWalletTrnTypeMaster_TrnTypeID_WalletTypeID", x => new { x.TrnTypeID, x.WalletTypeID });
                });

            migrationBuilder.CreateTable(
                name: "MarginTransactionAccount",
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
                    table.PrimaryKey("PK_MarginTransactionAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarginUserRoleMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    RoleName = table.Column<string>(maxLength: 20, nullable: false),
                    RoleType = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginUserRoleMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarginWalletAuthorizeUserMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    OrgID = table.Column<long>(nullable: false),
                    RoleID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginWalletAuthorizeUserMaster", x => new { x.UserID, x.WalletID });
                });

            migrationBuilder.CreateTable(
                name: "MarginWalletLedger",
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
                    table.PrimaryKey("PK_MarginWalletLedger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarginWalletMaster",
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
                    AccWalletID = table.Column<string>(maxLength: 16, nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    PublicAddress = table.Column<string>(maxLength: 50, nullable: false),
                    IsDefaultWallet = table.Column<byte>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    OrgID = table.Column<long>(nullable: true),
                    WalletUsageType = table.Column<short>(nullable: false),
                    OutBoundBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    InBoundBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginWalletMaster", x => x.AccWalletID);
                });

            migrationBuilder.CreateTable(
                name: "MarginWalletTransactionOrder",
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
                    table.PrimaryKey("PK_MarginWalletTransactionOrder", x => x.OrderID);
                });

            migrationBuilder.CreateTable(
                name: "MarginWalletTransactionQueue",
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
                    StatusMsg = table.Column<string>(maxLength: 50, nullable: false),
                    SettedAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    AllowedChannelID = table.Column<long>(nullable: false),
                    WalletTrnType = table.Column<int>(nullable: false),
                    WalletDeductionType = table.Column<int>(nullable: false),
                    HoldChargeAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DeductedChargeAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ErrorCode = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginWalletTransactionQueue", x => x.TrnNo);
                });

            migrationBuilder.CreateTable(
                name: "MarginWalletTypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletTypeName = table.Column<string>(maxLength: 7, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    IsDepositionAllow = table.Column<short>(nullable: false),
                    IsWithdrawalAllow = table.Column<short>(nullable: false),
                    IsTransactionWallet = table.Column<short>(nullable: false),
                    IsDefaultWallet = table.Column<short>(nullable: true),
                    ConfirmationCount = table.Column<short>(nullable: true),
                    IsLocal = table.Column<short>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginWalletTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarginWalletUsagePolicy",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PolicyName = table.Column<string>(maxLength: 50, nullable: false),
                    WalletType = table.Column<long>(nullable: false),
                    AllowedIP = table.Column<string>(nullable: false),
                    AllowedLocation = table.Column<string>(nullable: false),
                    AuthenticationType = table.Column<int>(nullable: false),
                    StartTime = table.Column<double>(nullable: true),
                    EndTime = table.Column<double>(nullable: true),
                    HourlyTrnCount = table.Column<long>(nullable: false),
                    HourlyTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DailyTrnCount = table.Column<long>(nullable: false),
                    DailyTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MonthlyTrnCount = table.Column<long>(nullable: false),
                    MonthlyTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    WeeklyTrnCount = table.Column<long>(nullable: false),
                    WeeklyTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    YearlyTrnCount = table.Column<long>(nullable: false),
                    YearlyTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LifeTimeTrnCount = table.Column<long>(nullable: false),
                    LifeTimeTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginWalletUsagePolicy", x => x.WalletType);
                });

            migrationBuilder.CreateTable(
                name: "MarginWTrnTypeMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnTypeId = table.Column<long>(nullable: false),
                    TrnTypeName = table.Column<string>(maxLength: 50, nullable: false),
                    Discription = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginWTrnTypeMaster", x => x.TrnTypeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarginBlockWalletTrnTypeMaster");

            migrationBuilder.DropTable(
                name: "MarginTransactionAccount");

            migrationBuilder.DropTable(
                name: "MarginUserRoleMaster");

            migrationBuilder.DropTable(
                name: "MarginWalletAuthorizeUserMaster");

            migrationBuilder.DropTable(
                name: "MarginWalletLedger");

            migrationBuilder.DropTable(
                name: "MarginWalletMaster");

            migrationBuilder.DropTable(
                name: "MarginWalletTransactionOrder");

            migrationBuilder.DropTable(
                name: "MarginWalletTransactionQueue");

            migrationBuilder.DropTable(
                name: "MarginWalletTypeMaster");

            migrationBuilder.DropTable(
                name: "MarginWalletUsagePolicy");

            migrationBuilder.DropTable(
                name: "MarginWTrnTypeMaster");
        }
    }
}
