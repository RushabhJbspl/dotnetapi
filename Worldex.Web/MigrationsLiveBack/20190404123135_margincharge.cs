using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class margincharge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SafetyWalletID",
                table: "MarginLoanRequest",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "LoanChargeDetail",
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
                    LoanID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    SafetyWalletID = table.Column<long>(nullable: false),
                    MarginWalletID = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    LoanAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Leverage = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LeverageMax = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SafetyPreBal = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MarginPreBal = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DailyChargePer = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DailyChargeValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MarginChargeCase = table.Column<int>(nullable: false),
                    ErrorCode = table.Column<int>(nullable: false),
                    ErrorMsg = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanChargeDetail", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanChargeDetail");

            migrationBuilder.DropColumn(
                name: "SafetyWalletID",
                table: "MarginLoanRequest");
        }
    }
}
