using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class GraphData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_TradeGraphDetail_DataDate_Id_PairId",
                table: "TradeGraphDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeGraphDetail",
                table: "TradeGraphDetail");

            migrationBuilder.AddColumn<short>(
                name: "IsDefaultWallet",
                table: "WalletTypeMasters",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TranNo",
                table: "TradeGraphDetail",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "ChangePer",
                table: "TradeGraphDetail",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TradeGraphDetail_TranNo",
                table: "TradeGraphDetail",
                column: "TranNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeGraphDetail",
                table: "TradeGraphDetail",
                columns: new[] { "Id", "TranNo" });

            migrationBuilder.CreateTable(
                name: "WalletLimitConfigurationMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnType = table.Column<int>(nullable: false),
                    LimitPerHour = table.Column<decimal>(nullable: false),
                    LimitPerDay = table.Column<decimal>(nullable: false),
                    LimitPerTransaction = table.Column<decimal>(nullable: false),
                    LifeTime = table.Column<decimal>(nullable: true),
                    StartTime = table.Column<TimeSpan>(nullable: false),
                    EndTime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletLimitConfigurationMaster", x => x.TrnType);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletLimitConfigurationMaster");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TradeGraphDetail_TranNo",
                table: "TradeGraphDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeGraphDetail",
                table: "TradeGraphDetail");

            migrationBuilder.DropColumn(
                name: "IsDefaultWallet",
                table: "WalletTypeMasters");

            migrationBuilder.DropColumn(
                name: "TranNo",
                table: "TradeGraphDetail");

            migrationBuilder.DropColumn(
                name: "ChangePer",
                table: "TradeGraphDetail");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TradeGraphDetail_DataDate_Id_PairId",
                table: "TradeGraphDetail",
                columns: new[] { "DataDate", "Id", "PairId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeGraphDetail",
                table: "TradeGraphDetail",
                columns: new[] { "Id", "PairId", "DataDate" });
        }
    }
}
