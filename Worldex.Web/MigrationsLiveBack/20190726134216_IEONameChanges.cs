using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class IEONameChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IROCronMaster");

            migrationBuilder.AlterColumn<string>(
                name: "PaidCurrency",
                table: "IEOPurchaseHistory",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "DeliveredCurrency",
                table: "IEOPurchaseHistory",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "IEOCronMaster",
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
                    table.PrimaryKey("PK_IEOCronMaster", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IEOPurchaseHistory_DeliveredCurrency",
                table: "IEOPurchaseHistory",
                column: "DeliveredCurrency");

            migrationBuilder.CreateIndex(
                name: "IX_IEOPurchaseHistory_PaidCurrency",
                table: "IEOPurchaseHistory",
                column: "PaidCurrency");

            migrationBuilder.CreateIndex(
                name: "IX_IEOPurchaseHistory_UserID",
                table: "IEOPurchaseHistory",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_IEOCronMaster_Guid",
                table: "IEOCronMaster",
                column: "Guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IEOCronMaster");

            migrationBuilder.DropIndex(
                name: "IX_IEOPurchaseHistory_DeliveredCurrency",
                table: "IEOPurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_IEOPurchaseHistory_PaidCurrency",
                table: "IEOPurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_IEOPurchaseHistory_UserID",
                table: "IEOPurchaseHistory");

            migrationBuilder.AlterColumn<string>(
                name: "PaidCurrency",
                table: "IEOPurchaseHistory",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "DeliveredCurrency",
                table: "IEOPurchaseHistory",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "IROCronMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CrWalletId = table.Column<long>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DeliveryCurrency = table.Column<string>(nullable: false),
                    DeliveryQuantity = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DrWalletId = table.Column<long>(nullable: false),
                    Guid = table.Column<string>(nullable: false),
                    IEOPurchaseHistoryId = table.Column<long>(nullable: false),
                    MaturityDate = table.Column<DateTime>(nullable: false),
                    RoundId = table.Column<long>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IROCronMaster", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IROCronMaster_Guid",
                table: "IROCronMaster",
                column: "Guid");
        }
    }
}
