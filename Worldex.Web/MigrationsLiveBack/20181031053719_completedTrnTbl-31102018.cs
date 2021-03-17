using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class completedTrnTbl31102018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DMemberID",
                table: "PoolOrder",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "OMemberID",
                table: "PoolOrder",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "TradeBuyerList",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PoolID = table.Column<long>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeBuyerList", x => new { x.TrnNo, x.PoolID });
                    table.UniqueConstraint("AK_TradeBuyerList_PoolID_TrnNo", x => new { x.PoolID, x.TrnNo });
                });

            migrationBuilder.CreateTable(
                name: "TradeDepositCompletedTrn",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnID = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeDepositCompletedTrn", x => new { x.Address, x.TrnID });
                });

            migrationBuilder.CreateTable(
                name: "TradeSellerList",
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
                    BuyReqID = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeSellerList", x => new { x.Id, x.TrnNo });
                    table.UniqueConstraint("AK_TradeSellerList_TrnNo", x => x.TrnNo);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeBuyerList");

            migrationBuilder.DropTable(
                name: "TradeDepositCompletedTrn");

            migrationBuilder.DropTable(
                name: "TradeSellerList");

            migrationBuilder.DropColumn(
                name: "DMemberID",
                table: "PoolOrder");

            migrationBuilder.DropColumn(
                name: "OMemberID",
                table: "PoolOrder");
        }
    }
}
