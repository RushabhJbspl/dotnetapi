using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddColMethodTypeIPAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "PublicAPIReqResLog",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MethodType",
                table: "PublicAPIReqResLog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "PublicAPIReqResLog");

            migrationBuilder.DropColumn(
                name: "MethodType",
                table: "PublicAPIReqResLog");
        }
    }
}
