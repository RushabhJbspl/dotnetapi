using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addSellTopUpRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Platform",
                table: "FiatTradeConfigurationMaster",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellCallBackURL",
                table: "FiatTradeConfigurationMaster",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WithdrawURL",
                table: "FiatTradeConfigurationMaster",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SellTopUpRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Guid = table.Column<string>(nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CoinRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    FiatConverationRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    FromCurrency = table.Column<string>(nullable: false),
                    ToCurrency = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    TransactionHash = table.Column<string>(nullable: false),
                    NotifyUrl = table.Column<string>(nullable: false),
                    TransactionId = table.Column<string>(nullable: false),
                    TransactionCode = table.Column<string>(nullable: false),
                    UserGuid = table.Column<string>(nullable: false),
                    Platform = table.Column<string>(nullable: false),
                    Type = table.Column<short>(nullable: false),
                    FromBankId = table.Column<long>(nullable: false),
                    ToBankId = table.Column<long>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    BankName = table.Column<string>(nullable: true),
                    CurrencyName = table.Column<string>(nullable: true),
                    BankId = table.Column<string>(nullable: true),
                    CurrencyId = table.Column<string>(nullable: true),
                    user_bank_name = table.Column<string>(nullable: true),
                    user_bank_account_number = table.Column<string>(nullable: true),
                    user_bank_acount_holder_name = table.Column<string>(nullable: true),
                    user_currency_code = table.Column<string>(nullable: true),
                    payus_transaction_id = table.Column<string>(nullable: true),
                    payus_amount_usd = table.Column<decimal>(nullable: false),
                    payus_amount_crypto = table.Column<decimal>(nullable: false),
                    payus_mining_fees = table.Column<decimal>(nullable: false),
                    payus_total_payable_amount = table.Column<decimal>(nullable: false),
                    payus_fees = table.Column<decimal>(nullable: false),
                    payus_total_fees = table.Column<decimal>(nullable: false),
                    payus_usd_rate = table.Column<decimal>(nullable: false),
                    payus_expire_datetime = table.Column<DateTime>(nullable: false),
                    payus_payment_tracking = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellTopUpRequest", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellTopUpRequest");

            migrationBuilder.DropColumn(
                name: "Platform",
                table: "FiatTradeConfigurationMaster");

            migrationBuilder.DropColumn(
                name: "SellCallBackURL",
                table: "FiatTradeConfigurationMaster");

            migrationBuilder.DropColumn(
                name: "WithdrawURL",
                table: "FiatTradeConfigurationMaster");
        }
    }
}
