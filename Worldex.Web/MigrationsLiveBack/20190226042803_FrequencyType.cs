using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class FrequencyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "LifeTime",
                table: "WalletLimitConfiguration",
                type: "decimal(28, 18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReferralChannelTypeId",
                table: "ReferralUser",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DailyLimit",
                table: "ReferralChannelType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HourlyLimit",
                table: "ReferralChannelType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyLimit",
                table: "ReferralChannelType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeeklyLimit",
                table: "ReferralChannelType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AddFrequencyType",
                table: "PublicAPIKeyPolicy",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeleteFrequencyType",
                table: "PublicAPIKeyPolicy",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReferralUserClick",
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
                    ServiceId = table.Column<long>(nullable: false),
                    LinkDetail = table.Column<string>(nullable: true),
                    IPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralUserClick", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferralUserClick");

            migrationBuilder.DropColumn(
                name: "ReferralChannelTypeId",
                table: "ReferralUser");

            migrationBuilder.DropColumn(
                name: "DailyLimit",
                table: "ReferralChannelType");

            migrationBuilder.DropColumn(
                name: "HourlyLimit",
                table: "ReferralChannelType");

            migrationBuilder.DropColumn(
                name: "MonthlyLimit",
                table: "ReferralChannelType");

            migrationBuilder.DropColumn(
                name: "WeeklyLimit",
                table: "ReferralChannelType");

            migrationBuilder.DropColumn(
                name: "AddFrequencyType",
                table: "PublicAPIKeyPolicy");

            migrationBuilder.DropColumn(
                name: "DeleteFrequencyType",
                table: "PublicAPIKeyPolicy");

            migrationBuilder.AlterColumn<decimal>(
                name: "LifeTime",
                table: "WalletLimitConfiguration",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)",
                oldNullable: true);
        }
    }
}
