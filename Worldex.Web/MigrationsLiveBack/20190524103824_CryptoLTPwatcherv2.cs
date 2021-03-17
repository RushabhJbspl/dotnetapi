using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class CryptoLTPwatcherv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "LTP",
                table: "CryptoWatcher",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "LTP",
                table: "CryptoWatcher",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");
        }
    }
}
