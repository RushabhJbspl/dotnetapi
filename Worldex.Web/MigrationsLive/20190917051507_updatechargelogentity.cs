using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class updatechargelogentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeductCurrency",
                table: "TrnChargeLog",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "TrnChargeLog",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "TrnChargeLog",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeductCurrency",
                table: "TrnChargeLog");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "TrnChargeLog");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TrnChargeLog");
        }
    }
}
