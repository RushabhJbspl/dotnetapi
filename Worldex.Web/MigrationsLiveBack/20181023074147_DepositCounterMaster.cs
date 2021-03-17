using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class DepositCounterMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DepositHistorys",
                table: "DepositHistorys");

            migrationBuilder.RenameTable(
                name: "DepositHistorys",
                newName: "DepositHistory");

            migrationBuilder.AddColumn<string>(
                name: "prevIterationID",
                table: "DepositCounterMaster",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DepositHistory_Address_TrnID",
                table: "DepositHistory",
                columns: new[] { "Address", "TrnID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepositHistory",
                table: "DepositHistory",
                columns: new[] { "TrnID", "Address" });

            migrationBuilder.CreateTable(
                name: "TradeGraphDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    PairId = table.Column<long>(nullable: false),
                    DataDate = table.Column<DateTime>(nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TodayClose = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TodayOpen = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeGraphDetail", x => new { x.Id, x.PairId, x.DataDate });
                    table.UniqueConstraint("AK_TradeGraphDetail_DataDate_Id_PairId", x => new { x.DataDate, x.Id, x.PairId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeGraphDetail");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DepositHistory_Address_TrnID",
                table: "DepositHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepositHistory",
                table: "DepositHistory");

            migrationBuilder.DropColumn(
                name: "prevIterationID",
                table: "DepositCounterMaster");

            migrationBuilder.RenameTable(
                name: "DepositHistory",
                newName: "DepositHistorys");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepositHistorys",
                table: "DepositHistorys",
                column: "TrnID");
        }
    }
}
