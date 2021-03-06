using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class MarginTrading : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceMasterMargin",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    SMSCode = table.Column<string>(maxLength: 6, nullable: false),
                    ServiceType = table.Column<short>(nullable: false),
                    LimitId = table.Column<long>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMasterMargin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SettledTradeTransactionQueueMargin",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnNo = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    MemberID = table.Column<long>(nullable: false),
                    TrnType = table.Column<short>(nullable: false),
                    TrnTypeName = table.Column<string>(nullable: true),
                    PairID = table.Column<long>(nullable: false),
                    PairName = table.Column<string>(nullable: false),
                    OrderWalletID = table.Column<long>(nullable: false),
                    OrderAccountID = table.Column<string>(nullable: true),
                    DeliveryWalletID = table.Column<long>(nullable: false),
                    DeliveryAccountID = table.Column<string>(nullable: true),
                    BuyQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    AskPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Order_Currency = table.Column<string>(nullable: true),
                    OrderTotalQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Delivery_Currency = table.Column<string>(nullable: true),
                    DeliveryTotalQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    StatusCode = table.Column<long>(nullable: false),
                    StatusMsg = table.Column<string>(nullable: true),
                    TrnRefNo = table.Column<long>(nullable: true),
                    IsCancelled = table.Column<short>(nullable: false),
                    SettledBuyQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SettledSellQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SettledDate = table.Column<DateTime>(nullable: true),
                    TakerPer = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettledTradeTransactionQueueMargin", x => x.TrnNo);
                });

            migrationBuilder.CreateTable(
                name: "TradeBuyerListMarginV1",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnNo = table.Column<long>(nullable: false),
                    PairID = table.Column<long>(nullable: false),
                    PairName = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DeliveredQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    RemainQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    OrderType = table.Column<short>(nullable: false),
                    IsProcessing = table.Column<short>(nullable: false),
                    IsAPITrade = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeBuyerListMarginV1", x => x.TrnNo);
                });

            migrationBuilder.CreateTable(
                name: "TradePairDetailMargin",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    PairId = table.Column<long>(nullable: false),
                    BuyMinQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BuyMaxQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellMinQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellMaxQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BuyPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BuyMinPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BuyMaxPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellMinPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellMaxPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BuyFees = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellFees = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    FeesCurrency = table.Column<string>(nullable: false),
                    ChargeType = table.Column<short>(nullable: true),
                    OpenOrderExpiration = table.Column<long>(nullable: true),
                    IsMarketTicker = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePairDetailMargin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradePairMasterMargin",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    PairName = table.Column<string>(nullable: false),
                    SecondaryCurrencyId = table.Column<long>(nullable: false),
                    WalletMasterID = table.Column<long>(nullable: false),
                    BaseCurrencyId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePairMasterMargin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradePairStasticsMargin",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    PairId = table.Column<long>(nullable: false),
                    CurrentRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LTP = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChangePer24 = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChangeVol24 = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    High24Hr = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Low24Hr = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    HighWeek = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LowWeek = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    High52Week = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Low52Week = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CurrencyPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    UpDownBit = table.Column<short>(nullable: false),
                    TranDate = table.Column<DateTime>(nullable: false),
                    ChangeValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CronDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePairStasticsMargin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradePoolQueueMarginV1",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    PairID = table.Column<long>(nullable: false),
                    MakerTrnNo = table.Column<long>(nullable: false),
                    MakerType = table.Column<string>(nullable: true),
                    MakerPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MakerQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerTrnNo = table.Column<long>(nullable: false),
                    TakerType = table.Column<string>(nullable: true),
                    TakerPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerDisc = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerLoss = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    IsAPITrade = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePoolQueueMarginV1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeSellerListMarginV1",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnNo = table.Column<long>(nullable: false),
                    PairID = table.Column<long>(nullable: false),
                    PairName = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ReleasedQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SelledQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    RemainQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    OrderType = table.Column<short>(nullable: false),
                    IsProcessing = table.Column<short>(nullable: false),
                    IsAPITrade = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeSellerListMarginV1", x => x.TrnNo);
                });

            migrationBuilder.CreateTable(
                name: "TradeStopLossMargin",
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
                    ordertype = table.Column<short>(nullable: false),
                    StopPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LTP = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    RangeMin = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    RangeMax = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MarketIndicator = table.Column<short>(nullable: false),
                    PairID = table.Column<long>(nullable: false),
                    ISFollowersReq = table.Column<short>(nullable: false),
                    FollowingTo = table.Column<long>(nullable: false),
                    LeaderTrnNo = table.Column<long>(nullable: false),
                    FollowTradeType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeStopLossMargin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeTransactionQueueMargin",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnNo = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    MemberID = table.Column<long>(nullable: false),
                    TrnType = table.Column<short>(nullable: false),
                    TrnTypeName = table.Column<string>(nullable: true),
                    PairID = table.Column<long>(nullable: false),
                    PairName = table.Column<string>(nullable: false),
                    OrderWalletID = table.Column<long>(nullable: false),
                    OrderAccountID = table.Column<string>(nullable: true),
                    DeliveryWalletID = table.Column<long>(nullable: false),
                    DeliveryAccountID = table.Column<string>(nullable: true),
                    BuyQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    AskPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Order_Currency = table.Column<string>(nullable: true),
                    OrderTotalQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Delivery_Currency = table.Column<string>(nullable: true),
                    DeliveryTotalQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    StatusCode = table.Column<long>(nullable: false),
                    StatusMsg = table.Column<string>(nullable: true),
                    IsCancelled = table.Column<short>(nullable: false),
                    SettledBuyQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SettledSellQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SettledDate = table.Column<DateTime>(nullable: true),
                    ordertype = table.Column<short>(nullable: false),
                    IsAPITrade = table.Column<short>(nullable: false),
                    IsExpired = table.Column<short>(nullable: false),
                    APIStatus = table.Column<string>(nullable: true),
                    IsAPICancelled = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeTransactionQueueMargin", x => x.TrnNo);
                });

            migrationBuilder.CreateTable(
                name: "TransactionQueueMargin",
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
                    TrnDate = table.Column<DateTime>(nullable: false),
                    TrnMode = table.Column<short>(nullable: false),
                    TrnType = table.Column<short>(nullable: false),
                    MemberID = table.Column<long>(nullable: false),
                    MemberMobile = table.Column<string>(nullable: true),
                    SMSCode = table.Column<string>(maxLength: 10, nullable: false),
                    TransactionAccount = table.Column<string>(maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ServiceID = table.Column<long>(nullable: false),
                    SerProID = table.Column<long>(nullable: false),
                    ProductID = table.Column<long>(nullable: false),
                    RouteID = table.Column<long>(nullable: false),
                    StatusCode = table.Column<long>(nullable: false),
                    StatusMsg = table.Column<string>(nullable: true),
                    VerifyDone = table.Column<short>(nullable: false),
                    TrnRefNo = table.Column<string>(nullable: true),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    ChargePer = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    ChargeRs = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    ChargeType = table.Column<short>(nullable: true),
                    DebitAccountID = table.Column<string>(nullable: true),
                    IsVerified = table.Column<short>(nullable: false),
                    IsInternalTrn = table.Column<short>(nullable: false),
                    EmailSendDate = table.Column<DateTime>(nullable: false),
                    CallStatus = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionQueueMargin", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceMasterMargin");

            migrationBuilder.DropTable(
                name: "SettledTradeTransactionQueueMargin");

            migrationBuilder.DropTable(
                name: "TradeBuyerListMarginV1");

            migrationBuilder.DropTable(
                name: "TradePairDetailMargin");

            migrationBuilder.DropTable(
                name: "TradePairMasterMargin");

            migrationBuilder.DropTable(
                name: "TradePairStasticsMargin");

            migrationBuilder.DropTable(
                name: "TradePoolQueueMarginV1");

            migrationBuilder.DropTable(
                name: "TradeSellerListMarginV1");

            migrationBuilder.DropTable(
                name: "TradeStopLossMargin");

            migrationBuilder.DropTable(
                name: "TradeTransactionQueueMargin");

            migrationBuilder.DropTable(
                name: "TransactionQueueMargin");
        }
    }
}
