using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddNewReferelEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReferralSchemeTypeMapping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    PayTypeId = table.Column<long>(nullable: false),
                    ServiceTypeMstId = table.Column<long>(nullable: false),
                    MinimumDepositionRequired = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FromDate = table.Column<DateTime>(nullable: false),
                    ToDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralSchemeTypeMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferralServiceDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    SchemeTypeMappingId = table.Column<long>(nullable: false),
                    MaximumLevel = table.Column<long>(nullable: false),
                    MaximumCoin = table.Column<long>(nullable: false),
                    MinimumValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaximumValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CreditWalletTypeId = table.Column<long>(nullable: false),
                    CommissionType = table.Column<int>(nullable: false),
                    CommissionValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralServiceDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferralUserLevelMapping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    ReferUserId = table.Column<long>(nullable: false),
                    Level = table.Column<long>(nullable: false),
                    IsCommissionCredited = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralUserLevelMapping", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferralSchemeTypeMapping");

            migrationBuilder.DropTable(
                name: "ReferralServiceDetail");

            migrationBuilder.DropTable(
                name: "ReferralUserLevelMapping");
        }
    }
}
