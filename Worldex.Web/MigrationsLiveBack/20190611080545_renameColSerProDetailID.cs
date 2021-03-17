using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class renameColSerProDetailID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProviderID",
                table: "TransactionQueueArbitrage",
                newName: "SerProDetailID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SerProDetailID",
                table: "TransactionQueueArbitrage",
                newName: "ProviderID");
        }
    }
}
