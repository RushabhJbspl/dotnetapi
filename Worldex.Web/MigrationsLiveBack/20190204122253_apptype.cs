using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class apptype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IsAPITrade",
                table: "TradePoolQueueV1",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "AppType",
                table: "DepositCounterMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "APIOrderSettlement",
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
                    table.PrimaryKey("PK_APIOrderSettlement", x => x.TrnNo);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIOrderSettlement");

            migrationBuilder.DropColumn(
                name: "IsAPITrade",
                table: "TradePoolQueueV1");

            migrationBuilder.DropColumn(
                name: "AppType",
                table: "DepositCounterMaster");
        }
    }
}
