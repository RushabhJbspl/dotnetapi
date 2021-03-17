using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class RenameColOriginalAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConvertedAmount",
                table: "TrnChargeLog",
                newName: "OriginalAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalAmount",
                table: "TrnChargeLog",
                newName: "ConvertedAmount");
        }
    }
}
