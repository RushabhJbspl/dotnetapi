using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddFiatTradeConfigurationMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FiatTradeConfigurationMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    BuyFee = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellFee = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TermsAndCondition = table.Column<string>(nullable: false),
                    IsBuyEnable = table.Column<short>(nullable: false),
                    IsSellEnable = table.Column<short>(nullable: false),
                    BuyFeeType = table.Column<short>(nullable: false),
                    SellFeeType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiatTradeConfigurationMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiatTradeConfigurationMaster");
        }
    }
}
