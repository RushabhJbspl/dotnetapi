using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class Changedecimalsize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WithdrawAmount",
                table: "UserProfitStatistics",
                type: "decimal(35, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StartingBalance",
                table: "UserProfitStatistics",
                type: "decimal(35, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProfitPer",
                table: "UserProfitStatistics",
                type: "decimal(35, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProfitAmount",
                table: "UserProfitStatistics",
                type: "decimal(35, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EndingBalance",
                table: "UserProfitStatistics",
                type: "decimal(35, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DepositionAmount",
                table: "UserProfitStatistics",
                type: "decimal(35, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 18)");

            migrationBuilder.AlterColumn<short>(
                name: "IsMaker",
                table: "TrnChargeLog",
                nullable: true,
                oldClrType: typeof(short));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WithdrawAmount",
                table: "UserProfitStatistics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(35, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StartingBalance",
                table: "UserProfitStatistics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(35, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProfitPer",
                table: "UserProfitStatistics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(35, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProfitAmount",
                table: "UserProfitStatistics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(35, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EndingBalance",
                table: "UserProfitStatistics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(35, 18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DepositionAmount",
                table: "UserProfitStatistics",
                type: "decimal(28, 18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(35, 18)");

            migrationBuilder.AlterColumn<short>(
                name: "IsMaker",
                table: "TrnChargeLog",
                nullable: false,
                oldClrType: typeof(short),
                oldNullable: true);
        }
    }
}
