using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class userloanavgentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserLoanMasterID",
                table: "MarginLoanRequest",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "UserLoanMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    SMSCode = table.Column<string>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    ToWalletID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    LeverageAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SafetyMarginAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SystemRemarks = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Leverage = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxLeverage = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TotalSafetyCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DailyChargePer = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LastChargeCalculated = table.Column<DateTime>(nullable: true),
                    SafetyWalletID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoanMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLoanMaster");

            migrationBuilder.DropColumn(
                name: "UserLoanMasterID",
                table: "MarginLoanRequest");
        }
    }
}
