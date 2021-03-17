using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class APIPlanConfigurationHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIPlanConfigurationHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    PlanName = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Charge = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    PlanDesc = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    MaxPerMinute = table.Column<long>(nullable: false),
                    MaxPerDay = table.Column<long>(nullable: false),
                    MaxPerMonth = table.Column<long>(nullable: false),
                    MaxOrderPerSec = table.Column<long>(nullable: false),
                    MaxRecPerRequest = table.Column<long>(nullable: false),
                    MaxReqSize = table.Column<long>(nullable: false),
                    MaxResSize = table.Column<long>(nullable: false),
                    WhitelistedEndPoints = table.Column<int>(nullable: false),
                    ConcurrentEndPoints = table.Column<int>(nullable: false),
                    HistoricalDataMonth = table.Column<int>(nullable: false),
                    IsPlanRecursive = table.Column<short>(nullable: false),
                    PlanValidity = table.Column<int>(nullable: false),
                    PlanValidityType = table.Column<int>(nullable: false),
                    CreatedIPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIPlanConfigurationHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIPlanConfigurationHistory");
        }
    }
}
