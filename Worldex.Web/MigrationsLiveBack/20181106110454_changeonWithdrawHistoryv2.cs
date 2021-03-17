using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class changeonWithdrawHistoryv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropUniqueConstraint(
            //    name: "AK_WithdrawHistory_Address_TrnID",
            //    table: "WithdrawHistory");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_WithdrawHistory",
            //    table: "WithdrawHistory");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "WithdrawHistory",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "TrnID",
                table: "WithdrawHistory",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WithdrawHistory",
                table: "WithdrawHistory",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WithdrawHistory",
                table: "WithdrawHistory");

            migrationBuilder.AlterColumn<string>(
                name: "TrnID",
                table: "WithdrawHistory",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "WithdrawHistory",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WithdrawHistory_Address_TrnID",
                table: "WithdrawHistory",
                columns: new[] { "Address", "TrnID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WithdrawHistory",
                table: "WithdrawHistory",
                columns: new[] { "TrnID", "Address" });
        }
    }
}
