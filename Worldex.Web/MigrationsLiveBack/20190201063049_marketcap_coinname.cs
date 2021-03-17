using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class marketcap_coinname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "IsMaker",
                table: "TrnChargeLog",
                nullable: false,
                oldClrType: typeof(short),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                table: "CurrencyRateMaster",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume_24h",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_7d",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_24h",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_1h",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Market_cap",
                table: "CurrencyRateDetail",
                type: "decimal(33, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyName",
                table: "CurrencyRateMaster");

            migrationBuilder.AlterColumn<short>(
                name: "IsMaker",
                table: "TrnChargeLog",
                nullable: true,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume_24h",
                table: "CurrencyRateDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CurrencyRateDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_7d",
                table: "CurrencyRateDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_24h",
                table: "CurrencyRateDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percent_change_1h",
                table: "CurrencyRateDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Market_cap",
                table: "CurrencyRateDetail",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(33, 18)");
        }
    }
}
