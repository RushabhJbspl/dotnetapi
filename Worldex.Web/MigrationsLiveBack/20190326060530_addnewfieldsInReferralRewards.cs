using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addnewfieldsInReferralRewards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IsCommissionCredited",
                table: "ReferralUser",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<decimal>(
                name: "ReferralPayRewards",
                table: "ReferralRewards",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "ReferralRewards",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "FromWalletId",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ToWalletId",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TrnRefNo",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TrnUserId",
                table: "ReferralRewards",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCommissionCredited",
                table: "ReferralUser");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "FromWalletId",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "ToWalletId",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "TrnRefNo",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "TrnUserId",
                table: "ReferralRewards");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReferralPayRewards",
                table: "ReferralRewards",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");
        }
    }
}
