using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class LimitchargeTableV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChargeRuleMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TrnType = table.Column<int>(nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    WalletType = table.Column<long>(nullable: false),
                    ChargeType = table.Column<int>(nullable: false),
                    ChargeValue = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeRuleMaster", x => new { x.TrnType, x.WalletType });
                });

            migrationBuilder.CreateTable(
                name: "LimitRuleMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TrnType = table.Column<int>(nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    WalletType = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitRuleMaster", x => new { x.TrnType, x.WalletType });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargeRuleMaster");

            migrationBuilder.DropTable(
                name: "LimitRuleMaster");
        }
    }
}
