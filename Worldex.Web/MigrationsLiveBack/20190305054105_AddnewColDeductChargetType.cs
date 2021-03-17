using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddnewColDeductChargetType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "DeductChargetType",
                table: "ChargeConfigurationDetail",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "APIPlanDetail",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeductChargetType",
                table: "ChargeConfigurationDetail");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "APIPlanDetail");
        }
    }
}
