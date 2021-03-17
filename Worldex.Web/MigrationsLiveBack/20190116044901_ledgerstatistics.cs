using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ledgerstatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StakingPolicyMaster",
                table: "StakingPolicyMaster");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StakingPolicyMaster",
                table: "StakingPolicyMaster",
                columns: new[] { "WalletTypeID", "StakingType" });

            migrationBuilder.CreateTable(
                name: "BalanceStatistics",
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
                    WalletID = table.Column<long>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    Year = table.Column<short>(nullable: false),
                    Month = table.Column<short>(nullable: false),
                    StartingBalance = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    EndingBalance = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyRateMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletTypeId = table.Column<long>(nullable: false),
                    CurrentRate = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRateMaster", x => x.WalletTypeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceStatistics");

            migrationBuilder.DropTable(
                name: "CurrencyRateMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StakingPolicyMaster",
                table: "StakingPolicyMaster");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StakingPolicyMaster",
                table: "StakingPolicyMaster",
                column: "WalletTypeID");
        }
    }
}
