using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddNewColInRouteConfigAndUserSubscribePlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChannelID",
                table: "UserSubscribeAPIPlan",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "AccNoStartsWith",
                table: "RouteConfiguration",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccNoValidationRegex",
                table: "RouteConfiguration",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountNoLen",
                table: "RouteConfiguration",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelID",
                table: "UserSubscribeAPIPlan");

            migrationBuilder.DropColumn(
                name: "AccNoStartsWith",
                table: "RouteConfiguration");

            migrationBuilder.DropColumn(
                name: "AccNoValidationRegex",
                table: "RouteConfiguration");

            migrationBuilder.DropColumn(
                name: "AccountNoLen",
                table: "RouteConfiguration");
        }
    }
}
