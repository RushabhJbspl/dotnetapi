using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class TQAddSerProDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "LPType",
                table: "TransactionQueue",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "SerProDetailID",
                table: "TransactionQueue",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LPType",
                table: "TransactionQueue");

            migrationBuilder.DropColumn(
                name: "SerProDetailID",
                table: "TransactionQueue");
        }
    }
}
