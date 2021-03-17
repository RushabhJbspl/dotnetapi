using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addguidinstaking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "TokenUnStakingHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "TokenStakingHistory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokenUnStakingHistory_GUID",
                table: "TokenUnStakingHistory",
                column: "GUID");

            migrationBuilder.CreateIndex(
                name: "IX_TokenStakingHistory_GUID",
                table: "TokenStakingHistory",
                column: "GUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TokenUnStakingHistory_GUID",
                table: "TokenUnStakingHistory");

            migrationBuilder.DropIndex(
                name: "IX_TokenStakingHistory_GUID",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "GUID",
                table: "TokenUnStakingHistory");

            migrationBuilder.DropColumn(
                name: "GUID",
                table: "TokenStakingHistory");
        }
    }
}
