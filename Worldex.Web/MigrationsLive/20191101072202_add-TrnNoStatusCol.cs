using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addTrnNoStatusCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "APIStatus",
                table: "SellTopUpRequest",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "TrnNo",
                table: "SellTopUpRequest",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APIStatus",
                table: "SellTopUpRequest");

            migrationBuilder.DropColumn(
                name: "TrnNo",
                table: "SellTopUpRequest");
        }
    }
}
