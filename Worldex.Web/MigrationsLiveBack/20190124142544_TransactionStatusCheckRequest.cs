using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class TransactionStatusCheckRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "CallStatus",
                table: "TransactionQueue",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume_24h",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CurrencyRateDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_7d",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_24h",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_1h",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Market_cap",
                table: "CurrencyRateDetail",
                type: "decimal(25, 10)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.CreateTable(
                name: "TransactionStatusCheckRequest",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    SerProDetailID = table.Column<long>(nullable: false),
                    RequestData = table.Column<string>(nullable: true),
                    ResponseTime = table.Column<DateTime>(nullable: false),
                    ResponseData = table.Column<string>(nullable: true),
                    TrnID = table.Column<string>(nullable: true),
                    OprTrnID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionStatusCheckRequest", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionStatusCheckRequest");

            migrationBuilder.DropColumn(
                name: "CallStatus",
                table: "TransactionQueue");

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume_24h",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_7d",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_24h",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_1h",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Market_cap",
                table: "CurrencyRateDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(25, 10)");
        }
    }
}
