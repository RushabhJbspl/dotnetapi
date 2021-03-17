using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class APIReqResStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "RestMethods",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "APIReqResStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    MethodID = table.Column<long>(nullable: false),
                    IPId = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    SuccessCount = table.Column<long>(nullable: false),
                    FaliureCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIReqResStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicAPIReqResLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    MethodID = table.Column<long>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    HTTPErrorCode = table.Column<string>(nullable: true),
                    HTTPStatusCode = table.Column<string>(nullable: true),
                    Device = table.Column<string>(nullable: true),
                    Browser = table.Column<string>(nullable: true),
                    Host = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicAPIReqResLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIReqResStatistics");

            migrationBuilder.DropTable(
                name: "PublicAPIReqResLog");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "RestMethods");
        }
    }
}
