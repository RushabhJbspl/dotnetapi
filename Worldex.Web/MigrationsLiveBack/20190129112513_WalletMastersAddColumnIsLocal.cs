using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class WalletMastersAddColumnIsLocal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IsLocal",
                table: "WalletTypeMasters",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeductionAmount",
                table: "TokenStakingHistory",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "GasLimit",
                table: "RouteConfiguration",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocal",
                table: "WalletTypeMasters");

            migrationBuilder.DropColumn(
                name: "DeductionAmount",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "GasLimit",
                table: "RouteConfiguration");
        }
    }
}
