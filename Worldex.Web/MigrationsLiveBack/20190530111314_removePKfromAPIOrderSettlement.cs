using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class removePKfromAPIOrderSettlement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_APIOrderSettlement",
                table: "APIOrderSettlement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_APIOrderSettlement",
                table: "APIOrderSettlement",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_APIOrderSettlement",
                table: "APIOrderSettlement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_APIOrderSettlement",
                table: "APIOrderSettlement",
                column: "TrnNo");
        }
    }
}
