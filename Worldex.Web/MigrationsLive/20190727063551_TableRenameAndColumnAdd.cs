using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class TableRenameAndColumnAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IEOPurchaseWalletMaster");

            migrationBuilder.CreateTable(
                name: "IEOCurrencyPairMapping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Guid = table.Column<string>(nullable: false),
                    IEOWalletTypeId = table.Column<long>(nullable: false),
                    PaidWalletTypeId = table.Column<long>(nullable: false),
                    PurchaseRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ConvertCurrencyType = table.Column<short>(nullable: false),
                    InstantPercentage = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IEOCurrencyPairMapping", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IEOCurrencyPairMapping_Guid",
                table: "IEOCurrencyPairMapping",
                column: "Guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IEOCurrencyPairMapping");

            migrationBuilder.CreateTable(
                name: "IEOPurchaseWalletMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConvertCurrencyType = table.Column<short>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Guid = table.Column<string>(nullable: false),
                    IEOWalletTypeId = table.Column<long>(nullable: false),
                    PurchaseRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    PurchaseWalletTypeId = table.Column<long>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IEOPurchaseWalletMaster", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IEOPurchaseWalletMaster_Guid",
                table: "IEOPurchaseWalletMaster",
                column: "Guid");
        }
    }
}
