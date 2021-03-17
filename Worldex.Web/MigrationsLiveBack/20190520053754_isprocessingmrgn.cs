using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class isprocessingmrgn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsProcessing",
                table: "WalletTransactionQueues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IsProcessing",
                table: "MarginWalletTransactionQueue",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessing",
                table: "WalletTransactionQueues");

            migrationBuilder.DropColumn(
                name: "IsProcessing",
                table: "MarginWalletTransactionQueue");
        }
    }
}
