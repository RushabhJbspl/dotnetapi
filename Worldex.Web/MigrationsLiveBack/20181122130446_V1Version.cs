using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class V1Version : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradeBuyerListV1",
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
                    Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    DeliveredQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    RemainQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    OrderType = table.Column<short>(nullable: false),
                    IsProcessing = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeBuyerListV1", x => x.TrnNo);
                });

            migrationBuilder.CreateTable(
                name: "TradePoolQueueV1",
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
                    MakerPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MakerQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TakerTrnNo = table.Column<long>(nullable: false),
                    TakerType = table.Column<string>(nullable: true),
                    TakerPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TakerQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TakerDisc = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TakerLoss = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePoolQueueV1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeSellerListV1",
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
                    Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    ReleasedQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    SelledQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    RemainQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    OrderType = table.Column<short>(nullable: false),
                    IsProcessing = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeSellerListV1", x => x.TrnNo);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeBuyerListV1");

            migrationBuilder.DropTable(
                name: "TradePoolQueueV1");

            migrationBuilder.DropTable(
                name: "TradeSellerListV1");
        }
    }
}
