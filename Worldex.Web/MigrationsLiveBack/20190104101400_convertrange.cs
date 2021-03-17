using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class convertrange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ConvertAmount",
                table: "RouteConfiguration",
                type: "decimal(22, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ConvertAmount",
                table: "RouteConfiguration",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22, 2)");
        }
    }
}
