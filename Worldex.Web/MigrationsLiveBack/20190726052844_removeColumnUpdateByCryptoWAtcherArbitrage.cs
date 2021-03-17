using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class removeColumnUpdateByCryptoWAtcherArbitrage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "CryptoWatcherArbitrage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UpdateDate",
                table: "CryptoWatcherArbitrage",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
