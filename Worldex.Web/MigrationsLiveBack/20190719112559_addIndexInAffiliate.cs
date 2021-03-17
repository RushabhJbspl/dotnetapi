using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addIndexInAffiliate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AffiliateSchemeTypeMapping_SchemeMstId",
                table: "AffiliateSchemeTypeMapping",
                column: "SchemeMstId");

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateSchemeTypeMapping_SchemeTypeMstId",
                table: "AffiliateSchemeTypeMapping",
                column: "SchemeTypeMstId");

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateCommissionHistory_AffiliateUserId_TrnUserId_SchemeMappingId",
                table: "AffiliateCommissionHistory",
                columns: new[] { "AffiliateUserId", "TrnUserId", "SchemeMappingId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AffiliateSchemeTypeMapping_SchemeMstId",
                table: "AffiliateSchemeTypeMapping");

            migrationBuilder.DropIndex(
                name: "IX_AffiliateSchemeTypeMapping_SchemeTypeMstId",
                table: "AffiliateSchemeTypeMapping");

            migrationBuilder.DropIndex(
                name: "IX_AffiliateCommissionHistory_AffiliateUserId_TrnUserId_SchemeMappingId",
                table: "AffiliateCommissionHistory");
        }
    }
}
