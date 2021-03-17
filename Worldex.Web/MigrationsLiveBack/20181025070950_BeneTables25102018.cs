using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class BeneTables25102018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyPrice",
                table: "TradePairDetail");

            migrationBuilder.DropColumn(
                name: "Currentrate",
                table: "TradePairDetail");

            migrationBuilder.DropColumn(
                name: "DailyHigh",
                table: "TradePairDetail");

            migrationBuilder.DropColumn(
                name: "DailyLow",
                table: "TradePairDetail");

            migrationBuilder.DropColumn(
                name: "FeeType",
                table: "TradePairDetail");

            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "TradePairDetail",
                newName: "SellFees");

            migrationBuilder.RenameColumn(
                name: "Fee",
                table: "TradePairDetail",
                newName: "BuyFees");

            migrationBuilder.RenameColumn(
                name: "Low",
                table: "TradeGraphDetail",
                newName: "LowWeek");

            migrationBuilder.RenameColumn(
                name: "High",
                table: "TradeGraphDetail",
                newName: "Low52Week");

            migrationBuilder.AddColumn<string>(
                name: "FeesCurrency",
                table: "TradePairDetail",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "High24Hr",
                table: "TradeGraphDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "High52Week",
                table: "TradeGraphDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HighWeek",
                table: "TradeGraphDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LTP",
                table: "TradeGraphDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Low24Hr",
                table: "TradeGraphDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "BeneficiaryMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    WalletTypeID = table.Column<long>(nullable: false),
                    IsWhiteListed = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiaryMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Market",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: false),
                    isBaseCurrency = table.Column<short>(nullable: false),
                    ServiceID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Market", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradePairStastics",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    PairId = table.Column<long>(nullable: false),
                    CurrentRate = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    LTP = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    ChangePer24 = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    ChangeVol24 = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    High24Hr = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Low24Hr = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    HighWeek = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    LowWeek = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    High52Week = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Low52Week = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    CurrencyPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    UpDownBit = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePairStastics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferencesMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    IsWhitelisting = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferencesMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeneficiaryMaster");

            migrationBuilder.DropTable(
                name: "Market");

            migrationBuilder.DropTable(
                name: "TradePairStastics");

            migrationBuilder.DropTable(
                name: "UserPreferencesMaster");

            migrationBuilder.DropColumn(
                name: "FeesCurrency",
                table: "TradePairDetail");

            migrationBuilder.DropColumn(
                name: "High24Hr",
                table: "TradeGraphDetail");

            migrationBuilder.DropColumn(
                name: "High52Week",
                table: "TradeGraphDetail");

            migrationBuilder.DropColumn(
                name: "HighWeek",
                table: "TradeGraphDetail");

            migrationBuilder.DropColumn(
                name: "LTP",
                table: "TradeGraphDetail");

            migrationBuilder.DropColumn(
                name: "Low24Hr",
                table: "TradeGraphDetail");

            migrationBuilder.RenameColumn(
                name: "SellFees",
                table: "TradePairDetail",
                newName: "Volume");

            migrationBuilder.RenameColumn(
                name: "BuyFees",
                table: "TradePairDetail",
                newName: "Fee");

            migrationBuilder.RenameColumn(
                name: "LowWeek",
                table: "TradeGraphDetail",
                newName: "Low");

            migrationBuilder.RenameColumn(
                name: "Low52Week",
                table: "TradeGraphDetail",
                newName: "High");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyPrice",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Currentrate",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyHigh",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyLow",
                table: "TradePairDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "FeeType",
                table: "TradePairDetail",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
