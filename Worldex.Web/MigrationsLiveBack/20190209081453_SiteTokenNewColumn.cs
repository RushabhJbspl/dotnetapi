using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class SiteTokenNewColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusMsg",
                table: "SiteTokenConversion",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeStamp",
                table: "SiteTokenConversion",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusMsg",
                table: "SiteTokenConversion");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "SiteTokenConversion");
        }
    }
}
