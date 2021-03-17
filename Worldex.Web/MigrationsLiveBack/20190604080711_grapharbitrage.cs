using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class grapharbitrage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradeGraphDetailArbitrage",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PairId = table.Column<long>(nullable: false),
                    DataDate = table.Column<DateTime>(nullable: false),
                    TranNo = table.Column<long>(nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChangePer = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    High24Hr = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Low24Hr = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TodayClose = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TodayOpen = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    HighWeek = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LowWeek = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    High52Week = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Low52Week = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LTP = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeGraphDetailArbitrage", x => new { x.Id, x.TranNo });
                    table.UniqueConstraint("AK_TradeGraphDetailArbitrage_TranNo", x => x.TranNo);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeGraphDetailArbitrage");
        }
    }
}
