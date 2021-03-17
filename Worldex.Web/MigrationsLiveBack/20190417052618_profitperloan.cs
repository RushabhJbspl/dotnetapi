using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class profitperloan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ProfitAmount",
                table: "UserLoanMaster",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "LoanID",
                table: "OpenPositionMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LoanID",
                table: "MarginPNLAccount",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "SiteTokenConversionMargin",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    GUID = table.Column<Guid>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    SourceCurrencyID = table.Column<long>(nullable: false),
                    SourceCurrency = table.Column<string>(nullable: true),
                    TargerCurrencyID = table.Column<long>(nullable: false),
                    TargerCurrency = table.Column<string>(nullable: true),
                    SourceCurrencyQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TargerCurrencyQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SourceToBasePrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SourceToBaseQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TokenPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SiteTokenMasterID = table.Column<long>(nullable: false),
                    TimeStamp = table.Column<string>(nullable: true),
                    StatusMsg = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTokenConversionMargin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteTokenMasterMargin",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    CurrencyID = table.Column<long>(nullable: false),
                    BaseCurrencyID = table.Column<long>(nullable: false),
                    CurrencySMSCode = table.Column<string>(nullable: false),
                    BaseCurrencySMSCode = table.Column<string>(nullable: false),
                    RateType = table.Column<short>(nullable: false),
                    PairID = table.Column<long>(nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DailyLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    WeeklyLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTokenMasterMargin", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteTokenConversionMargin");

            migrationBuilder.DropTable(
                name: "SiteTokenMasterMargin");

            migrationBuilder.DropColumn(
                name: "ProfitAmount",
                table: "UserLoanMaster");

            migrationBuilder.DropColumn(
                name: "LoanID",
                table: "OpenPositionMaster");

            migrationBuilder.DropColumn(
                name: "LoanID",
                table: "MarginPNLAccount");
        }
    }
}
