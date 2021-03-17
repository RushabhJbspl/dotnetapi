using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addColQRCodeANDChargeCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QRCode",
                table: "UserAPIKeyDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChargeCurrency",
                table: "TransactionQueue",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCode",
                table: "UserAPIKeyDetails");

            migrationBuilder.DropColumn(
                name: "ChargeCurrency",
                table: "TransactionQueue");
        }
    }
}
