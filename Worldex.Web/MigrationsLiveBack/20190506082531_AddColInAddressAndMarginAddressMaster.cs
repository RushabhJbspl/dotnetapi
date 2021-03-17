using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddColInAddressAndMarginAddressMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Param1",
                table: "ServiceProConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param2",
                table: "ServiceProConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param3",
                table: "ServiceProConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param4",
                table: "ServiceProConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param5",
                table: "ServiceProConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TxnID",
                table: "MarginAddressMaster",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TxnID",
                table: "AddressMasters",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Param1",
                table: "ServiceProConfiguration");

            migrationBuilder.DropColumn(
                name: "Param2",
                table: "ServiceProConfiguration");

            migrationBuilder.DropColumn(
                name: "Param3",
                table: "ServiceProConfiguration");

            migrationBuilder.DropColumn(
                name: "Param4",
                table: "ServiceProConfiguration");

            migrationBuilder.DropColumn(
                name: "Param5",
                table: "ServiceProConfiguration");

            migrationBuilder.DropColumn(
                name: "TxnID",
                table: "MarginAddressMaster");

            migrationBuilder.DropColumn(
                name: "TxnID",
                table: "AddressMasters");
        }
    }
}
