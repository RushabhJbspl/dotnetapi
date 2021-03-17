using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class APINewColumnsInTQ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "APIStatus",
                table: "TradeTransactionQueue",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "IsAPICancelled",
                table: "TradeTransactionQueue",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsAPITrade",
                table: "TradeTransactionQueue",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsExpired",
                table: "TradeTransactionQueue",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsAPITrade",
                table: "TradeSellerListV1",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsAPITrade",
                table: "TradeBuyerListV1",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "WalletTrnLimitConfiguration",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnType = table.Column<long>(nullable: false),
                    WalletType = table.Column<long>(nullable: false),
                    StartTime = table.Column<double>(nullable: false),
                    EndTime = table.Column<double>(nullable: false),
                    DailyTrnCount = table.Column<long>(nullable: false),
                    DailyTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MonthlyTrnCount = table.Column<long>(nullable: false),
                    MonthlyTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    WeeklyTrnCount = table.Column<long>(nullable: false),
                    WeeklyTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    YearlyTrnCount = table.Column<long>(nullable: false),
                    YearlyTrnAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTrnLimitConfiguration", x => new { x.TrnType, x.WalletType });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletTrnLimitConfiguration");

            migrationBuilder.DropColumn(
                name: "APIStatus",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "IsAPICancelled",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "IsAPITrade",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "IsExpired",
                table: "TradeTransactionQueue");

            migrationBuilder.DropColumn(
                name: "IsAPITrade",
                table: "TradeSellerListV1");

            migrationBuilder.DropColumn(
                name: "IsAPITrade",
                table: "TradeBuyerListV1");
        }
    }
}
