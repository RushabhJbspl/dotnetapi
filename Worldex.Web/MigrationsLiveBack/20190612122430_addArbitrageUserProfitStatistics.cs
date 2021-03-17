using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addArbitrageUserProfitStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArbitrageUserProfitStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: false),
                    USDStartingBalance = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    StartingBalance = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    EndingBalance = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    USDEndingBalance = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    DepositionAmount = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    USDDepositionAmount = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    WithdrawAmount = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    USDWithdrawAmount = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    ProfitAmount = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    USDProfitAmount = table.Column<decimal>(type: "decimal(35, 18)", nullable: false),
                    ProfitPer = table.Column<decimal>(type: "decimal(35, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageUserProfitStatistics", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbitrageUserProfitStatistics");
        }
    }
}
