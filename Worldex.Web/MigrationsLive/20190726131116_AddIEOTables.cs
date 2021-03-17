using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddIEOTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IEOPurchaseHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    Guid = table.Column<string>(nullable: false),
                    RoundID = table.Column<long>(nullable: false),
                    PaidQuantity = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DeliveredQuantity = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    PaidCurrency = table.Column<string>(nullable: false),
                    DeliveredCurrency = table.Column<string>(nullable: false),
                    InstantQuantity = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaximumDeliveredQuantiy = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    OrgWalletID = table.Column<long>(nullable: false),
                    UserWalletID = table.Column<long>(nullable: false),
                    Remarks = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IEOPurchaseHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IEOPurchaseWalletMaster",
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
                    PurchaseWalletTypeId = table.Column<long>(nullable: false),
                    PurchaseRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ConvertCurrencyType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IEOPurchaseWalletMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IEORoundMaster",
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
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    IEOCurrencyId = table.Column<long>(nullable: false),
                    TotalSupply = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinimumPurchaseAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaximumPurchaseAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    AllocatedSupply = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IEORoundMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IEOSlabMaster",
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
                    RoundId = table.Column<long>(nullable: false),
                    Priority = table.Column<long>(nullable: false),
                    Value = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Duration = table.Column<long>(nullable: false),
                    DurationType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IEOSlabMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IROCronMaster",
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
                    IEOPurchaseHistoryId = table.Column<long>(nullable: false),
                    MaturityDate = table.Column<DateTime>(nullable: false),
                    RoundId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    DeliveryQuantity = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DeliveryCurrency = table.Column<string>(nullable: false),
                    CrWalletId = table.Column<long>(nullable: false),
                    DrWalletId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IROCronMaster", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IEOPurchaseHistory_Guid",
                table: "IEOPurchaseHistory",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_IEOPurchaseWalletMaster_Guid",
                table: "IEOPurchaseWalletMaster",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_IEORoundMaster_Guid",
                table: "IEORoundMaster",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_IEOSlabMaster_Guid",
                table: "IEOSlabMaster",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_IROCronMaster_Guid",
                table: "IROCronMaster",
                column: "Guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IEOPurchaseHistory");

            migrationBuilder.DropTable(
                name: "IEOPurchaseWalletMaster");

            migrationBuilder.DropTable(
                name: "IEORoundMaster");

            migrationBuilder.DropTable(
                name: "IEOSlabMaster");

            migrationBuilder.DropTable(
                name: "IROCronMaster");
        }
    }
}
