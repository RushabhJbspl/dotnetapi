using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ipaddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletAuthorizeUserMaster",
                table: "WalletAuthorizeUserMaster");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletAuthorizeUserMaster",
                table: "WalletAuthorizeUserMaster",
                columns: new[] { "UserID", "WalletID" });

            migrationBuilder.CreateTable(
                name: "IPAddressMaster",
                columns: table => new
                {
                    AutoNo = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FromIP = table.Column<byte[]>(nullable: false),
                    ToIP = table.Column<byte[]>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Longitude = table.Column<float>(nullable: false),
                    Lattitude = table.Column<float>(nullable: false),
                    CountryCode = table.Column<string>(maxLength: 5, nullable: false),
                    CountryName = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPAddressMaster", x => x.AutoNo);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IPAddressMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletAuthorizeUserMaster",
                table: "WalletAuthorizeUserMaster");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletAuthorizeUserMaster",
                table: "WalletAuthorizeUserMaster",
                columns: new[] { "UserID", "WalletID", "RoleID" });
        }
    }
}
