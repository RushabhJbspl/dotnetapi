using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class StatasticsChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "AddRemoveUserWalletRequest",
                newName: "ReceiverEmail");

            migrationBuilder.AddColumn<DateTime>(
                name: "CronDate",
                table: "TradePairStastics",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<short>(
                name: "SlabType",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "StakingType",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CronDate",
                table: "TradePairStastics");

            migrationBuilder.DropColumn(
                name: "SlabType",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "StakingType",
                table: "TokenStakingHistory");

            migrationBuilder.RenameColumn(
                name: "ReceiverEmail",
                table: "AddRemoveUserWalletRequest",
                newName: "Email");
        }
    }
}
