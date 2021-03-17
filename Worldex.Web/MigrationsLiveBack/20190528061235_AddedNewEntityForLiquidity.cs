using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddedNewEntityForLiquidity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "LPType",
                table: "WalletTransactionQueues",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsVerifiedByAdmin",
                table: "TransactionQueue",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsAdminApprovalRequired",
                table: "RouteConfiguration",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "LPCharge",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    ChargeValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChargeType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LPCharge", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LPTransactionAccount",
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
                    table.PrimaryKey("PK_LPTransactionAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LPWalletLedger",
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
                    table.PrimaryKey("PK_LPWalletLedger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LPWalletMaster",
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
                    table.PrimaryKey("PK_LPWalletMaster", x => new { x.WalletTypeID, x.SerProID });
                    table.UniqueConstraint("AK_LPWalletMaster_SerProID_WalletTypeID", x => new { x.SerProID, x.WalletTypeID });
                });

            migrationBuilder.CreateTable(
                name: "LPWalletMismatch",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    TPBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SystemBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MismatchaingAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ResolvedBy = table.Column<long>(nullable: false),
                    ResolvedDate = table.Column<DateTime>(nullable: false),
                    ResolvedRemarks = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LPWalletMismatch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawAdminRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    ApprovedBy = table.Column<long>(nullable: false),
                    ApprovalDate = table.Column<DateTime>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawAdminRequest", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LPCharge");

            migrationBuilder.DropTable(
                name: "LPTransactionAccount");

            migrationBuilder.DropTable(
                name: "LPWalletLedger");

            migrationBuilder.DropTable(
                name: "LPWalletMaster");

            migrationBuilder.DropTable(
                name: "LPWalletMismatch");

            migrationBuilder.DropTable(
                name: "WithdrawAdminRequest");

            migrationBuilder.DropColumn(
                name: "LPType",
                table: "WalletTransactionQueues");

            migrationBuilder.DropColumn(
                name: "IsVerifiedByAdmin",
                table: "TransactionQueue");

            migrationBuilder.DropColumn(
                name: "IsAdminApprovalRequired",
                table: "RouteConfiguration");
        }
    }
}
