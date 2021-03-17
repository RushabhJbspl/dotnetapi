using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class marketcapentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyRateDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    CurrencyRateMasterId = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Volume_24h = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Market_cap = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Percent_change_1h = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Percent_change_24h = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Percent_change_7d = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Last_updated = table.Column<string>(nullable: false),
                    CoinName = table.Column<string>(nullable: false),
                    Symbol = table.Column<string>(nullable: false),
                    Last_updatedDateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRateDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketCapCounterMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    RecordCountLimit = table.Column<long>(nullable: false),
                    MaxLimit = table.Column<long>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    TPSPickupStatus = table.Column<long>(nullable: false),
                    StartLimit = table.Column<long>(nullable: false),
                    Url = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketCapCounterMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyRateDetail");

            migrationBuilder.DropTable(
                name: "MarketCapCounterMaster");
        }
    }
}
