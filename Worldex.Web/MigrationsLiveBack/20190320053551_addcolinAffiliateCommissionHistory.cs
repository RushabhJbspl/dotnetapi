using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addcolinAffiliateCommissionHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPer",
                table: "AffiliateCommissionHistory",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "Level",
                table: "AffiliateCommissionHistory",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "TransactionAmount",
                table: "AffiliateCommissionHistory",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrnDate",
                table: "AffiliateCommissionHistory",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "TradingRecon",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    OldStatus = table.Column<short>(nullable: false),
                    NewStatus = table.Column<short>(nullable: false),
                    Remarks = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingRecon", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradingRecon");

            migrationBuilder.DropColumn(
                name: "CommissionPer",
                table: "AffiliateCommissionHistory");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "AffiliateCommissionHistory");

            migrationBuilder.DropColumn(
                name: "TransactionAmount",
                table: "AffiliateCommissionHistory");

            migrationBuilder.DropColumn(
                name: "TrnDate",
                table: "AffiliateCommissionHistory");
        }
    }
}
