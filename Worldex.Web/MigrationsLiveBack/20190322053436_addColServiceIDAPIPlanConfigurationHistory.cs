using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addColServiceIDAPIPlanConfigurationHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "APIPlanConfigurationHistory",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ServiceID",
                table: "APIPlanConfigurationHistory",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "APIPlanConfigurationHistory");

            migrationBuilder.DropColumn(
                name: "ServiceID",
                table: "APIPlanConfigurationHistory");
        }
    }
}
