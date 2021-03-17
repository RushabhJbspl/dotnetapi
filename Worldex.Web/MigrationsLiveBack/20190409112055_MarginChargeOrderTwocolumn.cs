using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class MarginChargeOrderTwocolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseCurrency",
                table: "OpenPositionMaster",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BaseCurrency",
                table: "MarginChargeOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "MarginChargeOrder",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseCurrency",
                table: "OpenPositionMaster");

            migrationBuilder.DropColumn(
                name: "BaseCurrency",
                table: "MarginChargeOrder");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "MarginChargeOrder");
        }
    }
}
