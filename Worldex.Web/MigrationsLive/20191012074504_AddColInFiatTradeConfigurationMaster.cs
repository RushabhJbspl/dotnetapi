using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddColInFiatTradeConfigurationMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuyNotifyURL",
                table: "FiatTradeConfigurationMaster",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CallBackURL",
                table: "FiatTradeConfigurationMaster",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EncryptionKey",
                table: "FiatTradeConfigurationMaster",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellNotifyURL",
                table: "FiatTradeConfigurationMaster",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyNotifyURL",
                table: "FiatTradeConfigurationMaster");

            migrationBuilder.DropColumn(
                name: "CallBackURL",
                table: "FiatTradeConfigurationMaster");

            migrationBuilder.DropColumn(
                name: "EncryptionKey",
                table: "FiatTradeConfigurationMaster");

            migrationBuilder.DropColumn(
                name: "SellNotifyURL",
                table: "FiatTradeConfigurationMaster");
        }
    }
}
