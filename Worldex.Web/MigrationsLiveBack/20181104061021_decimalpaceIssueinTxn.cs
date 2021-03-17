using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class decimalpaceIssueinTxn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQty",
                table: "TradeTransactionStatus",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "SoldPrice",
                table: "TradeTransactionStatus",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledQty",
                table: "TradeTransactionStatus",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "PendingQty",
                table: "TradeTransactionStatus",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveredQty",
                table: "TradeTransactionStatus",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "TradeTransactionStatus",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerPer",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledSellQty",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledBuyQty",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "SellQty",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderTotalQty",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryTotalQty",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyQty",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "AskPrice",
                table: "TradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<decimal>(
                name: "StopPrice",
                table: "TradeStopLoss",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "PendingBuyQty",
                table: "TradeCancelQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliverQty",
                table: "TradeCancelQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliverBidPrice",
                table: "TradeCancelQueue",
                type: "decimal(18, 8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledSellQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledBuyQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "SellQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderTotalQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryTotalQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "AskPrice",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            //migrationBuilder.CreateTable(
            //    name: "CompainTrail",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        ComplainId = table.Column<long>(nullable: false),
            //        Description = table.Column<string>(maxLength: 4000, nullable: false),
            //        Remark = table.Column<string>(maxLength: 2000, nullable: false),
            //        Complainstatus = table.Column<string>(maxLength: 100, nullable: false),
            //        CreatedBy = table.Column<long>(nullable: true),
            //        CreatedDate = table.Column<DateTime>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CompainTrail", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Complainmaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: true),
            //        Status = table.Column<short>(nullable: false),
            //        UserID = table.Column<int>(nullable: false),
            //        TypeId = table.Column<long>(nullable: false),
            //        Subject = table.Column<string>(maxLength: 500, nullable: false),
            //        Description = table.Column<string>(maxLength: 4000, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Complainmaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProfileMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: true),
            //        Status = table.Column<short>(nullable: false),
            //        TypeId = table.Column<long>(nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        Description = table.Column<string>(maxLength: 2000, nullable: false),
            //        Level = table.Column<int>(nullable: false),
            //        LevelName = table.Column<string>(maxLength: 150, nullable: false),
            //        DepositFee = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        Withdrawalfee = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        Tradingfee = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        WithdrawalLimit = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        EnableStatus = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProfileMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Typemaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: true),
            //        Status = table.Column<short>(nullable: false),
            //        Type = table.Column<string>(maxLength: 100, nullable: false),
            //        SubType = table.Column<string>(maxLength: 150, nullable: false),
            //        EnableStatus = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Typemaster", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "CompainTrail");

            //migrationBuilder.DropTable(
            //    name: "Complainmaster");

            //migrationBuilder.DropTable(
            //    name: "ProfileMaster");

            //migrationBuilder.DropTable(
            //    name: "Typemaster");

            migrationBuilder.DropColumn(
                name: "StopPrice",
                table: "TradeStopLoss");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQty",
                table: "TradeTransactionStatus",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SoldPrice",
                table: "TradeTransactionStatus",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledQty",
                table: "TradeTransactionStatus",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PendingQty",
                table: "TradeTransactionStatus",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveredQty",
                table: "TradeTransactionStatus",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "TradeTransactionStatus",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerPer",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledSellQty",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledBuyQty",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellQty",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderTotalQty",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryTotalQty",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyQty",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AskPrice",
                table: "TradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PendingBuyQty",
                table: "TradeCancelQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliverQty",
                table: "TradeCancelQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliverBidPrice",
                table: "TradeCancelQueue",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledSellQty",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledBuyQty",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellQty",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderTotalQty",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryTotalQty",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyQty",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AskPrice",
                table: "SettledTradeTransactionQueue",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");
        }
    }
}
