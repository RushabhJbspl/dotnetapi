using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class stackingtbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StakingPolicyDetail",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StakingPolicyID = table.Column<long>(nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    StakingDurationWeek = table.Column<short>(nullable: false),
                    StakingDurationMonth = table.Column<short>(nullable: false),
                    InterestType = table.Column<short>(nullable: false),
                    InterestValue = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    InterestWalletTypeID = table.Column<long>(nullable: false),
                    MakerCharges = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TakerCharges = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    EnableAutoUnstaking = table.Column<short>(nullable: false),
                    EnableStakingBeforeMaturity = table.Column<short>(nullable: false),
                    EnableStakingBeforeMaturityCharge = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StakingPolicyDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokenStakingHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    StakingAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    StakingPolicyDetailID = table.Column<long>(nullable: false),
                    Charge = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaturityDate = table.Column<DateTime>(nullable: false),
                    MaturityAmount = table.Column<decimal>(nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MakerCharges = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TakerCharges = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    EnableAutoUnstaking = table.Column<short>(nullable: false),
                    EnableStakingBeforeMaturity = table.Column<short>(nullable: false),
                    EnableStakingBeforeMaturityCharge = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenStakingHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StakingPolicyDetail");

            migrationBuilder.DropTable(
                name: "TokenStakingHistory");
        }
    }
}
