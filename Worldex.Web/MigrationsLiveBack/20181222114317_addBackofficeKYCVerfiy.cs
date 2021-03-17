using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addBackofficeKYCVerfiy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VerifyStatus",
                table: "PersonalVerification",
                nullable: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "PersonalVerification",
                maxLength: 2000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remark",
                table: "PersonalVerification");

            migrationBuilder.AlterColumn<bool>(
                name: "VerifyStatus",
                table: "PersonalVerification",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
