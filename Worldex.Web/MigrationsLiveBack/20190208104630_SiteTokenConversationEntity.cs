using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class SiteTokenConversationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteTokenConversion",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    GUID = table.Column<Guid>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    SourceCurrencyID = table.Column<long>(nullable: false),
                    SourceCurrency = table.Column<string>(nullable: true),
                    TargerCurrencyID = table.Column<long>(nullable: false),
                    TargerCurrency = table.Column<string>(nullable: true),
                    SourceCurrencyQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TargerCurrencyQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SourceToBasePrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SourceToBaseQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TokenPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SiteTokenMasterID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTokenConversion", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteTokenConversion");
        }
    }
}
