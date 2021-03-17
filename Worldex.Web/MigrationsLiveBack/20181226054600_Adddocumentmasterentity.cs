using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class Adddocumentmasterentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LTP",
                table: "TradeStopLoss",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "MarketIndicator",
                table: "TradeStopLoss",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "RangeMax",
                table: "TradeStopLoss",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RangeMin",
                table: "TradeStopLoss",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "VerifyStatus",
                table: "PersonalVerification",
                nullable: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "PersonalVerification",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentMaster",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentMaster");

            migrationBuilder.DropColumn(
                name: "LTP",
                table: "TradeStopLoss");

            migrationBuilder.DropColumn(
                name: "MarketIndicator",
                table: "TradeStopLoss");

            migrationBuilder.DropColumn(
                name: "RangeMax",
                table: "TradeStopLoss");

            migrationBuilder.DropColumn(
                name: "RangeMin",
                table: "TradeStopLoss");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "PersonalVerification");

            migrationBuilder.AlterColumn<bool>(
                name: "VerifyStatus",
                table: "PersonalVerification",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
