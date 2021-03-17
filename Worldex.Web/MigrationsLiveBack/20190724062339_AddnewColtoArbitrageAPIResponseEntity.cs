using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddnewColtoArbitrageAPIResponseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Param4Regex",
                table: "ThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "Param5Regex",
                table: "ThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "Param6Regex",
                table: "ThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "Param7Regex",
                table: "ThirdPartyAPIResponseConfiguration");

            migrationBuilder.AddColumn<string>(
                name: "Param4Regex",
                table: "ArbitrageThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param5Regex",
                table: "ArbitrageThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param6Regex",
                table: "ArbitrageThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param7Regex",
                table: "ArbitrageThirdPartyAPIResponseConfiguration",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Param4Regex",
                table: "ArbitrageThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "Param5Regex",
                table: "ArbitrageThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "Param6Regex",
                table: "ArbitrageThirdPartyAPIResponseConfiguration");

            migrationBuilder.DropColumn(
                name: "Param7Regex",
                table: "ArbitrageThirdPartyAPIResponseConfiguration");

            migrationBuilder.AddColumn<string>(
                name: "Param4Regex",
                table: "ThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param5Regex",
                table: "ThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param6Regex",
                table: "ThirdPartyAPIResponseConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param7Regex",
                table: "ThirdPartyAPIResponseConfiguration",
                nullable: true);
        }
    }
}
