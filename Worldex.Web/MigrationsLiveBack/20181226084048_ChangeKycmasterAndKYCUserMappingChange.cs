using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ChangeKycmasterAndKYCUserMappingChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "RenewUnstakingEnable",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "RenewUnstakingPeriod",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<decimal>(
                name: "EnableStakingBeforeMaturityCharge",
                table: "StakingPolicyDetail",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddColumn<short>(
                name: "RenewUnstakingEnable",
                table: "StakingPolicyDetail",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "RenewUnstakingPeriod",
                table: "StakingPolicyDetail",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentMasterId",
                table: "kYCIdentityMaster",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewUnstakingEnable",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "RenewUnstakingPeriod",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "RenewUnstakingEnable",
                table: "StakingPolicyDetail");

            migrationBuilder.DropColumn(
                name: "RenewUnstakingPeriod",
                table: "StakingPolicyDetail");

            migrationBuilder.AlterColumn<short>(
                name: "EnableStakingBeforeMaturityCharge",
                table: "StakingPolicyDetail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentMasterId",
                table: "kYCIdentityMaster",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
