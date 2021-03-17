using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addAffiliateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AffiliateLinkClick",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    AffiliateUserId = table.Column<long>(nullable: false),
                    PromotionTypeId = table.Column<long>(nullable: false),
                    LinkDetail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateLinkClick", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliatePromotionMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    PromotionType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliatePromotionMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliatePromotionShare",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    AffiliateUserId = table.Column<long>(nullable: false),
                    PromotionTypeId = table.Column<long>(nullable: false),
                    PromotionDetail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliatePromotionShare", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliatePromotionUserTypeMapping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    AffiliateUserId = table.Column<long>(nullable: false),
                    PromotionTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliatePromotionUserTypeMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateSchemeDetail",
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
                    Level = table.Column<int>(nullable: false),
                    MinimumValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaximumValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CreditWalletTypeId = table.Column<long>(nullable: false),
                    CommissionType = table.Column<int>(nullable: false),
                    CommissionValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DistributionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateSchemeDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateSchemeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    SchemeType = table.Column<string>(nullable: false),
                    SchemeName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateSchemeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateSchemeTypeMapping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    SchemeMstId = table.Column<long>(nullable: false),
                    SchemeTypeMstId = table.Column<long>(nullable: false),
                    MinimumDepositionRequired = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DepositWalletTypeId = table.Column<long>(nullable: false),
                    CommissionTypeInterval = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateSchemeTypeMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateSchemeTypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    SchemeTypeName = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateSchemeTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateUserMaster",
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
                    ParentId = table.Column<long>(nullable: false),
                    UserBit = table.Column<long>(nullable: false),
                    SchemeMstId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateUserMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AffiliateLinkClick");

            migrationBuilder.DropTable(
                name: "AffiliatePromotionMaster");

            migrationBuilder.DropTable(
                name: "AffiliatePromotionShare");

            migrationBuilder.DropTable(
                name: "AffiliatePromotionUserTypeMapping");

            migrationBuilder.DropTable(
                name: "AffiliateSchemeDetail");

            migrationBuilder.DropTable(
                name: "AffiliateSchemeMaster");

            migrationBuilder.DropTable(
                name: "AffiliateSchemeTypeMapping");

            migrationBuilder.DropTable(
                name: "AffiliateSchemeTypeMaster");

            migrationBuilder.DropTable(
                name: "AffiliateUserMaster");
        }
    }
}
