using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class margincurrencypk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MarginWalletTypeMaster",
                table: "MarginWalletTypeMaster");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarginWalletTypeMaster",
                table: "MarginWalletTypeMaster",
                column: "WalletTypeName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MarginWalletTypeMaster",
                table: "MarginWalletTypeMaster");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarginWalletTypeMaster",
                table: "MarginWalletTypeMaster",
                column: "Id");
        }
    }
}
