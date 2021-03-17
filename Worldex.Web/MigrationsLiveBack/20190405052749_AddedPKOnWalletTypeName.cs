using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddedPKOnWalletTypeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletTypeMasters",
                table: "WalletTypeMasters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletTypeMasters",
                table: "WalletTypeMasters",
                column: "WalletTypeName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletTypeMasters",
                table: "WalletTypeMasters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletTypeMasters",
                table: "WalletTypeMasters",
                column: "Id");
        }
    }
}
