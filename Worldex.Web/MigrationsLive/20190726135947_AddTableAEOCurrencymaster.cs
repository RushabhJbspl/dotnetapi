using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class AddTableAEOCurrencymaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IEOCurrencyMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Guid = table.Column<string>(nullable: false),
                    IEOTokenTypeName = table.Column<string>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Rounds = table.Column<short>(nullable: false),
                    IconPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IEOCurrencyMaster", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IEOCurrencyMaster_Guid",
                table: "IEOCurrencyMaster",
                column: "Guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IEOCurrencyMaster");
        }
    }
}
