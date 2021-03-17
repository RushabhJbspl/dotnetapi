using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddNewColForCharge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkDetail",
                table: "ReferralUserClick");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "ReferralUserClick",
                newName: "ReferralServiceId");

            migrationBuilder.AddColumn<decimal>(
                name: "ConvertedAmount",
                table: "TrnChargeLog",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "ReferralChannelTypeId",
                table: "ReferralUserClick",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "IsCurrencyConverted",
                table: "ChargeConfigurationDetail",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertedAmount",
                table: "TrnChargeLog");

            migrationBuilder.DropColumn(
                name: "ReferralChannelTypeId",
                table: "ReferralUserClick");

            migrationBuilder.DropColumn(
                name: "IsCurrencyConverted",
                table: "ChargeConfigurationDetail");

            migrationBuilder.RenameColumn(
                name: "ReferralServiceId",
                table: "ReferralUserClick",
                newName: "ServiceId");

            migrationBuilder.AddColumn<string>(
                name: "LinkDetail",
                table: "ReferralUserClick",
                nullable: true);
        }
    }
}
