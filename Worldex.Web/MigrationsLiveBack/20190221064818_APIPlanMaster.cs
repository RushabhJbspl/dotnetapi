using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class APIPlanMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APIPlanDeatilID",
                table: "UserAPIKeyDetails");

            migrationBuilder.DropColumn(
                name: "APIKeyLimit",
                table: "APIPlanDetail");

            migrationBuilder.RenameColumn(
                name: "IPAddress",
                table: "APIPlanMaster",
                newName: "CreatedIPAddress");

            migrationBuilder.AlterColumn<long>(
                name: "UserID",
                table: "WhiteListIPEndPoint",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmt",
                table: "SubScribePlan",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "SubScribePlan",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<decimal>(
                name: "Charge",
                table: "SubScribePlan",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "ReferralChannel",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ConcurrentEndPoints",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HistoricalDataMonth",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "MaxOrderPerSec",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MaxPerDay",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MaxPerMinute",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MaxPerMonth",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MaxRecPerRequest",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MaxReqSize",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MaxResSize",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "PlanValidityType",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WhitelistedEndPoints",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "IsPlanRecursive",
                table: "APIPlanDetail",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "PlanValidity",
                table: "APIPlanDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TradeGraphDetailMargin",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PairId = table.Column<long>(nullable: false),
                    DataDate = table.Column<DateTime>(nullable: false),
                    TranNo = table.Column<long>(nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChangePer = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    High24Hr = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Low24Hr = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TodayClose = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TodayOpen = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    HighWeek = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LowWeek = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    High52Week = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Low52Week = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LTP = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeGraphDetailMargin", x => new { x.Id, x.TranNo });
                    table.UniqueConstraint("AK_TradeGraphDetailMargin_TranNo", x => x.TranNo);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeGraphDetailMargin");

            migrationBuilder.DropColumn(
                name: "ConcurrentEndPoints",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "HistoricalDataMonth",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "MaxOrderPerSec",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "MaxPerDay",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "MaxPerMinute",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "MaxPerMonth",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "MaxRecPerRequest",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "MaxReqSize",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "MaxResSize",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "PlanValidityType",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "WhitelistedEndPoints",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "IsPlanRecursive",
                table: "APIPlanDetail");

            migrationBuilder.DropColumn(
                name: "PlanValidity",
                table: "APIPlanDetail");

            migrationBuilder.RenameColumn(
                name: "CreatedIPAddress",
                table: "APIPlanMaster",
                newName: "IPAddress");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "WhiteListIPEndPoint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "APIPlanDeatilID",
                table: "UserAPIKeyDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "TotalAmt",
                table: "SubScribePlan",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<long>(
                name: "Price",
                table: "SubScribePlan",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<long>(
                name: "Charge",
                table: "SubScribePlan",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ReferralChannel",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "APIKeyLimit",
                table: "APIPlanDetail",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
