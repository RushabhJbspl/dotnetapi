using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class marginloanchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarginWalletTopupRequest");

            migrationBuilder.AlterColumn<int>(
                name: "WalletUsageType",
                table: "MarginWalletMaster",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<decimal>(
                name: "MarginChargePer",
                table: "LeverageMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MarginLoanRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    WalletTypeID = table.Column<long>(nullable: false),
                    FromWalletID = table.Column<long>(nullable: false),
                    ToWalletID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    LeverageID = table.Column<long>(nullable: false),
                    IsAutoApprove = table.Column<short>(nullable: false),
                    RequestRemarks = table.Column<string>(maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    LeverageAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChargeAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SafetyMarginAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ApprovedBy = table.Column<long>(nullable: true),
                    ApprovedDate = table.Column<DateTime>(nullable: true),
                    ApprovedRemarks = table.Column<string>(maxLength: 500, nullable: true),
                    SystemRemarks = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsChargeDeducted = table.Column<short>(nullable: false),
                    Leverage = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxLeverage = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TotalSafetyCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DailyChargePer = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LastChargeCalculated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginLoanRequest", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarginLoanRequest");

            migrationBuilder.AlterColumn<short>(
                name: "WalletUsageType",
                table: "MarginWalletMaster",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "MarginChargePer",
                table: "LeverageMaster",
                type: "decimal(28, 18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.CreateTable(
                name: "MarginWalletTopupRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ApprovedBy = table.Column<long>(nullable: true),
                    ApprovedDate = table.Column<DateTime>(nullable: true),
                    ApprovedRemarks = table.Column<string>(maxLength: 500, nullable: true),
                    ChargeAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    FromWalletID = table.Column<long>(nullable: false),
                    IsAutoApprove = table.Column<short>(nullable: false),
                    IsChargeDeducted = table.Column<short>(nullable: false),
                    Leverage = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LeverageAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LeverageID = table.Column<long>(nullable: false),
                    RequestRemarks = table.Column<string>(maxLength: 500, nullable: false),
                    SafetyMarginAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    SystemRemarks = table.Column<string>(maxLength: 500, nullable: true),
                    ToWalletID = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UserID = table.Column<long>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginWalletTopupRequest", x => x.Id);
                });
        }
    }
}
