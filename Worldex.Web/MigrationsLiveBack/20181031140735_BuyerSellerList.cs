using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class BuyerSellerList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletLimitConfiguration",
                table: "WalletLimitConfiguration");

            //migrationBuilder.RenameColumn(
            //    name: "StartTime",
            //    table: "WalletLimitConfigurationMaster",
            //    newName: "StartTimeUnix");

            //migrationBuilder.RenameColumn(
            //    name: "EndTime",
            //    table: "WalletLimitConfigurationMaster",
            //    newName: "EndTimeUnix");

            migrationBuilder.AddColumn<short>(
                name: "IsProcessing",
                table: "TradeSellerList",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainQty",
                table: "TradeSellerList",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BidPrice",
                table: "TradeGraphDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "TradeGraphDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "IsProcessing",
                table: "TradeBuyerList",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletLimitConfiguration",
                table: "WalletLimitConfiguration",
                columns: new[] { "TrnType", "WalletId" });

            //migrationBuilder.CreateTable(
            //    name: "SettledTradeTransactionQueue",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: true),
            //        Status = table.Column<short>(nullable: false),
            //        TrnNo = table.Column<long>(nullable: false),
            //        TrnDate = table.Column<DateTime>(nullable: false),
            //        MemberID = table.Column<long>(nullable: false),
            //        TrnType = table.Column<short>(nullable: false),
            //        TrnTypeName = table.Column<string>(nullable: true),
            //        PairID = table.Column<long>(nullable: false),
            //        PairName = table.Column<string>(nullable: false),
            //        OrderWalletID = table.Column<long>(nullable: false),
            //        DeliveryWalletID = table.Column<long>(nullable: false),
            //        BuyQty = table.Column<decimal>(nullable: false),
            //        BidPrice = table.Column<decimal>(nullable: false),
            //        SellQty = table.Column<decimal>(nullable: false),
            //        AskPrice = table.Column<decimal>(nullable: false),
            //        Order_Currency = table.Column<string>(nullable: true),
            //        OrderTotalQty = table.Column<decimal>(nullable: false),
            //        Delivery_Currency = table.Column<string>(nullable: true),
            //        DeliveryTotalQty = table.Column<decimal>(nullable: false),
            //        StatusCode = table.Column<long>(nullable: false),
            //        StatusMsg = table.Column<string>(nullable: true),
            //        ServiceID = table.Column<long>(nullable: false),
            //        ProductID = table.Column<long>(nullable: false),
            //        SerProID = table.Column<long>(nullable: false),
            //        RoutID = table.Column<int>(nullable: false),
            //        TrnRefNo = table.Column<long>(nullable: true),
            //        IsCancelled = table.Column<short>(nullable: false),
            //        SettledBuyQty = table.Column<decimal>(nullable: false),
            //        SettledSellQty = table.Column<decimal>(nullable: false),
            //        SettledDate = table.Column<DateTime>(nullable: true),
            //        TakerPer = table.Column<decimal>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SettledTradeTransactionQueue", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "TradePoolConfiguration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    CountPerPrice = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePoolConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradePoolQueue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    PoolID = table.Column<long>(nullable: false),
                    SellerListID = table.Column<long>(nullable: false),
                    MakerTrnNo = table.Column<long>(nullable: false),
                    MakerQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TakerTrnNo = table.Column<long>(nullable: false),
                    TakerQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePoolQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionStatus",
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
                    ServiceID = table.Column<long>(nullable: false),
                    SerProID = table.Column<long>(nullable: false),
                    StatusMsg = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionStatus", x => new { x.TrnNo, x.ServiceID, x.SerProID });
                    table.UniqueConstraint("AK_TransactionStatus_SerProID_ServiceID_TrnNo", x => new { x.SerProID, x.ServiceID, x.TrnNo });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettledTradeTransactionQueue");

            migrationBuilder.DropTable(
                name: "TradePoolConfiguration");

            migrationBuilder.DropTable(
                name: "TradePoolQueue");

            migrationBuilder.DropTable(
                name: "TransactionStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletLimitConfiguration",
                table: "WalletLimitConfiguration");

            migrationBuilder.DropColumn(
                name: "IsProcessing",
                table: "TradeSellerList");

            migrationBuilder.DropColumn(
                name: "RemainQty",
                table: "TradeSellerList");

            migrationBuilder.DropColumn(
                name: "BidPrice",
                table: "TradeGraphDetail");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "TradeGraphDetail");

            migrationBuilder.DropColumn(
                name: "IsProcessing",
                table: "TradeBuyerList");

            //migrationBuilder.RenameColumn(
            //    name: "StartTimeUnix",
            //    table: "WalletLimitConfigurationMaster",
            //    newName: "StartTime");

            //migrationBuilder.RenameColumn(
            //    name: "EndTimeUnix",
            //    table: "WalletLimitConfigurationMaster",
            //    newName: "EndTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletLimitConfiguration",
                table: "WalletLimitConfiguration",
                column: "Id");
        }
    }
}
