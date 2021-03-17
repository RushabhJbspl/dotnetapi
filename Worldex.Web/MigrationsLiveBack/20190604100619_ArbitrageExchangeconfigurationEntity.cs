using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ArbitrageExchangeconfigurationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LimitsArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    MinAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinAmtDaily = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxAmtDaily = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinAmtWeekly = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxAmtWeekly = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinAmtMonthly = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxAmtMonthly = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinRange = table.Column<long>(nullable: false),
                    Maxrange = table.Column<long>(nullable: false),
                    MinRangeDaily = table.Column<long>(nullable: false),
                    MaxRangeDaily = table.Column<long>(nullable: false),
                    MinRangeWeekly = table.Column<long>(nullable: false),
                    MaxRangeWeekly = table.Column<long>(nullable: false),
                    MinRangeMonthly = table.Column<long>(nullable: false),
                    MaxRangeMonthly = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitsArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteConfigurationArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    RouteName = table.Column<string>(maxLength: 30, nullable: false),
                    ServiceID = table.Column<long>(nullable: false),
                    SerProDetailID = table.Column<long>(nullable: false),
                    ProductID = table.Column<long>(nullable: false),
                    Priority = table.Column<short>(nullable: false),
                    StatusCheckUrl = table.Column<string>(nullable: true),
                    ValidationUrl = table.Column<string>(nullable: true),
                    TransactionUrl = table.Column<string>(nullable: true),
                    LimitId = table.Column<long>(nullable: false),
                    OpCode = table.Column<string>(maxLength: 50, nullable: true),
                    TrnType = table.Column<int>(nullable: false),
                    IsDelayAddress = table.Column<byte>(nullable: false),
                    ProviderWalletID = table.Column<string>(maxLength: 100, nullable: true),
                    ConvertAmount = table.Column<decimal>(type: "decimal(22, 2)", nullable: false),
                    ConfirmationCount = table.Column<int>(nullable: false),
                    OrderType = table.Column<long>(nullable: false),
                    PairId = table.Column<long>(nullable: false),
                    GasLimit = table.Column<long>(nullable: false),
                    AccountNoLen = table.Column<int>(nullable: false),
                    AccNoStartsWith = table.Column<string>(maxLength: 30, nullable: true),
                    AccNoValidationRegex = table.Column<string>(maxLength: 80, nullable: true),
                    ContractAddress = table.Column<string>(maxLength: 100, nullable: true),
                    IsAdminApprovalRequired = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteConfigurationArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProConfigurationArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    AppKey = table.Column<string>(maxLength: 50, nullable: false),
                    APIKey = table.Column<string>(maxLength: 100, nullable: false),
                    SecretKey = table.Column<string>(maxLength: 100, nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 50, nullable: false),
                    Param1 = table.Column<string>(nullable: true),
                    Param2 = table.Column<string>(nullable: true),
                    Param3 = table.Column<string>(nullable: true),
                    Param4 = table.Column<string>(nullable: true),
                    Param5 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProConfigurationArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviderDetailArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ServiceProID = table.Column<long>(nullable: false),
                    ProTypeID = table.Column<long>(nullable: false),
                    AppTypeID = table.Column<long>(nullable: false),
                    TrnTypeID = table.Column<long>(nullable: false),
                    LimitID = table.Column<long>(nullable: false),
                    DemonConfigID = table.Column<long>(nullable: false),
                    ServiceProConfigID = table.Column<long>(nullable: false),
                    ThirPartyAPIID = table.Column<long>(nullable: false),
                    SerProDetailName = table.Column<string>(nullable: true),
                    IsStopConvertAmount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderDetailArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviderMasterArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ProviderName = table.Column<string>(maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderMasterArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviderTypeArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ServiveProTypeName = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderTypeArbitrage", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LimitsArbitrage");

            migrationBuilder.DropTable(
                name: "RouteConfigurationArbitrage");

            migrationBuilder.DropTable(
                name: "ServiceProConfigurationArbitrage");

            migrationBuilder.DropTable(
                name: "ServiceProviderDetailArbitrage");

            migrationBuilder.DropTable(
                name: "ServiceProviderMasterArbitrage");

            migrationBuilder.DropTable(
                name: "ServiceProviderTypeArbitrage");
        }
    }
}
