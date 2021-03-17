using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CleanArchitecture.Web.Migrations
{
    public partial class DecimalPointChangeV4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Volume",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "TodayOpen",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "TodayClose",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Quantity",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "LowWeek",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Low52Week",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Low24Hr",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "LTP",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "HighWeek",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "High52Week",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "High24Hr",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "ChangePer",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "BidPrice",
            //    table: "TradeGraphDetail",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18, 8)");
            migrationBuilder.AlterColumn<decimal>(
                name: "PendingBuyQty",
                table: "TradeCancelQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliverQty",
                table: "TradeCancelQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliverBidPrice",
                table: "TradeCancelQueue",
                type: "decimal(28, 18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "TradeBuyRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PendingQty",
                table: "TradeBuyRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidQty",
                table: "TradeBuyRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveredQty",
                table: "TradeBuyRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "TradeBuyRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RemainQty",
                table: "TradeBuyerListV1",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "TradeBuyerListV1",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TradeBuyerListV1",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveredQty",
                table: "TradeBuyerListV1",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "TradeBuyerList",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TradeBuyerList",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveredQty",
                table: "TradeBuyerList",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestCreditedValue",
                table: "TokenUnStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChargeBeforeMaturity",
                table: "TokenUnStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountCredited",
                table: "TokenUnStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerCharges",
                table: "TokenStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StakingAmount",
                table: "TokenStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "TokenStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "TokenStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MakerCharges",
                table: "TokenStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestValueMst",
                table: "TokenStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestValue",
                table: "TokenStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EnableStakingBeforeMaturityCharge",
                table: "TokenStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeductionAmount",
                table: "TokenStakingHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StopLossPer",
                table: "StopLossMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinLimitAmount",
                table: "StckingScheme",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxLimitAmount",
                table: "StckingScheme",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerCharges",
                table: "StakingPolicyDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "StakingPolicyDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "StakingPolicyDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MakerCharges",
                table: "StakingPolicyDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestValue",
                table: "StakingPolicyDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EnableStakingBeforeMaturityCharge",
                table: "StakingPolicyDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerCharge",
                table: "StakingChargeMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MakerCharge",
                table: "StakingChargeMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerPer",
                table: "SettledTradeTransactionQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledSellQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledBuyQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderTotalQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryTotalQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "SettledTradeTransactionQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AskPrice",
                table: "SettledTradeTransactionQueue",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "VolGlobal",
                table: "ServiceStastics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarketCap",
                table: "ServiceStastics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Withdrawalfee",
                table: "ProfileMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Tradingfee",
                table: "ProfileMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SubscriptionAmount",
                table: "ProfileMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProfileFree",
                table: "ProfileMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DepositFee",
                table: "ProfileMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderAmt",
                table: "PoolOrder",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscRs",
                table: "PoolOrder",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscPer",
                table: "PoolOrder",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryAmt",
                table: "PoolOrder",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ShadowLimitAmount",
                table: "MemberShadowLimit",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ShadowAmount",
                table: "MemberShadowBalance",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmtWeekly",
                table: "Limits",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmtMonthly",
                table: "Limits",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmtDaily",
                table: "Limits",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmt",
                table: "Limits",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmtWeekly",
                table: "Limits",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmtMonthly",
                table: "Limits",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmtDaily",
                table: "Limits",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmt",
                table: "Limits",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "LimitRuleMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "LimitRuleMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LeveragePer",
                table: "LeverageMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "DepositHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentRate",
                table: "CurrencyRateMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume_24h",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_7d",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_24h",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_1h",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Market_cap",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SourcePrice",
                table: "ConvertFundHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "ConvertFundHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DestinationPrice",
                table: "ConvertFundHistory",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "CommissionPolicy",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "CommissionPolicy",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TrnFee",
                table: "CoinListRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalSupply",
                table: "CoinListRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxSupply",
                table: "CoinListRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "IssuePrice",
                table: "CoinListRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CirculatingSupply",
                table: "CoinListRequest",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "ChargeRuleMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "ChargeRuleMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChargeValue",
                table: "ChargeRuleMaster",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "ChargePolicy",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "ChargePolicy",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerCharge",
                table: "ChargeConfigurationDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "ChargeConfigurationDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "ChargeConfigurationDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MakerCharge",
                table: "ChargeConfigurationDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChargeValue",
                table: "ChargeConfigurationDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StartingBalance",
                table: "BalanceStatistics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EndingBalance",
                table: "BalanceStatistics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.CreateTable(
                name: "UserProfitStatistics",
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
                    StartingBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    EndingBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DepositionAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    WithdrawAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ProfitAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ProfitPer = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfitStatistics", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Volume",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "TodayOpen",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "TodayClose",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Quantity",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "LowWeek",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Low52Week",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Low24Hr",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "LTP",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "HighWeek",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "High52Week",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "High24Hr",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "ChangePer",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "BidPrice",
            //    table: "TradeGraphDetail",
            //    type: "decimal(18, 8)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(28, 18)");   
            migrationBuilder.AlterColumn<decimal>(
                name: "PendingBuyQty",
                table: "TradeCancelQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliverQty",
                table: "TradeCancelQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliverBidPrice",
                table: "TradeCancelQueue",
                type: "decimal(18, 8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "TradeBuyRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PendingQty",
                table: "TradeBuyRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidQty",
                table: "TradeBuyRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveredQty",
                table: "TradeBuyRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "TradeBuyRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RemainQty",
                table: "TradeBuyerListV1",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "TradeBuyerListV1",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TradeBuyerListV1",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveredQty",
                table: "TradeBuyerListV1",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "TradeBuyerList",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TradeBuyerList",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveredQty",
                table: "TradeBuyerList",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestCreditedValue",
                table: "TokenUnStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChargeBeforeMaturity",
                table: "TokenUnStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountCredited",
                table: "TokenUnStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerCharges",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StakingAmount",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MakerCharges",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestValueMst",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestValue",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EnableStakingBeforeMaturityCharge",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeductionAmount",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StopLossPer",
                table: "StopLossMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinLimitAmount",
                table: "StckingScheme",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxLimitAmount",
                table: "StckingScheme",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerCharges",
                table: "StakingPolicyDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "StakingPolicyDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "StakingPolicyDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MakerCharges",
                table: "StakingPolicyDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestValue",
                table: "StakingPolicyDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EnableStakingBeforeMaturityCharge",
                table: "StakingPolicyDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerCharge",
                table: "StakingChargeMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MakerCharge",
                table: "StakingChargeMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerPer",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledSellQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SettledBuyQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderTotalQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryTotalQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyQty",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AskPrice",
                table: "SettledTradeTransactionQueue",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "VolGlobal",
                table: "ServiceStastics",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarketCap",
                table: "ServiceStastics",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Withdrawalfee",
                table: "ProfileMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Tradingfee",
                table: "ProfileMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SubscriptionAmount",
                table: "ProfileMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProfileFree",
                table: "ProfileMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DepositFee",
                table: "ProfileMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderAmt",
                table: "PoolOrder",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscRs",
                table: "PoolOrder",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscPer",
                table: "PoolOrder",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryAmt",
                table: "PoolOrder",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ShadowLimitAmount",
                table: "MemberShadowLimit",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ShadowAmount",
                table: "MemberShadowBalance",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmtWeekly",
                table: "Limits",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmtMonthly",
                table: "Limits",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmtDaily",
                table: "Limits",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmt",
                table: "Limits",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmtWeekly",
                table: "Limits",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmtMonthly",
                table: "Limits",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmtDaily",
                table: "Limits",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmt",
                table: "Limits",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "LimitRuleMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "LimitRuleMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LeveragePer",
                table: "LeverageMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "DepositHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentRate",
                table: "CurrencyRateMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume_24h",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CurrencyRateDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_7d",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_24h",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_1h",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Market_cap",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SourcePrice",
                table: "ConvertFundHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "ConvertFundHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DestinationPrice",
                table: "ConvertFundHistory",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "CommissionPolicy",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "CommissionPolicy",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TrnFee",
                table: "CoinListRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalSupply",
                table: "CoinListRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxSupply",
                table: "CoinListRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "IssuePrice",
                table: "CoinListRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CirculatingSupply",
                table: "CoinListRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "ChargeRuleMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "ChargeRuleMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChargeValue",
                table: "ChargeRuleMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "ChargePolicy",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "ChargePolicy",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TakerCharge",
                table: "ChargeConfigurationDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinAmount",
                table: "ChargeConfigurationDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxAmount",
                table: "ChargeConfigurationDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MakerCharge",
                table: "ChargeConfigurationDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChargeValue",
                table: "ChargeConfigurationDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StartingBalance",
                table: "BalanceStatistics",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EndingBalance",
                table: "BalanceStatistics",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");
        }
    }
}
