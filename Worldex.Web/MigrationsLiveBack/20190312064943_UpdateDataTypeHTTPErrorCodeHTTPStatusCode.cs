using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class UpdateDataTypeHTTPErrorCodeHTTPStatusCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "HTTPStatusCode",
                table: "PublicAPIReqResLog",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "HTTPErrorCode",
                table: "PublicAPIReqResLog",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HTTPStatusCode",
                table: "PublicAPIReqResLog",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "HTTPErrorCode",
                table: "PublicAPIReqResLog",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
