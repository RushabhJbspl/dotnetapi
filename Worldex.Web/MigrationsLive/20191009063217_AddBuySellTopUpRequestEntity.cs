using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddBuySellTopUpRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuySellTopUpRequest",
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
                    FromAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CoinRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    FiatConverationRate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    FromCurrency = table.Column<string>(nullable: false),
                    ToCurrency = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    TransactionHash = table.Column<string>(nullable: false),
                    NotifyUrl = table.Column<string>(nullable: false),
                    TransactionId = table.Column<string>(nullable: false),
                    TransactionCode = table.Column<string>(nullable: false),
                    UserGuid = table.Column<string>(nullable: false),
                    Platform = table.Column<string>(nullable: false),
                    Type = table.Column<short>(nullable: false),
                    FromBankId = table.Column<long>(nullable: false),
                    ToBankId = table.Column<long>(nullable: false),
                    Code = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuySellTopUpRequest", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuySellTopUpRequest_Guid",
                table: "BuySellTopUpRequest",
                column: "Guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuySellTopUpRequest");
        }
    }
}
