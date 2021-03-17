using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddNewColInReferralRewards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CommissionCroneID",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CommissionCurrecyId",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LifeTimeUserCount",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "NewUserCount",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReferralPayTypeId",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "SumChargeAmount",
                table: "ReferralRewards",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "SumOfTransaction",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TransactionCurrencyId",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionCroneID",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "CommissionCurrecyId",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "LifeTimeUserCount",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "NewUserCount",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "ReferralPayTypeId",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "SumChargeAmount",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "SumOfTransaction",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "TransactionCurrencyId",
                table: "ReferralRewards");
        }
    }
}
