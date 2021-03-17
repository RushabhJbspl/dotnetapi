using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addServiceDetailArbitrageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductConfigurationArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ProductName = table.Column<string>(maxLength: 30, nullable: false),
                    ServiceID = table.Column<long>(nullable: false),
                    CountryID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConfigurationArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDetailArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ServiceId = table.Column<long>(nullable: false),
                    ServiceDetailJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDetailArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceStasticsArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ServiceId = table.Column<long>(nullable: false),
                    MarketCap = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    VolGlobal = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxSupply = table.Column<long>(nullable: false),
                    CirculatingSupply = table.Column<long>(nullable: false),
                    IssuePrice = table.Column<decimal>(nullable: false),
                    IssueDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStasticsArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypeMappingArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ServiceId = table.Column<long>(nullable: false),
                    TrnType = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypeMappingArbitrage", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductConfigurationArbitrage");

            migrationBuilder.DropTable(
                name: "ServiceDetailArbitrage");

            migrationBuilder.DropTable(
                name: "ServiceStasticsArbitrage");

            migrationBuilder.DropTable(
                name: "ServiceTypeMappingArbitrage");
        }
    }
}
