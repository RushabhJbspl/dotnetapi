using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class unstakinghistory1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "UserProfileConfig");

            migrationBuilder.DropColumn(
                name: "Charge",
                table: "TokenStakingHistory");

            migrationBuilder.AddColumn<long>(
                name: "StakingRequestID",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "TokenUnStakingHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TokenStakingHistoryID = table.Column<long>(nullable: false),
                    AmountCredited = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    UnstakeType = table.Column<short>(nullable: false),
                    InterestCreditedValue = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    ChargeBeforeMaturity = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    DegradeStakingHistoryRequestID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenUnStakingHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenUnStakingHistory");

            migrationBuilder.DropColumn(
                name: "StakingRequestID",
                table: "TokenStakingHistory");

            migrationBuilder.AddColumn<long>(
                name: "LeaderId",
                table: "UserProfileConfig",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "Charge",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
