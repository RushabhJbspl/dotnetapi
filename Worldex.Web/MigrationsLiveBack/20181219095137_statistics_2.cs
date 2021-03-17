using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class statistics_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StatasticsDetail",
                table: "StatasticsDetail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatasticsDetail",
                table: "StatasticsDetail",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StatasticsDetail",
                table: "StatasticsDetail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatasticsDetail",
                table: "StatasticsDetail",
                column: "StatasticsId");
        }
    }
}
