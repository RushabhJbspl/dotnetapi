using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class CoinRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ConvertAmount",
                table: "RouteConfiguration",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22, 2)");

            migrationBuilder.CreateTable(
                name: "coinListRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    CoinName = table.Column<string>(maxLength: 30, nullable: false),
                    CoinAbbreviationCode = table.Column<string>(maxLength: 6, nullable: false),
                    IconUrl = table.Column<string>(nullable: true),
                    TotalSupply = table.Column<long>(nullable: false),
                    MaxSupply = table.Column<long>(nullable: false),
                    IssueDate = table.Column<DateTime>(nullable: false),
                    IssuePrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    CirculatingSupply = table.Column<long>(nullable: false),
                    WebsiteUrl = table.Column<string>(nullable: false),
                    Explorer = table.Column<string>(type: "text", nullable: true),
                    Community = table.Column<string>(type: "text", nullable: true),
                    Introduction = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coinListRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coinListRequests");

            migrationBuilder.AlterColumn<decimal>(
                name: "ConvertAmount",
                table: "RouteConfiguration",
                type: "decimal(22, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");
        }
    }
}
