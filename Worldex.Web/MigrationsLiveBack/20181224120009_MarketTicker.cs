using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class MarketTicker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsMarketTicker",
                table: "TradePairDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StakingChargeMaster",
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
                    UserID = table.Column<long>(nullable: false),
                    StakingPolicyID = table.Column<long>(nullable: false),
                    MakerCharge = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TakerCharge = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    StakingHistoryID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StakingChargeMaster", x => new { x.WalletTypeID, x.UserID });
                    table.UniqueConstraint("AK_StakingChargeMaster_UserID_WalletTypeID", x => new { x.UserID, x.WalletTypeID });
                });

            migrationBuilder.CreateTable(
                name: "StakingPolicyMaster",
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
                    MinAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    DiscType = table.Column<short>(nullable: false),
                    StakingType = table.Column<short>(nullable: false),
                    StakingDurationWeek = table.Column<short>(nullable: false),
                    StakingDurationMonth = table.Column<short>(nullable: false),
                    InterestType = table.Column<short>(nullable: false),
                    InterestValue = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    InterestWalletTypeID = table.Column<long>(nullable: false),
                    MakerCharges = table.Column<decimal>(nullable: false),
                    TakerCharges = table.Column<decimal>(nullable: false),
                    EnableAutoUnstaking = table.Column<short>(nullable: false),
                    EnableStakingBeforeMaturity = table.Column<short>(nullable: false),
                    EnableStakingBeforeMaturityCharge = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StakingPolicyMaster", x => x.WalletTypeID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StakingChargeMaster");

            migrationBuilder.DropTable(
                name: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "IsMarketTicker",
                table: "TradePairDetail");
        }
    }
}
