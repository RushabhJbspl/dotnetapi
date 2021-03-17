using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addFiatCoinConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FiatCoinConfiguration",
                columns: table => new
                {
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MinQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BuyFee = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellFee = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiatCoinConfiguration", x => new { x.FromCurrencyId, x.ToCurrencyId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiatCoinConfiguration");
        }
    }
}
