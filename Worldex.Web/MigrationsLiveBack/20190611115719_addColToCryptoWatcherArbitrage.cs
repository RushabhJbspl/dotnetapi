using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addColToCryptoWatcherArbitrage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ChangePer",
                table: "CryptoWatcherArbitrage",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Fees",
                table: "CryptoWatcherArbitrage",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "PairId",
                table: "CryptoWatcherArbitrage",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "UpDownBit",
                table: "CryptoWatcherArbitrage",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "Volume",
                table: "CryptoWatcherArbitrage",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "PairID",
                table: "ArbitrageWalletTransactionQueue",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangePer",
                table: "CryptoWatcherArbitrage");

            migrationBuilder.DropColumn(
                name: "Fees",
                table: "CryptoWatcherArbitrage");

            migrationBuilder.DropColumn(
                name: "PairId",
                table: "CryptoWatcherArbitrage");

            migrationBuilder.DropColumn(
                name: "UpDownBit",
                table: "CryptoWatcherArbitrage");

            migrationBuilder.DropColumn(
                name: "Volume",
                table: "CryptoWatcherArbitrage");

            migrationBuilder.DropColumn(
                name: "PairID",
                table: "ArbitrageWalletTransactionQueue");
        }
    }
}
