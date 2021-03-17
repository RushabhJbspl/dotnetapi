using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class RenewDays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RenewDays",
                table: "UserSubscribeAPIPlan",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "IsCreatedByAdmin",
                table: "BizUser",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "BizUser",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewDays",
                table: "UserSubscribeAPIPlan");

            migrationBuilder.DropColumn(
                name: "IsCreatedByAdmin",
                table: "BizUser");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BizUser");
        }
    }
}
