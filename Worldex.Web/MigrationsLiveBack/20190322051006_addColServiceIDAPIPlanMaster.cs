using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addColServiceIDAPIPlanMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ServiceID",
                table: "UserSubscribeAPIPlan",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "APIPlanMaster",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ServiceID",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceID",
                table: "UserSubscribeAPIPlan");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "ServiceID",
                table: "APIPlanMaster");
        }
    }
}
