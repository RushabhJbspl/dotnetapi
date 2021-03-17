using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class TransactionRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BidPrice",
                table: "TradePoolMaster",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.CreateTable(
                name: "TransactionRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    ServiceID = table.Column<long>(nullable: false),
                    SerProID = table.Column<long>(nullable: false),
                    SerProDetailID = table.Column<long>(nullable: false),
                    RequestData = table.Column<string>(nullable: false),
                    ResponseTime = table.Column<DateTime>(nullable: false),
                    ResponseData = table.Column<string>(nullable: true),
                    TrnID = table.Column<string>(nullable: true),
                    OprTrnID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionRequest", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionRequest");

            migrationBuilder.AlterColumn<long>(
                name: "BidPrice",
                table: "TradePoolMaster",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");
        }
    }
}
