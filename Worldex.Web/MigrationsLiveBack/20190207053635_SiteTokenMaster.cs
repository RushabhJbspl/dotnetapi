using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class SiteTokenMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteTokenMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    CurrencyID = table.Column<long>(nullable: false),
                    BaseCurrencyID = table.Column<long>(nullable: false),
                    RateType = table.Column<short>(nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxxLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DailyLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    WeeklyLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTokenMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteTokenRateType",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TokenType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTokenRateType", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteTokenMaster");

            migrationBuilder.DropTable(
                name: "SiteTokenRateType");
        }
    }
}
