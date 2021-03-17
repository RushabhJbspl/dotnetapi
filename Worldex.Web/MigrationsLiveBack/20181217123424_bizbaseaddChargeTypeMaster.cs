using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class bizbaseaddChargeTypeMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChargeTypeId",
                table: "ChargeTypeMaster",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "UserWalletBlockTrnTypeMaster",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "UserWalletBlockTrnTypeMaster");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ChargeTypeMaster",
                newName: "ChargeTypeId");
        }
    }
}
