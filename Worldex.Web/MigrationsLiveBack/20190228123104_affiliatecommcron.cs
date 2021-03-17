using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class affiliatecommcron : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CommissionHour",
                table: "AffiliateSchemeTypeMapping",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "AffiliateCommissionCron",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    SchemeMappingId = table.Column<long>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateCommissionCron", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateCommissionHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TrnRefNo = table.Column<long>(nullable: false),
                    CronRefNo = table.Column<long>(nullable: false),
                    FromWalletId = table.Column<long>(nullable: false),
                    ToWalletId = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    AffiliateUserId = table.Column<long>(nullable: false),
                    SchemeMappingId = table.Column<long>(nullable: false),
                    TrnUserId = table.Column<long>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateCommissionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceStasticsMargin",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ServiceId = table.Column<long>(nullable: false),
                    MarketCap = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    VolGlobal = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxSupply = table.Column<long>(nullable: false),
                    CirculatingSupply = table.Column<long>(nullable: false),
                    IssuePrice = table.Column<decimal>(nullable: false),
                    IssueDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStasticsMargin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypeMappingMargin",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ServiceId = table.Column<long>(nullable: false),
                    TrnType = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypeMappingMargin", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AffiliateCommissionCron");

            migrationBuilder.DropTable(
                name: "AffiliateCommissionHistory");

            migrationBuilder.DropTable(
                name: "ServiceStasticsMargin");

            migrationBuilder.DropTable(
                name: "ServiceTypeMappingMargin");

            migrationBuilder.DropColumn(
                name: "CommissionHour",
                table: "AffiliateSchemeTypeMapping");
        }
    }
}
