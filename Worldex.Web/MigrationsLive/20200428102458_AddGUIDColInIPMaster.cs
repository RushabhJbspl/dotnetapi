using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddGUIDColInIPMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GUID",
                table: "IpMaster",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GUID",
                table: "IpMaster");
        }
    }
}
