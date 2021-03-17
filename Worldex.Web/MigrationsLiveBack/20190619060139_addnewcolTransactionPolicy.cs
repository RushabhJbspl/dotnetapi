using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addnewcolTransactionPolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DailyVelocityAddressAmount",
                table: "TransactionPolicy",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "DailyVelocityAddressCount",
                table: "TransactionPolicy",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyVelocityAddressAmount",
                table: "TransactionPolicy",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "MonthlyVelocityAddressCount",
                table: "TransactionPolicy",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "WeeklyVelocityAddressAmount",
                table: "TransactionPolicy",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "WeeklyVelocityAddressCount",
                table: "TransactionPolicy",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "YearlyVelocityAddressAmount",
                table: "TransactionPolicy",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "YearlyVelocityAddressCount",
                table: "TransactionPolicy",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyVelocityAddressAmount",
                table: "TransactionPolicy");

            migrationBuilder.DropColumn(
                name: "DailyVelocityAddressCount",
                table: "TransactionPolicy");

            migrationBuilder.DropColumn(
                name: "MonthlyVelocityAddressAmount",
                table: "TransactionPolicy");

            migrationBuilder.DropColumn(
                name: "MonthlyVelocityAddressCount",
                table: "TransactionPolicy");

            migrationBuilder.DropColumn(
                name: "WeeklyVelocityAddressAmount",
                table: "TransactionPolicy");

            migrationBuilder.DropColumn(
                name: "WeeklyVelocityAddressCount",
                table: "TransactionPolicy");

            migrationBuilder.DropColumn(
                name: "YearlyVelocityAddressAmount",
                table: "TransactionPolicy");

            migrationBuilder.DropColumn(
                name: "YearlyVelocityAddressCount",
                table: "TransactionPolicy");
        }
    }
}
