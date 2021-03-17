using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ProfileMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompainTrail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ComplainId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(maxLength: 4000, nullable: false),
                    Remark = table.Column<string>(maxLength: 2000, nullable: false),
                    Complainstatus = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompainTrail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Complainmaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    TypeId = table.Column<long>(nullable: false),
                    Subject = table.Column<string>(maxLength: 500, nullable: false),
                    Description = table.Column<string>(maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complainmaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TypeId = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    Level = table.Column<int>(nullable: false),
                    LevelName = table.Column<string>(maxLength: 150, nullable: false),
                    DepositFee = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Withdrawalfee = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Tradingfee = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    WithdrawalLimit = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    EnableStatus = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Typemaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Type = table.Column<string>(maxLength: 100, nullable: false),
                    SubType = table.Column<string>(maxLength: 150, nullable: false),
                    EnableStatus = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Typemaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompainTrail");

            migrationBuilder.DropTable(
                name: "Complainmaster");

            migrationBuilder.DropTable(
                name: "ProfileMaster");

            migrationBuilder.DropTable(
                name: "Typemaster");
        }
    }
}
