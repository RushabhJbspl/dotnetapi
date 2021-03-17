using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ChangeKycmasterAndKYCUserMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentMasterId",
                table: "kYCIdentityMaster",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LevelId",
                table: "kYCIdentityConfigurationMapping",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentMasterId",
                table: "kYCIdentityMaster");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "kYCIdentityConfigurationMapping");
        }
    }
}
