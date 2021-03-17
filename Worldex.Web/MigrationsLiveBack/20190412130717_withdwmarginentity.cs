using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class withdwmarginentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WithdrawLoanMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    LoanID = table.Column<string>(nullable: false),
                    SMSCode = table.Column<string>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    ToWalletID = table.Column<long>(nullable: false),
                    SafetyWalletID = table.Column<long>(nullable: false),
                    ProfitWalletID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    SafetyMarginAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ProfitMarginAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SystemRemarks = table.Column<string>(maxLength: 500, nullable: true),
                    StatusMessage = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawLoanMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WithdrawLoanMaster");
        }
    }
}
