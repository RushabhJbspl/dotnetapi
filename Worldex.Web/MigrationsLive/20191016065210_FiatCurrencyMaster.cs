using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class FiatCurrencyMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SellFee",
                table: "FiatCoinConfiguration",
                newName: "Rate");

            migrationBuilder.RenameColumn(
                name: "BuyFee",
                table: "FiatCoinConfiguration",
                newName: "MinRate");

            migrationBuilder.CreateTable(
                name: "FiatCurrencyMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CurrencyCode = table.Column<string>(nullable: false),
                    USDRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BuyFee = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SellFee = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BuyFeeType = table.Column<short>(nullable: false),
                    SellFeeType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiatCurrencyMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiatCurrencyMaster");

            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "FiatCoinConfiguration",
                newName: "SellFee");

            migrationBuilder.RenameColumn(
                name: "MinRate",
                table: "FiatCoinConfiguration",
                newName: "BuyFee");
        }
    }
}
