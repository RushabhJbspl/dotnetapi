using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddedPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
               name: "PK_TradePairMaster",
               table: "TradePairMaster",
               column: "Id");

            migrationBuilder.AddPrimaryKey(
               name: "PK_TrnAcBatch",
               table: "TrnAcBatch",
               column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
