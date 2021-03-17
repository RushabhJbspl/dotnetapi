using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class pairidinrouteconfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderType",
                table: "RouteConfiguration",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PairId",
                table: "RouteConfiguration",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyTypeName",
                table: "CurrencyTypeMaster",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 7);

            migrationBuilder.CreateTable(
                name: "Statastics",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    TrnType = table.Column<long>(nullable: false),
                    WalletType = table.Column<long>(nullable: false),
                    WalletId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Hour = table.Column<long>(nullable: false),
                    Day = table.Column<long>(nullable: false),
                    Week = table.Column<long>(nullable: false),
                    Month = table.Column<long>(nullable: false),
                    Year = table.Column<long>(nullable: false),
                    Count = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statastics", x => new { x.WalletId, x.WalletType, x.TrnType, x.Hour, x.Day, x.Week, x.Month, x.Year, x.UserId });
                    table.UniqueConstraint("AK_Statastics_Day_Hour_Month_TrnType_UserId_WalletId_WalletType_Week_Year", x => new { x.Day, x.Hour, x.Month, x.TrnType, x.UserId, x.WalletId, x.WalletType, x.Week, x.Year });
                });

            migrationBuilder.CreateTable(
                name: "StatasticsDetail",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    StatasticsId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityName = table.Column<string>(maxLength: 50, nullable: false),
                    TrnNo = table.Column<string>(nullable: false),
                    CurrentTime = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatasticsDetail", x => x.StatasticsId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statastics");

            migrationBuilder.DropTable(
                name: "StatasticsDetail");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "RouteConfiguration");

            migrationBuilder.DropColumn(
                name: "PairId",
                table: "RouteConfiguration");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyTypeName",
                table: "CurrencyTypeMaster",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
