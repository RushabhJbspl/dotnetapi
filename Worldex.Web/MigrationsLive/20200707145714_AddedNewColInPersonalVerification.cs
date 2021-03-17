using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddedNewColInPersonalVerification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GUID",
                table: "IpMaster");

            migrationBuilder.AddColumn<string>(
                name: "IdentityDocNumber",
                table: "PersonalVerification",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityDocNumber",
                table: "PersonalVerification");

            migrationBuilder.AddColumn<Guid>(
                name: "GUID",
                table: "IpMaster",
                nullable: true);
        }
    }
}
