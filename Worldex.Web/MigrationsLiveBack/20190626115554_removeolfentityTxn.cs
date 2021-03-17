using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class removeolfentityTxn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeBuyerList");

            migrationBuilder.DropTable(
                name: "TradePoolMaster");

            migrationBuilder.DropTable(
                name: "TradePoolQueue");

            migrationBuilder.DropTable(
                name: "TradeSellerList");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradeBuyerList",
                columns: table => new
                {
                    TrnNo = table.Column<long>(nullable: false),
                    BuyReqID = table.Column<long>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DeliveredQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsProcessing = table.Column<short>(nullable: false),
                    PaidServiceID = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ServiceID = table.Column<long>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeBuyerList", x => x.TrnNo);
                });

            migrationBuilder.CreateTable(
                name: "TradePoolMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SellServiceID = table.Column<long>(nullable: false),
                    BuyServiceID = table.Column<long>(nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    CountPerPrice = table.Column<long>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    GUID = table.Column<Guid>(nullable: false),
                    IsSleepMode = table.Column<short>(nullable: false),
                    Landing = table.Column<decimal>(type: "decimal(37, 16)", nullable: false),
                    OnProcessing = table.Column<short>(nullable: false),
                    PairId = table.Column<long>(nullable: false),
                    PairName = table.Column<string>(maxLength: 50, nullable: false),
                    ProductID = table.Column<long>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    TPSPickupStatus = table.Column<short>(nullable: false),
                    TotalQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePoolMaster", x => new { x.Id, x.SellServiceID, x.BuyServiceID, x.BidPrice });
                    table.UniqueConstraint("AK_TradePoolMaster_BidPrice_BuyServiceID_CountPerPrice_Id_SellServiceID", x => new { x.BidPrice, x.BuyServiceID, x.CountPerPrice, x.Id, x.SellServiceID });
                });

            migrationBuilder.CreateTable(
                name: "TradePoolQueue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    MakerPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MakerQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MakerTrnNo = table.Column<long>(nullable: false),
                    PoolID = table.Column<long>(nullable: false),
                    SellerListID = table.Column<long>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    TakerDisc = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerLoss = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerTrnNo = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePoolQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeSellerList",
                columns: table => new
                {
                    TrnNo = table.Column<long>(nullable: false),
                    PoolID = table.Column<long>(nullable: false),
                    BuyServiceID = table.Column<long>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsProcessing = table.Column<short>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    RemainQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellServiceID = table.Column<long>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeSellerList", x => new { x.TrnNo, x.PoolID });
                    table.UniqueConstraint("AK_TradeSellerList_PoolID_TrnNo", x => new { x.PoolID, x.TrnNo });
                });
        }
    }
}
