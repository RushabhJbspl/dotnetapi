using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class organizationbackofficetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityRegister",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    EventId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false),
                    Connection = table.Column<string>(maxLength: 500, nullable: true),
                    Application = table.Column<string>(maxLength: 500, nullable: true),
                    EndPoint = table.Column<string>(maxLength: 500, nullable: true),
                    StatusCode = table.Column<long>(nullable: false),
                    Channel = table.Column<string>(maxLength: 500, nullable: true),
                    Request = table.Column<string>(maxLength: 4000, nullable: true),
                    Response = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRegister", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationMaster",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    ApplicationName = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Org_App_Mapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    OrgId = table.Column<Guid>(nullable: false),
                    AppId = table.Column<Guid>(nullable: false),
                    AppName = table.Column<string>(maxLength: 250, nullable: false),
                    ClientSecret = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    ApplicationLogo = table.Column<string>(maxLength: 250, nullable: true),
                    AllowedCallBackURLS = table.Column<string>(maxLength: 500, nullable: true),
                    AllowedWebOrigins = table.Column<string>(maxLength: 500, nullable: true),
                    AllowedLogoutURLS = table.Column<string>(maxLength: 500, nullable: true),
                    AllowedOriginsCORS = table.Column<string>(maxLength: 500, nullable: true),
                    JWTExpiration = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Org_App_Mapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organization_Master",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    DomainName = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization_Master", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityRegister");

            migrationBuilder.DropTable(
                name: "ApplicationMaster");

            migrationBuilder.DropTable(
                name: "Org_App_Mapping");

            migrationBuilder.DropTable(
                name: "Organization_Master");
        }
    }
}
