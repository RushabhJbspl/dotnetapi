using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ForVishveta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeverageMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletTypeID = table.Column<long>(nullable: false),
                    LeveragePer = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeverageMaster", x => x.WalletTypeID);
                });

            migrationBuilder.CreateTable(
                name: "StopLossMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletTypeID = table.Column<long>(nullable: false),
                    StopLossPer = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopLossMaster", x => x.WalletTypeID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeverageMaster");

            migrationBuilder.DropTable(
                name: "StopLossMaster");
        }
    }
}
