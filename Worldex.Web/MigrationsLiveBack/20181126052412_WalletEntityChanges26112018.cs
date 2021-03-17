using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class WalletEntityChanges26112018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RoleID",
                table: "WalletAuthorizeUserMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "RoleID",
                table: "OrganizationUserMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "OrganizationMaster",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "AuthenticationType",
                table: "OrganizationMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "CityID",
                table: "OrganizationMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ContactNo",
                table: "OrganizationMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OrganizationMaster",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TnCAccepted",
                table: "OrganizationMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "OrganizationMaster",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "WalletAuthorizeUserMaster");

            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "OrganizationUserMaster");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "OrganizationMaster");

            migrationBuilder.DropColumn(
                name: "AuthenticationType",
                table: "OrganizationMaster");

            migrationBuilder.DropColumn(
                name: "CityID",
                table: "OrganizationMaster");

            migrationBuilder.DropColumn(
                name: "ContactNo",
                table: "OrganizationMaster");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "OrganizationMaster");

            migrationBuilder.DropColumn(
                name: "TnCAccepted",
                table: "OrganizationMaster");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "OrganizationMaster");
        }
    }
}
