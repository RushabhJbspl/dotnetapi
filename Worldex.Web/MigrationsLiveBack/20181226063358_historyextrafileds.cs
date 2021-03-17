using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class historyextrafileds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscType",
                table: "StakingPolicyMaster",
                newName: "SlabType");

            migrationBuilder.AddColumn<short>(
                name: "InterestType",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestValue",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "InterestWalletTypeID",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestType",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "InterestValue",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "InterestWalletTypeID",
                table: "TokenStakingHistory");

            migrationBuilder.RenameColumn(
                name: "SlabType",
                table: "StakingPolicyMaster",
                newName: "DiscType");
        }
    }
}
