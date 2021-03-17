using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class chnageaddresslength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OriginalAddress",
                table: "AddressMasters",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AddressMasters",
                maxLength: 160,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OriginalAddress",
                table: "AddressMasters",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AddressMasters",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 160,
                oldNullable: true);
        }
    }
}
