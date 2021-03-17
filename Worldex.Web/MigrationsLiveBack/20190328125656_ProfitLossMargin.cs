using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ProfitLossMargin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarginPNLAccount",
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
                    WTrnNo = table.Column<long>(nullable: false),
                    OpenPositionMasterID = table.Column<long>(nullable: false),
                    SettledQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    AvgLandingBuy = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    AvgLandingSell = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ProfitAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    ProfitCurrencyName = table.Column<string>(nullable: true),
                    ProfitWalletID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginPNLAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenPositionDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    OpenPositionMasterID = table.Column<long>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LandingPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CurrencyName = table.Column<string>(nullable: false),
                    TrnType = table.Column<short>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    WTrnNo = table.Column<long>(nullable: false),
                    SystemRemarks = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenPositionDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenPositionMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PairID = table.Column<long>(nullable: false),
                    BatchNo = table.Column<Guid>(nullable: false),
                    UserID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenPositionMaster", x => new { x.PairID, x.UserID, x.BatchNo });
                    table.UniqueConstraint("AK_OpenPositionMaster_BatchNo_PairID_UserID", x => new { x.BatchNo, x.PairID, x.UserID });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarginPNLAccount");

            migrationBuilder.DropTable(
                name: "OpenPositionDetail");

            migrationBuilder.DropTable(
                name: "OpenPositionMaster");
        }
    }
}
