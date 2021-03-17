using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class UpdateWalletService1510 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "TradeCancelQueue",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        TrnNo = table.Column<long>(nullable: false),
            //        DeliverServiceID = table.Column<long>(nullable: false),
            //        TrnDate = table.Column<DateTime>(nullable: false),
            //        PendingBuyQty = table.Column<decimal>(nullable: false),
            //        DeliverQty = table.Column<decimal>(nullable: false),
            //        OrderType = table.Column<short>(nullable: true),
            //        DeliverBidPrice = table.Column<decimal>(nullable: true),
            //        Status = table.Column<short>(nullable: false),
            //        StatusMsg = table.Column<string>(nullable: false),
            //        OrderID = table.Column<long>(nullable: false),
            //        SettledDate = table.Column<DateTime>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TradeCancelQueue", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "TradeCancelQueue");
        }
    }
}
