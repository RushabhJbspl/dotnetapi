using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addnewcolinhistorytable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BonusAmount",
                table: "IEOPurchaseHistory",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BonusPercentage",
                table: "IEOPurchaseHistory",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumDeliveredQuantiyWOBonus",
                table: "IEOPurchaseHistory",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "IEOPurchaseHistory",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusAmount",
                table: "IEOPurchaseHistory");

            migrationBuilder.DropColumn(
                name: "BonusPercentage",
                table: "IEOPurchaseHistory");

            migrationBuilder.DropColumn(
                name: "MaximumDeliveredQuantiyWOBonus",
                table: "IEOPurchaseHistory");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "IEOPurchaseHistory");
        }
    }
}
