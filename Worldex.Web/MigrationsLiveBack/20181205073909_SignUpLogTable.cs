using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class SignUpLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SignUpLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TempUserId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    RegisterType = table.Column<int>(nullable: false),
                    DeviceID = table.Column<string>(maxLength: 2000, nullable: true),
                    Mode = table.Column<string>(maxLength: 10, nullable: true),
                    IPAddress = table.Column<string>(maxLength: 15, nullable: true),
                    Location = table.Column<string>(maxLength: 2000, nullable: true),
                    HostName = table.Column<string>(maxLength: 250, nullable: true),
                    RegisterStatus = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignUpLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignUpLog");
        }
    }
}
