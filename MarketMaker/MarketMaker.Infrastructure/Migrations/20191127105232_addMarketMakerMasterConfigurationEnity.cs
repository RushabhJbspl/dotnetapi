using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketMaker.Infrastructure.Migrations
{
    public partial class addMarketMakerMasterConfigurationEnity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketMakerMasterConfiguration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GUID = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    NoOfBuyOrder = table.Column<long>(nullable: false),
                    NoOfSellOrder = table.Column<long>(nullable: false),
                    MarketMakerPreferenceID = table.Column<long>(nullable: false),
                    Depth = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SpreadGap = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    AvgQty = table.Column<decimal>(type: "decimal(28,18)", nullable: false),
                    OrderPerCall = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketMakerMasterConfiguration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketMakerMasterConfiguration");
        }
    }
}
