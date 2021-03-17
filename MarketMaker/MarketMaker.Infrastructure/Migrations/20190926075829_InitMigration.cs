using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MarketMaker.Infrastructure.Migrations
{
    public partial class InitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commons",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GUID = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketMakerPreferences",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GUID = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    PairId = table.Column<int>(nullable: false),
                    BuyLTPPrefProID = table.Column<long>(nullable: false),
                    SellLTPPrefProID = table.Column<long>(nullable: false),
                    BuyUpPercentage = table.Column<int>(nullable: false),
                    BuyDownPercentage = table.Column<int>(nullable: false),
                    SellUpPercentage = table.Column<int>(nullable: false),
                    SellDownPercentage = table.Column<int>(nullable: false),
                    BuyUpThreshold = table.Column<int>(nullable: false),
                    BuyDownThreshold = table.Column<int>(nullable: false),
                    SellUpThreshold = table.Column<int>(nullable: false),
                    SellDownThreshold = table.Column<int>(nullable: false),
                    RangeType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketMakerPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketMakerRangeDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GUID = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    PreferenceId = table.Column<long>(nullable: false),
                    RangeMin = table.Column<float>(nullable: false),
                    RangeMax = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketMakerRangeDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commons");

            migrationBuilder.DropTable(
                name: "MarketMakerPreferences");

            migrationBuilder.DropTable(
                name: "MarketMakerRangeDetails");
        }
    }
}
