using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class stackingchanges1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChannelID",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WalletID",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WalletOwnerID",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WalletTypeID",
                table: "TokenStakingHistory",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelID",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "WalletID",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "WalletOwnerID",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "WalletTypeID",
                table: "TokenStakingHistory");
        }
    }
}
