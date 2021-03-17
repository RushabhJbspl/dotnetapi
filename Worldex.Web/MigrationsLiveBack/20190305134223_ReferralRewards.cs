using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ReferralRewards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPlanRecursive",
                table: "APIPlanDetail");

            migrationBuilder.DropColumn(
                name: "PlanValidity",
                table: "APIPlanDetail");

            migrationBuilder.RenameColumn(
                name: "APIPlanMasterID",
                table: "APIPlanDetail",
                newName: "APIPlanID");

            migrationBuilder.AddColumn<long>(
                name: "CustomeLimitId",
                table: "UserSubscribeAPIPlan",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CustomeLimitId",
                table: "APIPlanMethodConfiguration",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ReferralRewards",
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
                    ReferralServiceId = table.Column<long>(nullable: false),
                    ReferralPayRewards = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralRewards", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "CustomeLimitId",
                table: "UserSubscribeAPIPlan");

            migrationBuilder.DropColumn(
                name: "CustomeLimitId",
                table: "APIPlanMethodConfiguration");

            migrationBuilder.RenameColumn(
                name: "APIPlanID",
                table: "APIPlanDetail",
                newName: "APIPlanMasterID");

            migrationBuilder.AddColumn<short>(
                name: "IsPlanRecursive",
                table: "APIPlanDetail",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "PlanValidity",
                table: "APIPlanDetail",
                nullable: false,
                defaultValue: 0);
        }
    }
}
