using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddTopUpEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArbitrageDepositFund",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TrnID = table.Column<string>(maxLength: 1000, nullable: true),
                    SMSCode = table.Column<string>(maxLength: 50, nullable: false),
                    FromSerProId = table.Column<long>(nullable: false),
                    ToserProId = table.Column<long>(nullable: false),
                    Address = table.Column<string>(maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    IsProcessing = table.Column<short>(nullable: false),
                    ToAddress = table.Column<string>(maxLength: 50, nullable: false),
                    SystemRemarks = table.Column<string>(maxLength: 100, nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    RouteTag = table.Column<string>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    ProviderWalletID = table.Column<string>(maxLength: 50, nullable: false),
                    ApprovedBy = table.Column<long>(nullable: false),
                    ApprovedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageDepositFund", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArbitrageThirdPartyAPIConfiguration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    APIName = table.Column<string>(maxLength: 30, nullable: false),
                    APISendURL = table.Column<string>(nullable: false),
                    APIValidateURL = table.Column<string>(nullable: true),
                    APIBalURL = table.Column<string>(nullable: true),
                    APIStatusCheckURL = table.Column<string>(nullable: true),
                    APIRequestBody = table.Column<string>(nullable: true),
                    BalCheckRequestBody = table.Column<string>(nullable: true),
                    TransactionIdPrefix = table.Column<string>(nullable: true),
                    MerchantCode = table.Column<string>(nullable: true),
                    ResponseSuccess = table.Column<string>(nullable: true),
                    ResponseFailure = table.Column<string>(nullable: true),
                    ResponseHold = table.Column<string>(nullable: true),
                    AuthHeader = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    MethodType = table.Column<string>(nullable: true),
                    BalCheckMethodType = table.Column<string>(nullable: true),
                    HashCode = table.Column<string>(nullable: true),
                    HashCodeRecheck = table.Column<string>(nullable: true),
                    HashType = table.Column<short>(nullable: false),
                    AppType = table.Column<short>(nullable: false),
                    ParsingDataID = table.Column<long>(nullable: false),
                    TimeStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageThirdPartyAPIConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArbitrageThirdPartyAPIResponseConfiguration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    BalanceRegex = table.Column<string>(nullable: true),
                    StatusRegex = table.Column<string>(nullable: true),
                    StatusMsgRegex = table.Column<string>(nullable: true),
                    ResponseCodeRegex = table.Column<string>(nullable: true),
                    ErrorCodeRegex = table.Column<string>(nullable: true),
                    TrnRefNoRegex = table.Column<string>(nullable: true),
                    OprTrnRefNoRegex = table.Column<string>(nullable: true),
                    Param1Regex = table.Column<string>(nullable: true),
                    Param2Regex = table.Column<string>(nullable: true),
                    Param3Regex = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageThirdPartyAPIResponseConfiguration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbitrageDepositFund");

            migrationBuilder.DropTable(
                name: "ArbitrageThirdPartyAPIConfiguration");

            migrationBuilder.DropTable(
                name: "ArbitrageThirdPartyAPIResponseConfiguration");
        }
    }
}
