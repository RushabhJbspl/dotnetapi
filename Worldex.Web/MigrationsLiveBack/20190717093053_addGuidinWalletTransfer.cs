using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addGuidinWalletTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "ArbitrageWalletTransfer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArbitrageWalletTransfer_Guid",
                table: "ArbitrageWalletTransfer",
                column: "Guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArbitrageWalletTransfer_Guid",
                table: "ArbitrageWalletTransfer");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "ArbitrageWalletTransfer");
        }
    }
}
