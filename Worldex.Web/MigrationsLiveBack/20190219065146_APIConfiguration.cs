using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class APIConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BenefitPay",
                table: "ReferralService",
                newName: "RewardsPay");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveDate",
                table: "ReferralService",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "APIPlanDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    APIPlanMasterID = table.Column<long>(nullable: false),
                    MaxPerMinute = table.Column<long>(nullable: false),
                    MaxPerDay = table.Column<long>(nullable: false),
                    MaxPerMonth = table.Column<long>(nullable: false),
                    MaxOrderPerSec = table.Column<long>(nullable: false),
                    MaxRecPerRequest = table.Column<long>(nullable: false),
                    MaxReqSize = table.Column<long>(nullable: false),
                    MaxResSize = table.Column<long>(nullable: false),
                    WhitelistedEndPoints = table.Column<int>(nullable: false),
                    ConcurrentEndPoints = table.Column<int>(nullable: false),
                    HistoricalDataMonth = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIPlanDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APIPlanMaster",
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
                    Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Charge = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    PlanValidity = table.Column<DateTime>(nullable: false),
                    PlanDesc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIPlanMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APIPlanMethodConfiguration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    RestMethodID = table.Column<long>(nullable: false),
                    APIPlanMasterID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIPlanMethodConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RestMethods",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    MethodName = table.Column<string>(maxLength: 50, nullable: false),
                    IsReadOnly = table.Column<short>(nullable: false),
                    IsFullAccess = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAPIKeyDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    AliasName = table.Column<string>(maxLength: 50, nullable: false),
                    APIPermission = table.Column<short>(nullable: false),
                    SecretKey = table.Column<string>(nullable: true),
                    APIKey = table.Column<string>(nullable: true),
                    IPAccess = table.Column<short>(nullable: false),
                    APIPlanMasterID = table.Column<long>(nullable: false),
                    APIPlanDeatilID = table.Column<long>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAPIKeyDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WhiteListIPEndPoint",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    APIKeyDetailsID = table.Column<long>(nullable: false),
                    APIPlanDeatilID = table.Column<long>(nullable: false),
                    IPAddress = table.Column<string>(nullable: true),
                    UserID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhiteListIPEndPoint", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIPlanDetail");

            migrationBuilder.DropTable(
                name: "APIPlanMaster");

            migrationBuilder.DropTable(
                name: "APIPlanMethodConfiguration");

            migrationBuilder.DropTable(
                name: "RestMethods");

            migrationBuilder.DropTable(
                name: "UserAPIKeyDetails");

            migrationBuilder.DropTable(
                name: "WhiteListIPEndPoint");

            migrationBuilder.DropColumn(
                name: "ActiveDate",
                table: "ReferralService");

            migrationBuilder.RenameColumn(
                name: "RewardsPay",
                table: "ReferralService",
                newName: "BenefitPay");
        }
    }
}
