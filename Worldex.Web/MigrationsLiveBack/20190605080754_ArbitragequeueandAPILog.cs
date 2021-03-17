using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ArbitragequeueandAPILog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIOrderSettlementArbitrage",
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
                    OldStatus = table.Column<short>(nullable: false),
                    NewStatus = table.Column<short>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    OldQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    NewQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SettledQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    APIPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    APISettledQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    APIRemainQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIOrderSettlementArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradePoolQueueArbitrageV1",
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
                    IsAPITrade = table.Column<short>(nullable: false),
                    StatusCode = table.Column<long>(nullable: false),
                    StatusMsg = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePoolQueueArbitrageV1", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIOrderSettlementArbitrage");

            migrationBuilder.DropTable(
                name: "TradePoolQueueArbitrageV1");
        }
    }
}
