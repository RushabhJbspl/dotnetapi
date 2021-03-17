using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class margincolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ChargeAmount",
                table: "WithdrawLoanMaster",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ErrorCode",
                table: "WithdrawLoanMaster",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpgradeLoanID",
                table: "MarginLoanRequest",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "CloseOpenPostionMargin",
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
                    PairID = table.Column<long>(nullable: false),
                    TrnRefNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloseOpenPostionMargin", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloseOpenPostionMargin");

            migrationBuilder.DropColumn(
                name: "ChargeAmount",
                table: "WithdrawLoanMaster");

            migrationBuilder.DropColumn(
                name: "ErrorCode",
                table: "WithdrawLoanMaster");

            migrationBuilder.DropColumn(
                name: "UpgradeLoanID",
                table: "MarginLoanRequest");
        }
    }
}
