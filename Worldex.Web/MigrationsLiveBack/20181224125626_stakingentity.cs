using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class stakingentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionBlockedChannel",
                table: "TransactionBlockedChannel");

            migrationBuilder.DropColumn(
                name: "ChannelName",
                table: "TransactionBlockedChannel");

            migrationBuilder.DropColumn(
                name: "EnableAutoUnstaking",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "EnableStakingBeforeMaturity",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "EnableStakingBeforeMaturityCharge",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "InterestType",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "InterestValue",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "InterestWalletTypeID",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "MakerCharges",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "MaxAmount",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "MinAmount",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "StakingDurationMonth",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "StakingDurationWeek",
                table: "StakingPolicyMaster");

            migrationBuilder.DropColumn(
                name: "TakerCharges",
                table: "StakingPolicyMaster");

            migrationBuilder.RenameColumn(
                name: "StakingPolicyID",
                table: "StakingChargeMaster",
                newName: "StakingPolicyDetailID");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelName",
                table: "AllowedChannels",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionBlockedChannel",
                table: "TransactionBlockedChannel",
                columns: new[] { "ChannelID", "TrnType" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StakingPolicyMaster_StakingType_WalletTypeID",
                table: "StakingPolicyMaster",
                columns: new[] { "StakingType", "WalletTypeID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionBlockedChannel",
                table: "TransactionBlockedChannel");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StakingPolicyMaster_StakingType_WalletTypeID",
                table: "StakingPolicyMaster");

            migrationBuilder.RenameColumn(
                name: "StakingPolicyDetailID",
                table: "StakingChargeMaster",
                newName: "StakingPolicyID");

            migrationBuilder.AddColumn<string>(
                name: "ChannelName",
                table: "TransactionBlockedChannel",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "EnableAutoUnstaking",
                table: "StakingPolicyMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "EnableStakingBeforeMaturity",
                table: "StakingPolicyMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "EnableStakingBeforeMaturityCharge",
                table: "StakingPolicyMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "InterestType",
                table: "StakingPolicyMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestValue",
                table: "StakingPolicyMaster",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "InterestWalletTypeID",
                table: "StakingPolicyMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "MakerCharges",
                table: "StakingPolicyMaster",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxAmount",
                table: "StakingPolicyMaster",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinAmount",
                table: "StakingPolicyMaster",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "StakingDurationMonth",
                table: "StakingPolicyMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "StakingDurationWeek",
                table: "StakingPolicyMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "TakerCharges",
                table: "StakingPolicyMaster",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "ChannelName",
                table: "AllowedChannels",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionBlockedChannel",
                table: "TransactionBlockedChannel",
                column: "Id");
        }
    }
}
