using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddRegTypeIdinbizuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "IsMaker",
                table: "TrnChargeLog",
                nullable: true,
                oldClrType: typeof(short));

            migrationBuilder.AddColumn<int>(
                name: "RegTypeId",
                table: "BizUser",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegTypeId",
                table: "BizUser");

            migrationBuilder.AlterColumn<short>(
                name: "IsMaker",
                table: "TrnChargeLog",
                nullable: false,
                oldClrType: typeof(short),
                oldNullable: true);
        }
    }
}
