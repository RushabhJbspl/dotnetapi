using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class CHECKIDENT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DBCC CHECKIDENT ('ServiceMaster', RESEED, 1000000);");
            migrationBuilder.Sql("DBCC CHECKIDENT ('ServiceProviderDetail', RESEED, 3000000);");
            migrationBuilder.Sql("DBCC CHECKIDENT ('ServiceProviderMaster', RESEED, 2000000);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
