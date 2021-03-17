using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addnewcolWalletTrnLimitConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HourlyTrnAmount",
                table: "WalletTrnLimitConfiguration",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "HourlyTrnCount",
                table: "WalletTrnLimitConfiguration",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyTrnAmount",
                table: "WalletTrnLimitConfiguration");

            migrationBuilder.DropColumn(
                name: "HourlyTrnCount",
                table: "WalletTrnLimitConfiguration");
        }
    }
}
