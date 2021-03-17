using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class SiteTokenMasterV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxxLimit",
                table: "SiteTokenMaster",
                newName: "MaxLimit");

            migrationBuilder.AddColumn<string>(
                name: "BaseCurrencySMSCode",
                table: "SiteTokenMaster",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrencySMSCode",
                table: "SiteTokenMaster",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseCurrencySMSCode",
                table: "SiteTokenMaster");

            migrationBuilder.DropColumn(
                name: "CurrencySMSCode",
                table: "SiteTokenMaster");

            migrationBuilder.RenameColumn(
                name: "MaxLimit",
                table: "SiteTokenMaster",
                newName: "MaxxLimit");
        }
    }
}
